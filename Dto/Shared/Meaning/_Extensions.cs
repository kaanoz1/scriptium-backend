using ScriptiumBackend.Dto.Shared.Language;

namespace ScriptiumBackend.Dto.Shared.Meaning;

public static class Extensions
{
    extension(Model.Shared.Meaning meaning)
    {
        Plain ToPlainDto()
        {
            ArgumentNullException.ThrowIfNull(meaning.Language);

            return new()
            {
                Text = meaning.Content,
                Language = meaning.Language.ToPlainDto()
            };
        }
    }
}