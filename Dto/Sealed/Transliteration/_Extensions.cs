using System;
using ScriptiumBackend.Dto.Sealed.Language;

namespace ScriptiumBackend.Dto.Sealed.Transliteration;

public static class Extensions
{
    extension(Model.Sealed.Transliteration transliteration)
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