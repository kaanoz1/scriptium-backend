using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ScriptiumBackend.Models;
using ScriptiumBackend.Services;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using FluentValidation.AspNetCore;
using ScriptiumBackend.MiddleWare;
using ScriptiumBackend.Interface;
using ScriptiumBackend.DB;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); 
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

Log.Information("Application is starting...");

builder.Host.UseSerilog();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("StaticControllerRateLimiter", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromSeconds(1),
                AutoReplenishment = true,
                QueueLimit = 0
            }));

    options.AddPolicy("AuthControllerRateLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User?.Identity?.Name
                          ?? context.Connection.RemoteIpAddress?.ToString()
                          ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,               
                Window = TimeSpan.FromSeconds(10),
                AutoReplenishment = true,
                QueueLimit = 0
            }));

    options.OnRejected = async (ctx, ct) =>
    {
        var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Rate limit exceeded for {Path} from User {User}",
            ctx.HttpContext.Request.Path,
            ctx.HttpContext.User.Identity?.Name ?? "unknown");

        // Retry-After ekle (süzülen meta varsa kullan)
        if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            ctx.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
        }

        ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await ctx.HttpContext.Response.WriteAsync(
            "Rate limit exceeded. Please try again later.", ct);
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(180));
});

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddSingleton<LuceneIndexerService>();
builder.Services.AddHostedService<IndexSyncBackgroundService>();
builder.Services.AddScoped<ISearchService, LuceneSearcherService>();


/*
    THESE SERVICES ARE TEMPORARILY DISABLED SINCE THE BACKEND SERVICE IS CLOSED FOR USER DATA.

builder.Services.AddSingleton<ITicketStore, SessionStore>();
builder.Services.AddHostedService<UserDeletionBackgroundService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 6;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789._";
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnValidatePrincipal = async context =>
    {
        var ticketStore = context.HttpContext.RequestServices.GetRequiredService<ITicketStore>();
        context.Options.SessionStore = ticketStore;

        await Task.CompletedTask;
    };
    options.LoginPath = "/auth/login";
    options.ExpireTimeSpan = TimeSpan.FromDays(3);
    options.SlidingExpiration = false;

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Name = "sessionB";
});
*/

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Log.Information("Development environment detected. Swagger is enabled.");


}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseSerilogRequestLogging();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
    Log.Information("Environment is production. HttpsRedirection enabled.");
}

app.UseRouting();

app.UseCors("AllowAll");


// app.UseAuthentication();
// app.UseAuthorization();

app.UseRateLimiter();

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();
