using ScriptiumBackend.Model;
using Serilog.Sinks.SystemConsole.Themes;

namespace ScriptiumBackend.Utils.Constants;

public static class Islam
{
    public static class Quran
    {
        public static readonly char Code = 'Q';
    }
}

public static class Default
{
    

    
    public static class Language
    {
        public static readonly List<ScriptiumBackend.Model.Shared.Language> InitialLanguages =
        [
            new()
            {
                Id = 1,
                Code = "en",
                Name = "English",
                EnglishName = "English"
            }
        ];
    }
}

public static class Configurations
{
    public static class Logger
    {
        public static readonly AnsiConsoleTheme Theme = new (new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[37m",             // Standart metin (Beyaz)
            [ConsoleThemeStyle.SecondaryText] = "\x1b[37m",    // İkincil metin
            [ConsoleThemeStyle.TertiaryText] = "\x1b[37m",     // Üçüncül metin
            [ConsoleThemeStyle.Invalid] = "\x1b[33m",          // Geçersiz
            [ConsoleThemeStyle.Null] = "\x1b[34m",             // Null (Mavi)
            [ConsoleThemeStyle.Name] = "\x1b[37m",             // Property isimleri
            [ConsoleThemeStyle.String] = "\x1b[36m",           // String değerler (Cyan)
            [ConsoleThemeStyle.Number] = "\x1b[35m",           // Sayılar (Magenta)
            [ConsoleThemeStyle.Boolean] = "\x1b[34m",          // Boolean (Mavi)
            [ConsoleThemeStyle.Scalar] = "\x1b[32m",           // Diğer objeler (Yeşil)
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m",     // Verbose
            [ConsoleThemeStyle.LevelDebug] = "\x1b[37m",       // Debug
    
            // SENİN İSTEDİĞİN RENKLER BURADA:
            [ConsoleThemeStyle.LevelInformation] = "\x1b[32m", // Info -> Yeşil
            [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",     // Warning -> Sarı
            [ConsoleThemeStyle.LevelError] = "\x1b[31m",       // Error -> Kırmızı
            [ConsoleThemeStyle.LevelFatal] = "\x1b[35m",       // Fatal -> Koyu Mor (Magenta)
        });
    }
}