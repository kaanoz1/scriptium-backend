using System;
using ScriptiumBackend.Dto.Sealed.Language;

namespace ScriptiumBackend.Dto.Sealed.Meaning;

public static class Extensions
{
    extension(Model.Sealed.Meaning meaning)
    {
        public Plain ToPlainDto()
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