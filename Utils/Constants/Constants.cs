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
        public static readonly Model.Sealed.Language English = new()
        {
            Id = 1,
            Code = "en",
            Name = "English",
            EnglishName = "English"
        };


        public static readonly List<Model.Sealed.Language> InitialLanguages =
        [
            English
        ];
    }
}

public static class Configurations
{
    public static class Logger
    {
        public static readonly AnsiConsoleTheme Theme = new(new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[37m",
            [ConsoleThemeStyle.SecondaryText] = "\x1b[37m",
            [ConsoleThemeStyle.TertiaryText] = "\x1b[37m",
            [ConsoleThemeStyle.Invalid] = "\x1b[33m",
            [ConsoleThemeStyle.Null] = "\x1b[34m",
            [ConsoleThemeStyle.Name] = "\x1b[37m",
            [ConsoleThemeStyle.String] = "\x1b[36m",
            [ConsoleThemeStyle.Number] = "\x1b[35m",
            [ConsoleThemeStyle.Boolean] = "\x1b[34m",
            [ConsoleThemeStyle.Scalar] = "\x1b[32m",
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m",
            [ConsoleThemeStyle.LevelDebug] = "\x1b[37m",

            [ConsoleThemeStyle.LevelInformation] = "\x1b[32m",
            [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",
            [ConsoleThemeStyle.LevelError] = "\x1b[31m",
            [ConsoleThemeStyle.LevelFatal] = "\x1b[35m",
        });
    }
}