using ScriptiumBackend.Models;

namespace DTO
{
    public abstract class SectionBaseDto
    {
        public short Id { get; set; }

        public required string Name { get; set; }

        public required short Number { get; set; }
    }

    public abstract class SectionSimpleDto : SectionBaseDto
    {
        public required List<SectionMeaningDto> Meanings { get; set; } = [];
    }

    public class SectionDto : SectionSimpleDto;


    public class SectionUpperDto : SectionDto
    {
        public required ScriptureDto Scripture { get; set; }
    }


    public class SectionLowerDto : SectionDto
    {
        public required List<ChapterLowerDto> Chapters { get; set; }
    }

    public class SectionOneLevelLowerDto : SectionDto
    {
        public required List<ChapterDto> Chapters { get; set; }
    }

    public class SectionBothDto : SectionDto
    {
        public required ScriptureUpperDto Scripture { get; set; }
        public required List<ChapterLowerDto> Chapters { get; set; }
    }

    public class SectionOneLevelBothDto : SectionDto
    {
        public required ScriptureDto Scripture { get; set; }
        public required List<ChapterDto> Chapters { get; set; }
    }

    public class SectionMeaningDto : Meaning;

    public abstract class SectionConfinedDto : SectionBaseDto;

    public class SectionUpperConfinedDto : SectionConfinedDto
    {
        public required ScriptureUpperConfinedDto Scripture { get; set; }
    }

    public class SectionLowerConfinedDto : SectionConfinedDto
    {
        public required List<ChapterLowerConfinedDto> Chapters { get; set; }
    }

    //Custom Dtos
    public class SectionMeanDto : SectionBaseDto
    {
        public required List<SectionMeaningDto> Meanings { get; set; } = [];
    }

    public class SectionUpperMeanDto : SectionMeanDto
    {
        public required ScriptureUpperMeanDto Scripture { get; set; }
    }

    public class SectionLowerMeanDto : SectionMeanDto
    {
        public required List<ChapterLowerMeanDto> Chapters { get; set; }
    }
    
    
    public class SectionIndicatorDto : IEquatable<SectionIndicatorDto>
    {
        public required int Scripture { get; set; }
        public required int Section { get; set; }

        public override bool Equals(object? obj) => Equals(obj as SectionIndicatorDto);

        public bool Equals(SectionIndicatorDto? other)
        {
            if (other is null) return false;
            return Scripture == other.Scripture &&
                   Section == other.Section;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Scripture, Section);
        }
    }

    public static class SectionExtensions
    {
        public static SectionDto ToSectionDto(this Section section)
        {
            return new SectionDto
            {
                Id = section.Id,
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(sm => sm.ToSectionMeaningDto()).ToList(),
            };
        }

        public static SectionUpperDto ToSectionUpperDto(this Section section)
        {
            return new SectionUpperDto
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDto()).ToList(),

                Scripture = section.Scripture.ToScriptureDto(),
            };
        }

        public static SectionOneLevelLowerDto ToSectionOneLevelLowerDto(this Section section)
        {
            return new SectionOneLevelLowerDto
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDto()).ToList(),

                Chapters = section.Chapters.Select(c => c.ToChapterDto()).ToList(),
            };
        }

        public static SectionLowerDto ToSectionLowerDto(this Section section)
        {
            return new SectionLowerDto
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDto()).ToList(),

                Chapters = section.Chapters.Select(c => c.ToChapterLowerDto()).ToList(),
            };
        }

        public static SectionBothDto ToSectionBothDto(this Section section)
        {
            return new SectionBothDto
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDto()).ToList(),

                Scripture = section.Scripture.ToScriptureUpperDto(),
                Chapters = section.Chapters.Select(c => c.ToChapterLowerDto()).ToList(),
            };
        }

        public static SectionUpperConfinedDto ToSectionUpperConfinedDto(this Section section)
        {
            return new SectionUpperConfinedDto
            {
                Name = section.Name,
                Number = section.Number,
                Scripture = section.Scripture.ToScriptureUpperConfinedDto()
            };
        }

        public static SectionLowerConfinedDto ToSectionLowerConfinedDto(this Section section)
        {
            return new SectionLowerConfinedDto
            {
                Name = section.Name,
                Number = section.Number,
                Chapters = section.Chapters.Select(c => c.ToChapterLowerConfinedDto()).ToList()
            };
        }

        public static SectionMeaningDto ToSectionMeaningDto(this SectionMeaning sectionMeaning)
        {
            return new SectionMeaningDto
            {
                Text = sectionMeaning.Meaning,
                Language = sectionMeaning.Language.ToLanguageDto()
            };
        }

        public static SectionUpperMeanDto ToSectionUpperMeanDto(this Section section)
        {
            return new SectionUpperMeanDto
            {
                Id = section.Id,
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(sm => sm.ToSectionMeaningDto()).ToList(),
                Scripture = section.Scripture.ToScriptureUpperMeanDto(),
            };
        }

        public static SectionOneLevelBothDto ToSectionOneLevelBothDto(this Section section)
        {
            return new SectionOneLevelBothDto
            {
                Id = section.Id,
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(sm => sm.ToSectionMeaningDto()).ToList(),

                Chapters = section.Chapters.Select(c => c.ToChapterDto()).ToList(),
                Scripture = section.Scripture.ToScriptureDto()
            };
        }
    }
}