using scriptium_backend_dotnet.Models;

namespace DTO
{

    public abstract class VerseBaseDTO
    {
        public required int Id { get; set; }

        public required short Number { get; set; }

        public required TextVariation Variation { get; set; }


    }

    public abstract class VerseSimpleDTO : VerseBaseDTO
    {

        public required List<TransliterationDTO> Transliterations { get; set; } = [];

        public required List<TranslationTextDTO> TranslationTexts { get; set; } = [];

        public bool IsSaved { get; set; } = false;

    }


    public class VerseDTO : VerseSimpleDTO;

    public class VerseUpperDTO : VerseDTO
    {
        public required ChapterUpperDTO Chapter { get; set; }
    }

    public class VerseOneLevelUpperDTO : VerseDTO
    {
        public required ChapterDTO Chapter { get; set; }
    }

    public class VerseOneLevelLowerDTO : VerseDTO
    {
        public required List<WordDTO> Words { get; set; }
    }

    public class VerseLowerDTO : VerseDTO
    {
        public required List<WordLowerDTO> Words { get; set; }

    }

    public class VerseBothDTO : VerseDTO
    {
        public required ChapterUpperDTO Chapter { get; set; }

        public required List<WordLowerDTO> Words { get; set; }
    }


    public abstract class VerseConfinedDTO : VerseBaseDTO;

    public class VerseUpperConfinedDTO : VerseConfinedDTO
    {
        public required ChapterUpperConfinedDTO Chapter { get; set; }
    }
    public class VerseLowerConfinedDTO : VerseConfinedDTO
    {
        public required List<WordLowerConfinedDTO> Words { get; set; }
    }

    // Custom DTOs

    public class VerseMeanDTO : VerseBaseDTO;

    public class VerseUpperMeanDTO : VerseMeanDTO
    {
        public required ChapterUpperMeanDTO Chapter { get; set; }
    }
    public class VerseLowerMeanDTO : VerseMeanDTO;

    public class TextVariation
    {
        public required string Usual { get; set; }

        public required string? Simplified { get; set; }

        public required string? WithoutVowel { get; set; }
    }


    public static class VerseExtensions
    {
        public static VerseUpperDTO ToVerseUpperDTO(this Verse verse, bool? isSaved = false)
        {
            return new VerseUpperDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDTO()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDTO()).ToList(),
                IsSaved = isSaved ?? false,
                Chapter = verse.Chapter.ToChapterUpperDTO(),

            };
        }

        public static VerseOneLevelUpperDTO ToVerseOneLevelUpperDTO(this Verse verse)
        {
            return new VerseOneLevelUpperDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDTO()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDTO()).ToList(),

                Chapter = verse.Chapter.ToChapterDTO(),

            };
        }

        public static VerseLowerDTO ToVerseLowerDTO(this Verse verse)
        {
            return new VerseLowerDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDTO()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDTO()).ToList(),

                Words = verse.Words.Select(w => w.ToWordLowerDTO()).ToList()
            };
        }

        public static VerseOneLevelLowerDTO ToVerseOneLevelLowerDTO(this Verse verse)
        {
            return new VerseOneLevelLowerDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDTO()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDTO()).ToList(),

                Words = verse.Words.Select(w => w.ToWordDTO()).ToList()

            };
        }

        public static VerseDTO ToVerseDTO(this Verse verse)
        {
            return new VerseDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(t => t.ToTransliterationDTO()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(tt => tt.ToTranslationTextDTO()).ToList()
            };
        }

        public static VerseBothDTO ToVerseBothDTO(this Verse verse)
        {
            return new VerseBothDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDTO()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDTO()).ToList(),

                Chapter = verse.Chapter.ToChapterUpperDTO(),
                Words = verse.Words.Select(w => w.ToWordLowerDTO()).ToList()

            };
        }

        public static VerseLowerConfinedDTO ToVerseLowerConfinedDTO(this Verse verse)
        {
            return new VerseLowerConfinedDTO
            {

                Id = verse.Id,
                Number = verse.Number,
                Words = verse.Words.Select(w => w.ToWordLowerConfinedDTO()).ToList(),
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
            };
        }

        public static VerseUpperConfinedDTO ToVerseUpperConfinedDTO(this Verse verse)
        {
            return new VerseUpperConfinedDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Chapter = verse.Chapter.ToChapterUpperConfinedDTO(),
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },

            };
        }

        public static VerseUpperMeanDTO ToVerseUpperMeanDTO(this Verse verse)
        {
            return new VerseUpperMeanDTO
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Chapter = verse.Chapter.ToChapterUpperMeanDTO(),

            };
        }

    }

}




