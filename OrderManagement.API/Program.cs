using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderManagement.Business.Interfaces;
using OrderManagement.Business.Services;
using OrderManagement.Persistence;
using OrderManagement.Persistence.Interfaces;
using OrderManagement.Persistence.Repositories;
using System.Data;
using Serilog;
using Serilog.Enrichers;
using Serilog.Enrichers.CorrelationId;
using CorrelationId;
using CorrelationId.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

var password = builder.Configuration["DbPassword"];

if (string.IsNullOrEmpty(password))
{
    Console.WriteLine("Warning: Database password is not configured. Skipping DB connection.");
}
else
{
    var connectionStringWithoutPassword = builder.Configuration.GetConnectionString("DefaultConnection");
    var connectionString = connectionStringWithoutPassword + password;

    builder.Services.AddScoped<IDbConnection>(_ =>
        new NpgsqlConnection(connectionString));
}
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddControllers();

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
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

