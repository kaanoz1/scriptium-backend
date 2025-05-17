using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class LanguageDTO
    {
        public string LangCode { get; set; } = string.Empty;
        public string LangOwn { get; set; } = string.Empty;
        public string LangEnglish { get; set; } = string.Empty;
    }

    public abstract class Meaning
    {
        public required string Text { get; set; }
        public required LanguageDTO Language { get; set; }
    }

    public static class LanguageExtensions
    {
        public static LanguageDTO ToLanguageDTO(this Language language)
        {
            return new LanguageDTO
            {
                LangCode = language.LangCode,
                LangOwn = language.LangOwn,
                LangEnglish = language.LangEnglish
            };
        }
    }
}
