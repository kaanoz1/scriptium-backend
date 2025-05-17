using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class TransliterationDTO
    {
        public required string Transliteration { get; set; }
        public required LanguageDTO Language { get; set; }
    }

    public static class TransliterationExtensions
    {
        public static TransliterationDTO ToTransliterationDTO(this Transliteration transliteration)
        {
            return new TransliterationDTO
            {
                Transliteration = transliteration.Text,
                Language = transliteration.Language.ToLanguageDTO()
            };
        }
    }
}