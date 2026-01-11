
using ScriptiumBackend.Dto.Sealed.Language;
using ScriptiumBackend.Dto.Sealed.Meaning;

namespace ScriptiumBackend.Dto.Sealed.Author;

public static class Extensions
{
    extension(Model.Sealed.Author author)
    {
        public Plain ToPlainDto()
        {
            return new()
            {
                Name = author.Name,
                Url = author.Url,
                Description = author.Description,
            };
        }

        public Complete ToCompleteDto()
        {
            return new()
            {
                Name = author.Name,
                Url = author.Url,
                Description = author.Description,
                Language = author.Language.ToPlainDto(),
                NameTranslations = author.NameTranslations.Select(t => t.ToPlainDto()).ToList(),
            };
        }
    }
}