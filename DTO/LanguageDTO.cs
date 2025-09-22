using ScriptiumBackend.Models;

namespace DTO
{
    public class LanguageDto
    {
        public string LangCode { get; set; } = string.Empty;
        public string LangOwn { get; set; } = string.Empty;
        public string LangEnglish { get; set; } = string.Empty;
    }

    public abstract class Meaning
    {
        public required string Text { get; set; }
        public required LanguageDto Language { get; set; }
    }

    public static class LanguageExtensions
    {
        public static LanguageDto ToLanguageDto(this Language language)
        {
            return new LanguageDto
            {
                LangCode = language.LangCode,
                LangOwn = language.LangOwn,
                LangEnglish = language.LangEnglish
            };
        }
    }
}
