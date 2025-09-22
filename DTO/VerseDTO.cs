using ScriptiumBackend.Models;

namespace DTO
{

    public abstract class VerseBaseDto
    {
        public required int Id { get; set; }

        public required short Number { get; set; }

        public required TextVariation Variation { get; set; }

    }

    public abstract class VerseSimpleDto : VerseBaseDto
    {

        public required List<TransliterationDto> Transliterations { get; set; } = [];

        public required List<TranslationTextDto> TranslationTexts { get; set; } = [];

        public bool IsSaved { get; set; } = false;

    }


    public class VerseDto : VerseSimpleDto;

    public class VerseUpperDto : VerseDto
    {
        public required ChapterUpperDto Chapter { get; set; }
    }

    public class VerseOneLevelUpperDto : VerseDto
    {
        public required ChapterDto Chapter { get; set; }
    }

    public class VerseOneLevelLowerDto : VerseDto
    {
        public required List<WordDto> Words { get; set; }
    }

    public class VerseLowerDto : VerseDto
    {
        public required List<WordLowerDto> Words { get; set; }

    }

    public class VerseBothDto : VerseDto
    {
        public required ChapterUpperDto Chapter { get; set; }

        public required List<WordLowerDto> Words { get; set; }
    }


    public abstract class VerseConfinedDto : VerseBaseDto;

    public class VerseUpperConfinedDto : VerseConfinedDto
    {
        public required ChapterUpperConfinedDto Chapter { get; set; }
    }
    public class VerseLowerConfinedDto : VerseConfinedDto
    {
        public required List<WordLowerConfinedDto> Words { get; set; }
    }

    // Custom Dtos

    public class VerseMeanDto : VerseBaseDto;

    public class VerseUpperMeanDto : VerseMeanDto
    {
        public required ChapterUpperMeanDto Chapter { get; set; }
    }
    
    
    public class VerseLowerMeanDto : VerseMeanDto;

    public class TextVariation
    {
        public required string Usual { get; set; }

        public required string? Simplified { get; set; }

        public required string? WithoutVowel { get; set; }
    }

    public class VerseUpperMeanStatisticsDto : VerseUpperMeanDto
    {
        public required long Count { get; set; }
        
        public required DateOnly Date { get; set; }
    }

    
    
    public class VerseIndicatorDto : IEquatable<VerseIndicatorDto>
    {
        public int Scripture { get; set; }
        public int Section { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }

        public override bool Equals(object? obj) => Equals(obj as VerseIndicatorDto);

        public bool Equals(VerseIndicatorDto? other)
        {
            return other != null &&
                   Scripture == other.Scripture &&
                   Section == other.Section &&
                   Chapter == other.Chapter &&
                   Verse == other.Verse;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Scripture, Section, Chapter, Verse);
        }

        public override string ToString() => $"{Scripture}:{Section}:{Chapter}:{Verse}";
    }


    public static class VerseExtensions
    {
        public static VerseUpperDto ToVerseUpperDto(this Verse verse, bool? isSaved = false)
        {
            return new VerseUpperDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDto()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDto()).ToList(),
                IsSaved = isSaved ?? false,
                Chapter = verse.Chapter.ToChapterUpperDto(),

            };
        }

        public static VerseOneLevelUpperDto ToVerseOneLevelUpperDto(this Verse verse)
        {
            return new VerseOneLevelUpperDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDto()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDto()).ToList(),

                Chapter = verse.Chapter.ToChapterDto(),

            };
        }

        public static VerseLowerDto ToVerseLowerDto(this Verse verse)
        {
            return new VerseLowerDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDto()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDto()).ToList(),

                Words = verse.Words.Select(w => w.ToWordLowerDto()).ToList()
            };
        }

        public static VerseOneLevelLowerDto ToVerseOneLevelLowerDto(this Verse verse)
        {
            return new VerseOneLevelLowerDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDto()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDto()).ToList(),

                Words = verse.Words.Select(w => w.ToWordDto()).ToList()

            };
        }

        public static VerseDto ToVerseDto(this Verse verse)
        {
            return new VerseDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(t => t.ToTransliterationDto()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(tt => tt.ToTranslationTextDto()).ToList()
            };
        }

        public static VerseBothDto ToVerseBothDto(this Verse verse)
        {
            return new VerseBothDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Transliterations = verse.Transliterations.Select(e => e.ToTransliterationDto()).ToList(),
                TranslationTexts = verse.TranslationTexts.Select(e => e.ToTranslationTextDto()).ToList(),

                Chapter = verse.Chapter.ToChapterUpperDto(),
                Words = verse.Words.Select(w => w.ToWordLowerDto()).ToList()

            };
        }

        public static VerseLowerConfinedDto ToVerseLowerConfinedDto(this Verse verse)
        {
            return new VerseLowerConfinedDto
            {

                Id = verse.Id,
                Number = verse.Number,
                Words = verse.Words.Select(w => w.ToWordLowerConfinedDto()).ToList(),
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
            };
        }

        public static VerseUpperConfinedDto ToVerseUpperConfinedDto(this Verse verse)
        {
            return new VerseUpperConfinedDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Chapter = verse.Chapter.ToChapterUpperConfinedDto(),
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },

            };
        }

        public static VerseUpperMeanDto ToVerseUpperMeanDto(this Verse verse)
        {
            return new VerseUpperMeanDto
            {
                Id = verse.Id,
                Number = verse.Number,
                Variation = new TextVariation { Usual = verse.Text, Simplified = verse.TextSimplified, WithoutVowel = verse.TextWithoutVowel },
                Chapter = verse.Chapter.ToChapterUpperMeanDto(),

            };
        }

    }

}




