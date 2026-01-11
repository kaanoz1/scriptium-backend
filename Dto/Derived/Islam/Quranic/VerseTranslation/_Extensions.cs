using ScriptiumBackend.Dto.Sealed.Author;
using ScriptiumBackend.Dto.Sealed.Footnote;
using ScriptiumBackend.Dto.Sealed.Language;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation;

using Quranic = Model.Derived.Islam.Quranic;

public static class Extensions
{
    extension(Quranic.VerseTranslation verseTranslation)
    {
        public Plain ToPlainDto()
        {
            return new()
            {
                Text = verseTranslation.Text,
            };
        }


        public Complete ToCompleteDto()
        {
            return new()
            {
                Text = verseTranslation.Text,
                Authors = verseTranslation.Translation.Authors
                    .Select(a => a.ToCompleteDto()).ToList(),
                Footnotes = verseTranslation.Footnotes.Select(f => f.ToPlainDto())
                    .ToList(),
                Language = verseTranslation.Translation.Language.ToPlainDto(),
            };
        }
    }
}