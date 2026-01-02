using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Services.ConcreteServices.Cache;
using ScriptiumBackend.Services.ServiceInterfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(theme: ScriptiumBackend.Utils.Constants.Configurations.Logger.Theme)
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddScoped<ICacheService, MainCacheService>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ScriptiumDbContext>(options =>
{
    options.UseNpgsql(connectionString ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    Log.Information("Scalar API Documentation is available at: http://localhost:5005/scalar/v1");
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();


