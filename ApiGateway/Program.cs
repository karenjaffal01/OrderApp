using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using CacheManager.Core;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCacheManager<ICacheManager<object>>(builder.Configuration, "CacheManager", y =>
{
    y.WithDictionaryHandle();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("http://localhost:7002/swagger/v1/swagger.json", "Orders Service");
    c.SwaggerEndpoint("http://localhost:7003/swagger/v1/swagger.json", "Items Service");
    c.SwaggerEndpoint("http://localhost:7004/swagger/v1/swagger.json", "Stock Service");
    c.SwaggerEndpoint("http://localhost:7001/swagger/v1/swagger.json", "Auth Service");

    c.RoutePrefix = "swagger";
});

await app.UseOcelot();

app.Run();
