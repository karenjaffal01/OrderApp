using CorrelationId;
using CorrelationId.DependencyInjection;
using Dapper;
using Microsoft.OpenApi.Models;
using Npgsql;
using OrderManagement.Business.Interfaces;
using OrderManagement.Business.Services;
using OrderManagement.Persistence;
using OrderManagement.Persistence.Interfaces;
using OrderManagement.Persistence.Repositories;
using OrderManagement.Persistence.UnitOfWorks;
using Serilog;
using Serilog.Enrichers.CorrelationId;
using SharedLibrary;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var password = builder.Configuration["DbPassword"];
if (!string.IsNullOrEmpty(password))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") + password;
    builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
}
else
{
    Console.WriteLine("Warning: Database password is not configured. Skipping DB connection.");
}

builder.Services.AddScoped<IItemUnitOfWork, ItemUnitOfWork>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IStockUnitOfWork, StockUnitOfWork>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console()
    .WriteTo.File("Logs/item-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCorrelationId(options =>
{
    options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
    options.UpdateTraceIdentifier = true;
    options.AddToLoggingScope = true;
    options.IncludeInResponse = true;
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseMiddleware<RestrictAccessMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
