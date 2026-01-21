using Microsoft.EntityFrameworkCore;
using Npgsql;
using Scalar.AspNetCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Services.ConcreteServices.BackgroundServices;
using ScriptiumBackend.Services.ConcreteServices.Cache;
using ScriptiumBackend.Services.ConcreteServices.Embedding;
using ScriptiumBackend.Services.ServiceInterfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(theme: ScriptiumBackend.Utils.Constants.Configurations.Logger.Theme)
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddValidation();

builder.Services.AddScoped<ICacheService, MainCacheService>();
builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>();
builder.Services.AddHostedService<IndexingWorker>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ScriptiumDbContext>(options =>
{
    options.UseNpgsql(connectionString ??
                      throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
        npgsqlOptions => { npgsqlOptions.UseVector(); });
});

var app = builder.Build();
app.UseCors("AllowAll");

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