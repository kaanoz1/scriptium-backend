using ScriptiumBackend.Dto.Sealed.Author;
using ScriptiumBackend.Dto.Sealed.Language;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Translation;

public static class Extensions
{
    extension(Model.Derived.Islam.Quranic.Translation translation)
    {
        public Plain ToPlain()
        {
            ArgumentNullException.ThrowIfNull(translation.Language);

            return new()
            {
                Name = translation.Name,
                Description = translation.Description,
                Language = translation.Language.ToPlainDto(),
            };
        }

        public Complete ToComplete()
        {
            ArgumentNullException.ThrowIfNull(translation.Language);
            ArgumentNullException.ThrowIfNull(translation.Authors);

            return new()
            {
                Name = translation.Name,
                Description = translation.Description,
                Language = translation.Language.ToPlainDto(),
                Authors = translation.Authors.Select(a => a.ToCompleteDto()).ToList(),
            };
        }
    }
}