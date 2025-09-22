using ScriptiumBackend.Models;

namespace DTO
{
    public class TranslationDto
    {
        public required short Id { get; set; }
        public required string Name { get; set; }
        public required LanguageDto Language { get; set; }
        public required List<TranslatorDto> Translators { get; set; }
        public required bool IsEager { get; set; }
    }

    public class TranslationWithScriptureDtoDto : TranslationDto
    {
        public required ScriptureDto Scripture { get; set; }
    }

    public static class TranslationExtensions
    {
        public static TranslationWithScriptureDtoDto ToTranslationWithScriptureDtoDto(this Translation translation)
        {
            return new TranslationWithScriptureDtoDto
            {
                Id = translation.Id,
                Name = translation.Name,
                Language = translation.Language.ToLanguageDto(),
                Translators = translation.TranslatorTranslations.Select(e => e.Translator.ToTranslatorDto()).ToList(),
                IsEager = translation.EagerFrom.HasValue,
                Scripture = translation.Scripture.ToScriptureDto()
            };
        }
        public static TranslationDto ToTranslationDto(this Translation translation)
        {
            return new TranslationDto
            {
                Id = translation.Id,
                Name = translation.Name,
                Language = translation.Language.ToLanguageDto(),
                Translators = translation.TranslatorTranslations.Select(e => e.Translator.ToTranslatorDto()).ToList(),
                IsEager = translation.EagerFrom.HasValue
            };
        }
    }
}