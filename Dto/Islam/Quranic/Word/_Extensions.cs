using ScriptiumBackend.Dto.Islam.Quranic.Root;
using ScriptiumBackend.Dto.Shared.Meaning;

namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

using Quranic = Model.Islam.Quranic;

public static class WordExtensions
{
    extension(Quranic.Word word)
    {
        public Plain ToPlainDto()
        {
            ArgumentNullException.ThrowIfNull(word.WordC);

            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
            };
        }

        public Meaning ToMeaningDto()
        {
            ArgumentNullException.ThrowIfNull(word.WordC);
            ArgumentNullException.ThrowIfNull(word.Meanings);


            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Meanings = word.Meanings
            };
        }

        public Transliteration ToTransliterationDto()
        {
            ArgumentNullException.ThrowIfNull(word.WordC);
            ArgumentNullException.ThrowIfNull(word.Transliterations);


            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Transliterations = word.Transliterations
            };
        }

        public Complete ToCompleteDto()
        {
            ArgumentNullException.ThrowIfNull(word.WordC);
            ArgumentNullException.ThrowIfNull(word.Meanings);
            ArgumentNullException.ThrowIfNull(word.Transliterations);

            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Meanings = word.Meanings.Select(m => m.ToPlainDto()).ToList(),
                Transliterations = word.Transliterations
            };
        }

        public Down ToDownDto()
        {
            ArgumentNullException.ThrowIfNull(word.WordC);
            ArgumentNullException.ThrowIfNull(word.WordC.Roots);
            ArgumentNullException.ThrowIfNull(word.Meanings);
            ArgumentNullException.ThrowIfNull(word.Transliterations);

            return new()
            {
                Meanings = word.Meanings.Select(m => m.ToPlainDto()).ToList(),
                Transliterations = word.Transliterations,
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Roots = word.WordC.Roots.Select(r => r.ToDownDto()).ToList(),
            };
        }
    }
}