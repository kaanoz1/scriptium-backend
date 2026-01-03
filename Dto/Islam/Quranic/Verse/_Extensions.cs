using ScriptiumBackend.Dto.Islam.Quranic.Word;

namespace ScriptiumBackend.Dto.Islam.Quranic.Verse;

using Model.Islam.Quranic;

public static class VerseExtensions
{
    extension(Verse verse)
    {
        public Plain ToPlainVerseDto()
        {
            ArgumentNullException.ThrowIfNull(verse.VerseC);

            return new Plain()
            {
                Sequence = verse.VerseC.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
            };
        }

        public Complete ToCompleteDto()
        {
            ArgumentNullException.ThrowIfNull(verse.VerseC);

            return new()
            {
                Sequence = verse.VerseC.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
            };
        }

        public Down ToDownDto()
        {
            ArgumentNullException.ThrowIfNull(verse.VerseC);
            ArgumentNullException.ThrowIfNull(verse.Words);

            if (verse.Words.First() is { } w)
            {
                ArgumentNullException.ThrowIfNull(w.WordC);
                ArgumentNullException.ThrowIfNull(w.Meanings);
                ArgumentNullException.ThrowIfNull(w.Transliterations);
            }


            return new()
            {
                Sequence = verse.VerseC.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
                Word = verse.Words.Select(w => w.ToDownDto()).ToList()
            };
        }
    }
}