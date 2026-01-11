using ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public static class VerseExtensions
{
    extension(Model.Derived.Islam.Quranic.Verse verse)
    {
        public Plain ToPlainVerseDto()
        {
            return new Plain()
            {
                Sequence = verse.Number,
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
            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),


                Sequence = verse.Number,
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
            ArgumentNullException.ThrowIfNull(verse.Words);


            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),


                Sequence = verse.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
                Word = verse.Words.Select(word => word.ToDownDto()).ToList()
            };
        }

        public UpToQuran UpToQuranDto()
        {
            ArgumentNullException.ThrowIfNull(verse.Chapter);

            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),


                Sequence = verse.Number,
                Chapter = verse.Chapter.ToUpToQuranDto(),
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
            };
        }

        public Both ToBothDto()
        {
            ArgumentNullException.ThrowIfNull(verse.Chapter);
            ArgumentNullException.ThrowIfNull(verse.Words);


            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),


                Chapter = verse.Chapter.ToUpToQuranDto(),
                Words = verse.Words.Select(word => word.ToDownDto()).ToList(),

                Sequence = verse.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
            };
        }
    }
}