using ScriptiumBackend.Models;

namespace DTO
{
    public abstract class ChapterBaseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required short Number { get; set; }
    }

    public abstract class ChapterSimpleDto : ChapterBaseDto
    {
        public required List<ChapterMeaningDto> Meanings { get; set; } = [];
    }

    public class ChapterDto : ChapterSimpleDto;

    public class ChapterUpperDto : ChapterDto
    {
        public required SectionUpperDto Section { get; set; }
    }

    public class ChapterOneLevelUpperDto : ChapterDto
    {
        public required SectionDto Section { get; set; }
    }

    public class ChapterLowerDto : ChapterDto
    {
        public required List<VerseLowerDto> Verses { get; set; }
    }

    public class ChapterOneLevelLowerDto : ChapterDto
    {
        public required List<VerseDto> Verses { get; set; }
    }

    public class ChapterUpperAndOneLevelLowerDto : ChapterUpperDto
    {
        public required List<VerseDto> Verses { get; set; }
    }

    public class ChapterBothDto : ChapterDto
    {
        public required SectionUpperDto Section { get; set; }
        public required List<VerseLowerDto> Verses { get; set; }
    }

    public class ChapterMeaningDto : Meaning;

    public abstract class ChapterConfinedDto : ChapterBaseDto;

    public class ChapterUpperConfinedDto : ChapterConfinedDto
    {
        public required SectionUpperConfinedDto Section { get; set; }
    }

    public class ChapterLowerConfinedDto : ChapterConfinedDto
    {
        public required List<VerseLowerConfinedDto> Verses { get; set; }
    }

    //Custom Dtos
    public class ChapterMeanDto : ChapterBaseDto
    {
        public required List<ChapterMeaningDto> Meanings { get; set; } = [];
    }

    public class ChapterUpperMeanDto : ChapterMeanDto
    {
        public required SectionUpperMeanDto Section { get; set; }
    }

    public class ChapterLowerMeanDto : ChapterMeanDto
    {
        public required List<VerseLowerMeanDto> Verses { get; set; }
    }

    
    public class ChapterIndicatorDto : IEquatable<ChapterIndicatorDto>
    {
        public required int Scripture { get; set; }
        public required int Section { get; set; }
        public required int Chapter { get; set; }

        public override bool Equals(object? obj) => Equals(obj as ChapterIndicatorDto);

        public bool Equals(ChapterIndicatorDto? other)
        {
            if (other is null) return false;
            return Scripture == other.Scripture &&
                   Section == other.Section &&
                   Chapter == other.Chapter;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Scripture, Section, Chapter);
        }
    }
    
    public static class ChapterExtensions
    {
        public static ChapterUpperAndOneLevelLowerDto ToChapterUpperAndOneLevelLowerDto(this Chapter chapter)
        {
            return new ChapterUpperAndOneLevelLowerDto
            {
                Id = chapter.Id,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),
                Name = chapter.Name,
                Number = chapter.Number,
                Section = chapter.Section.ToSectionUpperDto(),
                Verses = chapter.Verses.Select(v => v.ToVerseDto()).ToList()
            };
        }

        public static ChapterDto ToChapterDto(this Chapter chapter)
        {
            return new ChapterDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),
            };
        }

        public static ChapterUpperDto ToChapterUpperDto(this Chapter chapter)
        {
            return new ChapterUpperDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),
                Section = chapter.Section.ToSectionUpperDto(),
            };
        }

        public static ChapterOneLevelUpperDto ToChapterOneLevelUpperDto(this Chapter chapter)
        {
            return new ChapterOneLevelUpperDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),
                Section = chapter.Section.ToSectionDto(),
            };
        }

        public static ChapterLowerDto ToChapterLowerDto(this Chapter chapter)
        {
            return new ChapterLowerDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),
                Verses = chapter.Verses.Select(v => v.ToVerseLowerDto()).ToList(),
            };
        }

        public static ChapterOneLevelLowerDto ToChapterOneLevelLowerDto(this Chapter chapter)
        {
            return new ChapterOneLevelLowerDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),
                Verses = chapter.Verses.Select(v => v.ToVerseDto()).ToList(),
            };
        }

        public static ChapterBothDto ToChapterBothBaseDto(this Chapter chapter)
        {
            return new ChapterBothDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),

                Verses = chapter.Verses.Select(v => v.ToVerseLowerDto()).ToList(),
                Section = chapter.Section.ToSectionUpperDto(),
            };
        }

        public static ChapterUpperConfinedDto ToChapterUpperConfinedDto(this Chapter chapter)
        {
            return new ChapterUpperConfinedDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Section = chapter.Section.ToSectionUpperConfinedDto(),
            };
        }

        public static ChapterLowerConfinedDto ToChapterLowerConfinedDto(this Chapter chapter)
        {
            return new ChapterLowerConfinedDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Verses = chapter.Verses.Select(v => v.ToVerseLowerConfinedDto()).ToList()
            };
        }

        public static ChapterMeaningDto ToChapterMeaningDto(this ChapterMeaning chapterMeaning)
        {
            return new ChapterMeaningDto
            {
                Text = chapterMeaning.Meaning,
                Language = chapterMeaning.Language.ToLanguageDto()
            };
        }

        public static ChapterUpperMeanDto ToChapterUpperMeanDto(this Chapter chapter)
        {
            return new ChapterUpperMeanDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDto()).ToList(),

                Section = chapter.Section.ToSectionUpperMeanDto(),
            };
        }
    }
}