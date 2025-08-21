using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var password = builder.Configuration["DbPassword"];
if (!string.IsNullOrEmpty(password))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") + password;
    builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
}

builder.Services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();

var app = builder.Build();
app.UseMiddleware<RestrictAccessMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseAuthorization();
app.MapControllers();
app.Run();
