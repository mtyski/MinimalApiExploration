using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Minimal.Api;
using Minimal.Db;
using Minimal.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoContext>(
    opt => opt.UseSqlite(
        new SqliteConnectionStringBuilder
        {
            DataSource = builder.Configuration["Database:Source"],
            Mode = Enum.Parse<SqliteOpenMode>(builder.Configuration["Database:Mode"]),
            Cache = Enum.Parse<SqliteCacheMode>(builder.Configuration["Database:Cache"])
        }.ToString()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// add endpoints
app.MapGet(
    "/items",
    async (TodoContext context) => await context.Items.ToListAsync());

app.MapGet(
    "/items/{id:long}",
    async (long id, TodoContext context) =>
        await context.Items.FindAsync(id) is { } item ?
            Results.Ok(TodoItemDto.FromTodoItem(item)) :
            Results.NotFound($"Todo item with id: {id} was not found!")
    );

app.MapPost(
    "/items",
    async (NewTodoItemDto newItemDto, TodoContext context) =>
    {
        if (string.IsNullOrWhiteSpace(newItemDto.Name))
        {
            return Results.BadRequest("Name of the new item cannot be empty");
        }

        var newItem = TodoItem.Create(newItemDto.Name);
        context.Items.Add(newItem);
        await context.SaveChangesAsync();

        return Results.Created($"/items/{newItem.Id}", TodoItemDto.FromTodoItem(newItem));
    });

app.MapPut(
    "/items/{id:long}",
    async (long id, TodoItemDto inputDto, TodoContext context) =>
    {
        if (await context.Items.FindAsync(id) is not { } item)
        {
            return Results.NotFound($"Todo item with id: {id} was not found!");
        }

        if (!TodoItem.CanRename(inputDto.Name))
        {
            return Results.BadRequest($"New name cannot be empty!");
        }

        if (!TodoItem.CanSetState((TodoItem.State)inputDto.Status))
        {
            return Results.BadRequest($"New state has to be a value from enumeration range!");
        }

        item.Rename(inputDto.Name);
        item.SetState((TodoItem.State)inputDto.Status);
        context.Update(item);

        await context.SaveChangesAsync();

        return Results.NoContent();
    });

app.MapDelete(
    "/items/{id:long}",
    async (long id, TodoContext context) =>
    {
        if (await context.Items.FindAsync(id) is not { } item)
        {
            return Results.NotFound($"Todo item with id: {id} was not found!");
        }

        context.Items.Remove(item);
        await context.SaveChangesAsync();

        return Results.NoContent();
    });

app.UseHttpsRedirection();

app.Run();

namespace Minimal.Api
{
    /// <summary>
    ///     Required by integration tests.
    /// </summary>
    public partial class Program { }
}