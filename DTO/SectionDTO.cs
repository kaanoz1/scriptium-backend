using scriptium_backend_dotnet.Models;

namespace DTO
{
    public abstract class SectionBaseDTO
    {
        public short Id { get; set; }

        public required string Name { get; set; }

        public required short Number { get; set; }
    }

    public abstract class SectionSimpleDTO : SectionBaseDTO
    {
        public required List<SectionMeaningDTO> Meanings { get; set; } = [];
    }

    public class SectionDTO : SectionSimpleDTO;


    public class SectionUpperDTO : SectionDTO
    {
        public required ScriptureDTO Scripture { get; set; }
    }


    public class SectionLowerDTO : SectionDTO
    {
        public required List<ChapterLowerDTO> Chapters { get; set; }
    }

    public class SectionOneLevelLowerDTO : SectionDTO
    {
        public required List<ChapterDTO> Chapters { get; set; }
    }

    public class SectionBothDTO : SectionDTO
    {
        public required ScriptureUpperDTO Scripture { get; set; }
        public required List<ChapterLowerDTO> Chapters { get; set; }
    }

    public class SectionOneLevelBothDTO : SectionDTO
    {
        public required ScriptureDTO Scripture { get; set; }
        public required List<ChapterDTO> Chapters { get; set; }
    }

    public class SectionMeaningDTO : Meaning;

    public abstract class SectionConfinedDTO : SectionBaseDTO;

    public class SectionUpperConfinedDTO : SectionConfinedDTO
    {
        public required ScriptureUpperConfinedDTO Scripture { get; set; }
    }

    public class SectionLowerConfinedDTO : SectionConfinedDTO
    {
        public required List<ChapterLowerConfinedDTO> Chapters { get; set; }
    }

    //Custom DTOs
    public class SectionMeanDTO : SectionBaseDTO
    {
        public required List<SectionMeaningDTO> Meanings { get; set; } = [];
    }

    public class SectionUpperMeanDTO : SectionMeanDTO
    {
        public required ScriptureUpperMeanDTO Scripture { get; set; }
    }

    public class SectionLowerMeanDTO : SectionMeanDTO
    {
        public required List<ChapterLowerMeanDTO> Chapters { get; set; }
    }
    
    
    public class SectionIndicatorDTO : IEquatable<SectionIndicatorDTO>
    {
        public required int Scripture { get; set; }
        public required int Section { get; set; }

        public override bool Equals(object? obj) => Equals(obj as SectionIndicatorDTO);

        public bool Equals(SectionIndicatorDTO? other)
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
        public static SectionDTO ToSectionDTO(this Section section)
        {
            return new SectionDTO
            {
                Id = section.Id,
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(sm => sm.ToSectionMeaningDTO()).ToList(),
            };
        }

        public static SectionUpperDTO ToSectionUpperDTO(this Section section)
        {
            return new SectionUpperDTO
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDTO()).ToList(),

                Scripture = section.Scripture.ToScriptureDTO(),
            };
        }

        public static SectionOneLevelLowerDTO ToSectionOneLevelLowerDTO(this Section section)
        {
            return new SectionOneLevelLowerDTO
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDTO()).ToList(),

                Chapters = section.Chapters.Select(c => c.ToChapterDTO()).ToList(),
            };
        }

        public static SectionLowerDTO ToSectionLowerDTO(this Section section)
        {
            return new SectionLowerDTO
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDTO()).ToList(),

                Chapters = section.Chapters.Select(c => c.ToChapterLowerDTO()).ToList(),
            };
        }

        public static SectionBothDTO ToSectionBothDTO(this Section section)
        {
            return new SectionBothDTO
            {
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(e => e.ToSectionMeaningDTO()).ToList(),

                Scripture = section.Scripture.ToScriptureUpperDTO(),
                Chapters = section.Chapters.Select(c => c.ToChapterLowerDTO()).ToList(),
            };
        }

        public static SectionUpperConfinedDTO ToSectionUpperConfinedDTO(this Section section)
        {
            return new SectionUpperConfinedDTO
            {
                Name = section.Name,
                Number = section.Number,
                Scripture = section.Scripture.ToScriptureUpperConfinedDTO()
            };
        }

        public static SectionLowerConfinedDTO ToSectionLowerConfinedDTO(this Section section)
        {
            return new SectionLowerConfinedDTO
            {
                Name = section.Name,
                Number = section.Number,
                Chapters = section.Chapters.Select(c => c.ToChapterLowerConfinedDTO()).ToList()
            };
        }

        public static SectionMeaningDTO ToSectionMeaningDTO(this SectionMeaning sectionMeaning)
        {
            return new SectionMeaningDTO
            {
                Text = sectionMeaning.Meaning,
                Language = sectionMeaning.Language.ToLanguageDTO()
            };
        }

        public static SectionUpperMeanDTO ToSectionUpperMeanDTO(this Section section)
        {
            return new SectionUpperMeanDTO
            {
                Id = section.Id,
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(sm => sm.ToSectionMeaningDTO()).ToList(),
                Scripture = section.Scripture.ToScriptureUpperMeanDTO(),
            };
        }

        public static SectionOneLevelBothDTO ToSectionOneLevelBothDTO(this Section section)
        {
            return new SectionOneLevelBothDTO
            {
                Id = section.Id,
                Name = section.Name,
                Number = section.Number,
                Meanings = section.Meanings.Select(sm => sm.ToSectionMeaningDTO()).ToList(),

                Chapters = section.Chapters.Select(c => c.ToChapterDTO()).ToList(),
                Scripture = section.Scripture.ToScriptureDTO()
            };
        }
    }
}