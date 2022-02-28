using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Minimal.Db;
using Minimal.Model;
using Minimal.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoContext>(opt => opt.UseSqlite(
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

        item.Rename(inputDto.Name);
        item.SetStatus((TodoItem.State)inputDto.Status);
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

// required by the integration tests
public partial class Program { }