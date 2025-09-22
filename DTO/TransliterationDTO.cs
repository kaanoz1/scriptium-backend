using ScriptiumBackend.Models;

namespace DTO
{
    public class TransliterationDto
    {
        public required string Transliteration { get; set; }
        public required LanguageDto Language { get; set; }
    }

    public static class TransliterationExtensions
    {
        public static TransliterationDto ToTransliterationDto(this Transliteration transliteration)
        {
            return new TransliterationDto
            {
                Transliteration = transliteration.Text,
                Language = transliteration.Language.ToLanguageDto()
            };
        }
    }
}