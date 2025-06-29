using scriptium_backend_dotnet.Models;

namespace DTO
{
    public abstract class ScriptureBaseDTO
    {
        public required short Id { get; set; }

        public required string Name { get; set; }

        public required byte Number { get; set; }

        public required char Code { get; set; }
    }

    public abstract class ScriptureSimpleDTO : ScriptureBaseDTO
    {
        public List<ScriptureMeaningDTO> Meanings { get; set; } = [];
    }

    public class ScriptureDTO : ScriptureSimpleDTO;

    public class ScriptureLowerDTO : ScriptureDTO
    {
        public required List<SectionLowerDTO> Sections { get; set; }
    }

    public class ScriptureUpperDTO : ScriptureDTO;

    public class ScriptureOneLevelUpperDTO : ScriptureDTO;

    public class ScriptureOneLevelLowerDTO : ScriptureDTO
    {
        public required List<SectionDTO> Sections { get; set; }
    }

    public class ScriptureMeaningDTO : Meaning;

    public abstract class ScriptureConfinedDTO : ScriptureBaseDTO;

    public class ScriptureUpperConfinedDTO : ScriptureConfinedDTO;

    public class ScriptureLowerConfinedDTO : ScriptureConfinedDTO
    {
        public required List<SectionLowerConfinedDTO> Sections { get; set; }
    }

    // Custom DTOs
    public class ScriptureMeanDTO : ScriptureBaseDTO
    {
        public List<ScriptureMeaningDTO> Meanings { get; set; } = [];
    }

    public class ScriptureUpperMeanDTO : ScriptureMeanDTO;

    public class ScriptureLowerMeanDTO : ScriptureMeanDTO
    {
        public required List<SectionLowerMeanDTO> Sections { get; set; }
    }

    public static class ScriptureExtensions
    {
        public static ScriptureMeaningDTO ToScriptureMeaningDTO(this ScriptureMeaning scriptureMeaning)
        {
            return new ScriptureMeaningDTO
            {
                Text = scriptureMeaning.Meaning,
                Language = scriptureMeaning.Language.ToLanguageDTO()
            };
        }

        public static ScriptureDTO ToScriptureDTO(this Scripture scripture)
        {
            return new ScriptureDTO
            {
                Id = scripture.Id,
                Name = scripture.Name,
                Code = scripture.Code,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDTO()).ToList()
            };
        }

        public static ScriptureOneLevelLowerDTO ToScriptureOneLevelLowerDTO(this Scripture scripture)
        {
            return new ScriptureOneLevelLowerDTO
            {
                Id = scripture.Id,
                Name = scripture.Name,
                Code = scripture.Code,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDTO()).ToList(),
                Sections = scripture.Sections.Select(s => s.ToSectionDTO()).ToList()
            };
        }

        public static ScriptureLowerDTO ToScriptureLowerDTO(this Scripture scripture)
        {
            return new ScriptureLowerDTO
            {
                Id = scripture.Id,
                Code = scripture.Code,
                Name = scripture.Name,
                Number = scripture.Number,
                Sections = scripture.Sections.Select(s => s.ToSectionLowerDTO()).ToList()
            };
        }

        public static ScriptureUpperConfinedDTO ToScriptureUpperConfinedDTO(this Scripture scripture)
        {
            return new ScriptureUpperConfinedDTO
            {
                Id = scripture.Id,
                Code = scripture.Code,
                Name = scripture.Name,
                Number = scripture.Number
            };
        }

        public static ScriptureLowerConfinedDTO ToScriptureLowerConfinedDTO(this Scripture scripture)
        {
            return new ScriptureLowerConfinedDTO
            {
                Code = scripture.Code,
                Id = scripture.Id,
                Name = scripture.Name,
                Number = scripture.Number,
                Sections = scripture.Sections.Select(s => s.ToSectionLowerConfinedDTO()).ToList()
            };
        }

        public static ScriptureUpperMeanDTO ToScriptureUpperMeanDTO(this Scripture scripture)
        {
            return new ScriptureUpperMeanDTO
            {
                Id = scripture.Id,
                Code = scripture.Code,
                Name = scripture.Name,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDTO()).ToList()
            };
        }


        public static ScriptureUpperDTO ToScriptureUpperDTO(this Scripture scripture)
        {
            return new ScriptureUpperDTO
            {
                Id = scripture.Id,
                Name = scripture.Name,
                Code = scripture.Code,
                Number = scripture.Number,
                Meanings = scripture.Meanings.Select(m => m.ToScriptureMeaningDTO()).ToList()
            };
        }
    }
}