using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Application.Handlers.TodoItems;
using Minimal.Application.PipelineBehavior;
using Minimal.Db;
using Microsoft.AspNetCore.Http.Json;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(
    static (_, config) => config.AddEnvironmentVariables());

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoContext>(
    opt => opt.UseNpgsql(new NpgsqlConnectionStringBuilder
    {
        Host = builder.Configuration["Database:Host"],
        Database = builder.Configuration["Database:Source"],
        Username = builder.Configuration["Database:User"],
        Password = builder.Configuration["Database:Password"],
        Port = int.Parse(builder.Configuration["Database:Port"])
    }.ToString()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddMediatR(typeof(ValidationPipelineBehavior));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

builder.Services.AddValidatorsFromAssemblyContaining<Delete.Request.Validator>();

builder.Services.Configure<JsonOptions>(
    static opts => opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// add endpoints
app.AddEndpoints();

app.Migrate<TodoContext>();

app.UseHttpsRedirection();

app.Run();

namespace Minimal.Api
{
    /// <summary>
    ///     Required by integration tests.
    /// </summary>
    public partial class Program { }
}