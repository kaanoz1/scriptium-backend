using ScriptiumBackend.Dto.Derived.Islam.Quranic.Root;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;
using ScriptiumBackend.Dto.Sealed.Meaning;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

using Quranic = Model.Derived.Islam.Quranic;

public static class WordExtensions
{
    extension(Quranic.Word word)
    {
        public Plain ToPlainDto()
        {
            return new()
            {
                Sequence = word.Sequence,
                Text = word.Text,
            };
        }

        public Meaning ToMeaningDto()
        {
            ArgumentNullException.ThrowIfNull(word.Meanings);


            return new()
            {
                Sequence = word.Sequence,
                Text = word.Text,
                Meanings = word.Meanings
            };
        }

        public Transliteration ToTransliterationDto()
        {
            ArgumentNullException.ThrowIfNull(word.Transliterations);


            return new()
            {
                Sequence = word.Sequence,
                Text = word.Text,
                Transliterations = word.Transliterations
            };
        }

        public Complete ToCompleteDto()
        {
            ArgumentNullException.ThrowIfNull(word.Meanings);
            ArgumentNullException.ThrowIfNull(word.Transliterations);

            return new()
            {
                Sequence = word.Sequence,
                Text = word.Text,
                Meanings = word.Meanings.Select(m => m.ToPlainDto()).ToList(),
                Transliterations = word.Transliterations
            };
        }

        public Down ToDownDto()
        {
            ArgumentNullException.ThrowIfNull(word.Roots);
            ArgumentNullException.ThrowIfNull(word.Meanings);
            ArgumentNullException.ThrowIfNull(word.Transliterations);

            return new()
            {
                Meanings = word.Meanings.Select(m => m.ToPlainDto()).ToList(),
                Transliterations = word.Transliterations,
                Sequence = word.Sequence,
                Text = word.Text,
                Roots = word.Roots.Select(r => r.ToDownDto()).ToList(),
            };
        }

        public UpToVerse ToUpToVerseDto()
        {
            ArgumentNullException.ThrowIfNull(word.Meanings);
            ArgumentNullException.ThrowIfNull(word.Transliterations);
            ArgumentNullException.ThrowIfNull(word.Verse);

            return new()
            {
                Sequence = word.Sequence,
                Text = word.Text,
                Meanings = word.Meanings.Select(m => m.ToPlainDto()).ToList(),
                Transliterations = word.Transliterations,
                Verse = word.Verse.ToCompleteDto()
            };
        }
    }
}