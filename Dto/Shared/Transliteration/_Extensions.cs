using ScriptiumBackend.Dto.Shared.Language;

namespace ScriptiumBackend.Dto.Shared.Transliteration;

public static class Extensions
{
    extension(Model.Shared.Transliteration transliteration)
    {
        public Plain ToPlainDto()
        {
            ArgumentNullException.ThrowIfNull(transliteration.Language);

            return new()
            {
                Text = transliteration.Content,
                Language = transliteration.Language.ToPlainDto(),
            };
        }
    }
}