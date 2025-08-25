using CorrelationId;
using CorrelationId.DependencyInjection;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
else
{
    Console.WriteLine("Warning: Database password is not configured");
}

builder.Services.AddScoped<IStockUnitOfWork, StockUnitOfWork>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
        };
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection") + builder.Configuration["DbPassword"],
        name: "postgresql",
        tags: new[] { "db" }
    )
    .AddCheck("self", () => HealthCheckResult.Healthy("Service is alive"));
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock API v1");
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Name == "self"
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db")
});


//app.UseMiddleware<RestrictAccessMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
