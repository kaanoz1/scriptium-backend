using scriptium_backend_dotnet.Models;

namespace DTO
{
    public abstract class ChapterBaseDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required short Number { get; set; }
    }

    public abstract class ChapterSimpleDTO : ChapterBaseDTO
    {
        public required List<ChapterMeaningDTO> Meanings { get; set; } = [];
    }

    public class ChapterDTO : ChapterSimpleDTO;

    public class ChapterUpperDTO : ChapterDTO
    {
        public required SectionUpperDTO Section { get; set; }
    }

    public class ChapterOneLevelUpperDTO : ChapterDTO
    {
        public required SectionDTO Section { get; set; }
    }

    public class ChapterLowerDTO : ChapterDTO
    {
        public required List<VerseLowerDTO> Verses { get; set; }
    }

    public class ChapterOneLevelLowerDTO : ChapterDTO
    {
        public required List<VerseDTO> Verses { get; set; }
    }

    public class ChapterUpperAndOneLevelLowerDTO : ChapterUpperDTO
    {
        public required List<VerseDTO> Verses { get; set; }
    }

    public class ChapterBothDTO : ChapterDTO
    {
        public required SectionUpperDTO Section { get; set; }
        public required List<VerseLowerDTO> Verses { get; set; }
    }

    public class ChapterMeaningDTO : Meaning;

    public abstract class ChapterConfinedDTO : ChapterBaseDTO;

    public class ChapterUpperConfinedDTO : ChapterConfinedDTO
    {
        public required SectionUpperConfinedDTO Section { get; set; }
    }

    public class ChapterLowerConfinedDTO : ChapterConfinedDTO
    {
        public required List<VerseLowerConfinedDTO> Verses { get; set; }
    }

    //Custom DTOs
    public class ChapterMeanDTO : ChapterBaseDTO
    {
        public required List<ChapterMeaningDTO> Meanings { get; set; } = [];
    }

    public class ChapterUpperMeanDTO : ChapterMeanDTO
    {
        public required SectionUpperMeanDTO Section { get; set; }
    }

    public class ChapterLowerMeanDTO : ChapterMeanDTO
    {
        public required List<VerseLowerMeanDTO> Verses { get; set; }
    }

    
    public class ChapterIndicatorDTO : IEquatable<ChapterIndicatorDTO>
    {
        public required int Scripture { get; set; }
        public required int Section { get; set; }
        public required int Chapter { get; set; }

        public override bool Equals(object? obj) => Equals(obj as ChapterIndicatorDTO);

        public bool Equals(ChapterIndicatorDTO? other)
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
        public static ChapterUpperAndOneLevelLowerDTO ToChapterUpperAndOneLevelLowerDTO(this Chapter chapter)
        {
            return new ChapterUpperAndOneLevelLowerDTO
            {
                Id = chapter.Id,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),
                Name = chapter.Name,
                Number = chapter.Number,
                Section = chapter.Section.ToSectionUpperDTO(),
                Verses = chapter.Verses.Select(v => v.ToVerseDTO()).ToList()
            };
        }

        public static ChapterDTO ToChapterDTO(this Chapter chapter)
        {
            return new ChapterDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),
            };
        }

        public static ChapterUpperDTO ToChapterUpperDTO(this Chapter chapter)
        {
            return new ChapterUpperDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),
                Section = chapter.Section.ToSectionUpperDTO(),
            };
        }

        public static ChapterOneLevelUpperDTO ToChapterOneLevelUpperDTO(this Chapter chapter)
        {
            return new ChapterOneLevelUpperDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),
                Section = chapter.Section.ToSectionDTO(),
            };
        }

        public static ChapterLowerDTO ToChapterLowerDTO(this Chapter chapter)
        {
            return new ChapterLowerDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),
                Verses = chapter.Verses.Select(v => v.ToVerseLowerDTO()).ToList(),
            };
        }

        public static ChapterOneLevelLowerDTO ToChapterOneLevelLowerDTO(this Chapter chapter)
        {
            return new ChapterOneLevelLowerDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),
                Verses = chapter.Verses.Select(v => v.ToVerseDTO()).ToList(),
            };
        }

        public static ChapterBothDTO ToChapterBothBaseDTO(this Chapter chapter)
        {
            return new ChapterBothDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),

                Verses = chapter.Verses.Select(v => v.ToVerseLowerDTO()).ToList(),
                Section = chapter.Section.ToSectionUpperDTO(),
            };
        }

        public static ChapterUpperConfinedDTO ToChapterUpperConfinedDTO(this Chapter chapter)
        {
            return new ChapterUpperConfinedDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Section = chapter.Section.ToSectionUpperConfinedDTO(),
            };
        }

        public static ChapterLowerConfinedDTO ToChapterLowerConfinedDTO(this Chapter chapter)
        {
            return new ChapterLowerConfinedDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Verses = chapter.Verses.Select(v => v.ToVerseLowerConfinedDTO()).ToList()
            };
        }

        public static ChapterMeaningDTO ToChapterMeaningDTO(this ChapterMeaning chapterMeaning)
        {
            return new ChapterMeaningDTO
            {
                Text = chapterMeaning.Meaning,
                Language = chapterMeaning.Language.ToLanguageDTO()
            };
        }

        public static ChapterUpperMeanDTO ToChapterUpperMeanDTO(this Chapter chapter)
        {
            return new ChapterUpperMeanDTO
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Number = chapter.Number,
                Meanings = chapter.Meanings.Select(cm => cm.ToChapterMeaningDTO()).ToList(),

                Section = chapter.Section.ToSectionUpperMeanDTO(),
            };
        }
    }
}