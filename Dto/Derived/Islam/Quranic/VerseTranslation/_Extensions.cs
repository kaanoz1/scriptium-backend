using System;
using System.Linq;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Translation;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;
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
            ArgumentNullException.ThrowIfNull(verseTranslation.Translation);
            ArgumentNullException.ThrowIfNull(verseTranslation.Translation.Authors);
            ArgumentNullException.ThrowIfNull(verseTranslation.Footnotes);
            ArgumentNullException.ThrowIfNull(verseTranslation.Translation.Language);

            return new()
            {
                Text = verseTranslation.Text,
                Footnotes = verseTranslation.Footnotes.Select(f => f.ToPlainDto())
                    .ToList(),
                Translation = verseTranslation.Translation.ToComplete(),
            };
        }

        public WithVerse ToWithVerse()
        {
            ArgumentNullException.ThrowIfNull(verseTranslation.Translation);
            ArgumentNullException.ThrowIfNull(verseTranslation.Footnotes);
            ArgumentNullException.ThrowIfNull(verseTranslation.Verse);

            return new()
            {
                Text = verseTranslation.Text,
                Footnotes = verseTranslation.Footnotes.Select(f => f.ToPlainDto()).ToList(),
                Verse = verseTranslation.Verse.ToTransliterationUpToQuran(),
                Translation = verseTranslation.Translation.ToComplete(),
            };
        }
    }
}