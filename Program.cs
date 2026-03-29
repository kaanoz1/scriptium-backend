using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Services.ConcreteServices.BackgroundServices;
using ScriptiumBackend.Services.ConcreteServices.Cache;
using ScriptiumBackend.Services.ConcreteServices.Embedding;
using ScriptiumBackend.Services.ServiceInterfaces;
using ScriptiumBackend.Utils.Environment;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(theme: ScriptiumBackend.Utils.Constants.Configurations.Logger.Theme)
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    ScriptiumEnvironmentGuard.Initialize(builder.Configuration);
    ScriptiumEnvironmentGuard.Validate();

    var allowedOrigins = ScriptiumEnvironmentGuard.GetAllowedOrigins();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            if (allowedOrigins.Length == 0) return;

            foreach (var origin in allowedOrigins)
                Log.Information("Registered CORS Origin: {Origin}", origin);

            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddValidation();

    builder.Services.AddScoped<ICacheService, MainCacheService>();
    builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>();
    builder.Services.AddHostedService<IndexingWorker>();

    var connectionString = ScriptiumEnvironmentGuard.GetConnectionString();

    builder.Services.AddDbContext<ScriptiumDbContext>(options =>
    {
        options.UseNpgsql(connectionString,
            npgsqlOptions => { npgsqlOptions.UseVector(); });
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ScriptiumDbContext>();
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
                Log.Information("Migrations applied.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database migration failed.");
        }
    }

    app.UseCors("CorsPolicy");
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        Log.Information("Scalar API: http://localhost:5005/scalar/v1");
    }

    app.UseHttpsRedirection();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}