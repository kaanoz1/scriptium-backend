using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class TranslatorDTO
    {
        public required string Name { get; set; }
        public string? URL { get; set; }
        public required LanguageDTO Language { get; set; }
    }

    public static class TranslatorExtensions
    {
        public static TranslatorDTO ToTranslatorDTO(this Translator translator)
        {
            return new TranslatorDTO
            {
                Name = translator.Name,
                URL = translator.Url,
                Language = translator.Language.ToLanguageDTO()
            };
        }
    }
}