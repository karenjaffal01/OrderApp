using ApiGateway.Middleware; // Your RestrictAccessMiddleware or InterceptionMiddleware
using MMLib.SwaggerForOcelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot config
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Register Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Register SwaggerForOcelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Enable CORS
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Use CORS and your custom middleware
app.UseCors();
app.UseMiddleware<InterceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Map controllers if needed
app.MapControllers();

// Serve aggregated Swagger UI at gateway
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

// Run Ocelot
await app.UseOcelot();

app.Run();
