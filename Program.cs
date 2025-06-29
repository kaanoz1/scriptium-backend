using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Models;
using scriptium_backend_dotnet.Services;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using FluentValidation.AspNetCore;
using scriptium_backend_dotnet.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5227); 
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

builder.Services.AddRateLimiter(option =>
{
    option.AddFixedWindowLimiter(policyName: "StaticControllerRateLimiter", windowsOptions =>
    {
        windowsOptions.PermitLimit = 5;
        windowsOptions.Window = TimeSpan.FromSeconds(10);
    });

    option.AddFixedWindowLimiter(policyName: "AuthControllerRateLimit", windowsOptions =>
    {
        windowsOptions.PermitLimit = 10;
        windowsOptions.Window = TimeSpan.FromDays(1);
    });

    option.OnRejected = async (context, CancellationToken) =>
    {
        ILogger<Program> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Rate limit exceeded for {Path} from User {User}", context.HttpContext.Request.Path, context.HttpContext.User.Identity?.Name ?? "unknown");

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", CancellationToken);
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://192.168.1.2:3000")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), options => options.CommandTimeout(180)));

builder.Services.AddSingleton<ITicketStore, SessionStore>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 6;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789._";
})
    .AddEntityFrameworkStores<ApplicationDBContext>()
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

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

//app.UseRateLimiter();

//app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();
