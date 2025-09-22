using ScriptiumBackend.Models;

namespace DTO
{
    public class TranslatorDto
    {
        public required string Name { get; set; }
        public string? URL { get; set; }
        public required LanguageDto Language { get; set; }
    }

    public static class TranslatorExtensions
    {
        public static TranslatorDto ToTranslatorDto(this Translator translator)
        {
            return new TranslatorDto
            {
                Name = translator.Name,
                URL = translator.Url,
                Language = translator.Language.ToLanguageDto()
            };
        }
    }
}