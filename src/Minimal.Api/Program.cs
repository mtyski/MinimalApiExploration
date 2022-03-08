using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Application.Handlers.TodoItems;
using Minimal.Application.PipelineBehavior;
using Minimal.Db;

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

builder.Services.AddMediatR(typeof(ValidationPipelineBehavior), typeof(GetAll.Request.Handler));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

builder.Services.AddValidatorsFromAssemblyContaining<Delete.Request.Validator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// add endpoints
app.AddEndpoints();

app.UseHttpsRedirection();

app.Run();

namespace Minimal.Api
{
    /// <summary>
    ///     Required by integration tests.
    /// </summary>
    public partial class Program { }
}