using ScriptiumBackend.Model;

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