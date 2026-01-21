using System;
using System.Linq;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;
using ScriptiumBackend.Dto.Sealed.Transliteration;

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
            ArgumentNullException.ThrowIfNull(verse.Translations);
            ArgumentNullException.ThrowIfNull(verse.Transliterations);

            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),
                Transliterations = verse.Transliterations.Select(t => t.ToPlainDto()).ToList(),


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
            ArgumentNullException.ThrowIfNull(verse.Transliterations);


            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),
                Transliterations = verse.Transliterations.Select(t => t.ToPlainDto()).ToList(),


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
            ArgumentNullException.ThrowIfNull(verse.Transliterations);

            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),
                Transliterations = verse.Transliterations.Select(t => t.ToPlainDto()).ToList(),


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
            ArgumentNullException.ThrowIfNull(verse.Translations);
            ArgumentNullException.ThrowIfNull(verse.Transliterations);


            return new()
            {
                Translations = verse.Translations.Select(t => t.ToCompleteDto()).ToList(),
                Chapter = verse.Chapter.ToUpToQuranDto(),
                Words = verse.Words.Select(word => word.ToDownDto()).ToList(),
                Transliterations = verse.Transliterations.Select(t => t.ToPlainDto()).ToList(),

                Sequence = verse.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
            };
        }

        public PlainWithPlainChapter ToPlainWithPlainChapterDto()
        {
            ArgumentNullException.ThrowIfNull(verse.Chapter);

            return new()
            {
                Sequence = verse.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,

                Chapter = verse.Chapter.ToPlainDto(),
            };
        }

        public PlainUpToQuran ToPlainUpToQuran()
        {
            ArgumentNullException.ThrowIfNull(verse.Chapter);

            return new PlainUpToQuran()
            {
                Sequence = verse.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,

                Chapter = verse.Chapter.ToUpToQuranDto(),
            };
        }

        public TransliterationUpToQuran ToTransliterationUpToQuran()
        {
            ArgumentNullException.ThrowIfNull(verse.Chapter);
            ArgumentNullException.ThrowIfNull(verse.Transliterations);

            
            return new TransliterationUpToQuran()
            {
                Sequence = verse.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,

                Chapter = verse.Chapter.ToUpToQuranDto(),
                Transliterations = verse.Transliterations.Select(t => t.ToPlainDto()).ToList(),
            };
        }
    }
}