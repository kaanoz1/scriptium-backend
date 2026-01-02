using ScriptiumBackend.Dto.Islam.Quranic.Root;

namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

using Quranic = Model.Islam.Quranic;

public static class WordExtensions
{
    extension(Quranic.Word word)
    {
        Plain ToPlainDto()
        {
            ArgumentNullException.ThrowIfNull(word.WordC);

            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
            };
        }

        Meaning ToMeaningDto()
        {
            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Meanings = word.Meanings
            };
        }

        Transliteration ToTransliterationDto()
        {
            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Transliterations = word.Transliterations
            };
        }

        Complete ToCompleteDto()
        {
            return new()
            {
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Meanings = word.Meanings,
                Transliterations = word.Transliterations
            };
        }

        Down ToDownDto()
        {
            return new()
            {
                Meanings = word.Meanings,
                Transliterations = word.Transliterations,
                Sequence = word.WordC.SequenceNumber,
                Text = word.WordC.Content,
                Roots = word.WordC.Roots.Select(r => r.ToDownDto()).ToList(),
            };
        }
    }
}