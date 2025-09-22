using ScriptiumBackend.Models;

namespace DTO
{
    public abstract class ScriptureBaseDto
    {
        public required short Id { get; set; }

        public required string Name { get; set; }

        public required byte Number { get; set; }

        public required char Code { get; set; }
    }

    public abstract class ScriptureSimpleDto : ScriptureBaseDto
    {
        public List<ScriptureMeaningDto> Meanings { get; set; } = [];
    }

    public class ScriptureDto : ScriptureSimpleDto;

    public class ScriptureLowerDto : ScriptureDto
    {
        public required List<SectionLowerDto> Sections { get; set; }
    }

    public class ScriptureUpperDto : ScriptureDto;

    public class ScriptureOneLevelUpperDto : ScriptureDto;

    public class ScriptureOneLevelLowerDto : ScriptureDto
    {
        public required List<SectionDto> Sections { get; set; }
    }

    public class ScriptureMeaningDto : Meaning;

    public abstract class ScriptureConfinedDto : ScriptureBaseDto;

    public class ScriptureUpperConfinedDto : ScriptureConfinedDto;

    public class ScriptureLowerConfinedDto : ScriptureConfinedDto
    {
        public required List<SectionLowerConfinedDto> Sections { get; set; }
    }

    // Custom Dtos
    public class ScriptureMeanDto : ScriptureBaseDto
    {
        public List<ScriptureMeaningDto> Meanings { get; set; } = [];
    }

    public class ScriptureUpperMeanDto : ScriptureMeanDto;

    public class ScriptureLowerMeanDto : ScriptureMeanDto
    {
        public required List<SectionLowerMeanDto> Sections { get; set; }
    }

    public static class ScriptureExtensions
    {
        public static ScriptureMeaningDto ToScriptureMeaningDto(this ScriptureMeaning scriptureMeaning)
        {
            return new ScriptureMeaningDto
            {
                Text = scriptureMeaning.Meaning,
                Language = scriptureMeaning.Language.ToLanguageDto()
            };
        }

        public static ScriptureDto ToScriptureDto(this Scripture scripture)
        {
            return new ScriptureDto
            {
                Id = scripture.Id,
                Name = scripture.Name,
                Code = scripture.Code,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDto()).ToList()
            };
        }

        public static ScriptureOneLevelLowerDto ToScriptureOneLevelLowerDto(this Scripture scripture)
        {
            return new ScriptureOneLevelLowerDto
            {
                Id = scripture.Id,
                Name = scripture.Name,
                Code = scripture.Code,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDto()).ToList(),
                Sections = scripture.Sections.Select(s => s.ToSectionDto()).ToList()
            };
        }

        public static ScriptureLowerDto ToScriptureLowerDto(this Scripture scripture)
        {
            return new ScriptureLowerDto
            {
                Id = scripture.Id,
                Code = scripture.Code,
                Name = scripture.Name,
                Number = scripture.Number,
                Sections = scripture.Sections.Select(s => s.ToSectionLowerDto()).ToList()
            };
        }

        public static ScriptureUpperConfinedDto ToScriptureUpperConfinedDto(this Scripture scripture)
        {
            return new ScriptureUpperConfinedDto
            {
                Id = scripture.Id,
                Code = scripture.Code,
                Name = scripture.Name,
                Number = scripture.Number
            };
        }

        public static ScriptureLowerConfinedDto ToScriptureLowerConfinedDto(this Scripture scripture)
        {
            return new ScriptureLowerConfinedDto
            {
                Code = scripture.Code,
                Id = scripture.Id,
                Name = scripture.Name,
                Number = scripture.Number,
                Sections = scripture.Sections.Select(s => s.ToSectionLowerConfinedDto()).ToList()
            };
        }

        public static ScriptureUpperMeanDto ToScriptureUpperMeanDto(this Scripture scripture)
        {
            return new ScriptureUpperMeanDto
            {
                Id = scripture.Id,
                Code = scripture.Code,
                Name = scripture.Name,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDto()).ToList()
            };
        }


        public static ScriptureUpperDto ToScriptureUpperDto(this Scripture scripture)
        {
            return new ScriptureUpperDto
            {
                Id = scripture.Id,
                Name = scripture.Name,
                Code = scripture.Code,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDto()).ToList()
            };
        }
        
        
    }
}