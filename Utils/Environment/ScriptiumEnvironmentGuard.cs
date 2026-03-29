
namespace ScriptiumBackend.Utils.Environment;

public static class ScriptiumEnvironmentGuard
{
    private static readonly ILogger Logger;
    private static IConfiguration? _configuration;

    static ScriptiumEnvironmentGuard()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = loggerFactory.CreateLogger("ScriptiumEnvironmentGuard");
    }

    private static class Keys
    {
        public const string ConnectionString = "ConnectionStrings:DefaultConnection";
        public const string OllamaBaseUrl = "OLLAMA_BASE_URL";
        public const string AllowedOrigins = "CorsSettings:AllowedOrigins";
    }

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static void Validate()
    {
        if (_configuration == null)
        {
            Logger.LogCritical("ERROR: ScriptiumEnvironmentGuard.Initialize(configuration) must be called before Validate().");
            throw new InvalidOperationException("Guard not initialized.");
        }

        var requiredKeys = new[]
        {
            Keys.ConnectionString,
            Keys.OllamaBaseUrl,
            Keys.AllowedOrigins
        };

        var hasError = false;
        foreach (var key in requiredKeys)
        {
            if (!string.IsNullOrWhiteSpace(_configuration[key])) continue;

            Logger.LogCritical("ERROR: Configuration key '{Key}' is missing or empty!", key);
            hasError = true;
        }

        if (hasError)
        {
            Logger.LogCritical("Application termination: Missing required configuration.");
            System.Environment.Exit(1);
        }

        try
        {
            _ = GetAllowedOrigins();
            Logger.LogInformation("Configuration validation successful.");
        }
        catch (Exception ex)
        {
            Logger.LogCritical("ERROR: {Message}", ex.Message);
            System.Environment.Exit(1);
        }
    }

    public static string GetConnectionString() => GetValue(Keys.ConnectionString);
    
    public static string GetOllamaBaseUrl() => GetValue(Keys.OllamaBaseUrl);

    public static string[] GetAllowedOrigins()
    {
        var rawValue = GetValue(Keys.AllowedOrigins);

        var origins = rawValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        return origins.Length == 0 ? throw new InvalidOperationException($"Configuration '{Keys.AllowedOrigins}' must contain at least one valid origin.") : origins;
    }

    private static string GetValue(string key)
    {
        if (_configuration == null)
            throw new InvalidOperationException("ScriptiumEnvironmentGuard not initialized. Call Initialize(configuration) first.");

        return _configuration[key] 
               ?? throw new InvalidOperationException($"Configuration '{key}' is not set in any provider.");
    }
}