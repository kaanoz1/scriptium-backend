using ScriptiumBackend.Models;

namespace DTO
{
    public class RootBaseDto
    {
        public required string Latin { get; set; }

        public required string Own { get; set; }
    }

    public class RootDto : RootBaseDto;

    public class RootUpperDto : RootDto
    {
        public required List<WordUpperDto> Words { get; set; }
    }

    public class RootLowerDto : RootDto;

    public abstract class RootConfinedDto : RootBaseDto;

    public class RootUpperConfinedDto : RootConfinedDto
    {
        public required List<WordUpperConfinedDto> Words { get; set; }
    }

    public class RootLowerConfinedDto : RootConfinedDto;


    
    
    public class RootScriptureConfinedDto
    {
        public required string Latin { get; set; }

        public required string Own { get; set; }

        public required ScriptureUpperMeanDto Scripture { get; set; }
        
    }
    
    
    
    public class RootIdentifierDto
    {
        public required string Latin { get; set; }

        public required string Own { get; set; }

        public required int ScriptureNumber { get; set; }
    }
    public static class RootExtensions
    {
        public static RootDto ToRootDto(this Root root)
        {
            return new RootDto
            {
                Own = root.Own,
                Latin = root.Latin
            };
        }

        public static RootUpperDto ToRootUpperDto(this Root root)
        {
            return new RootUpperDto
            {
                Own = root.Own,
                Latin = root.Latin,
                Words = root.Words.Select(w => w.ToWordUpperDto()).ToList()
            };
        }

        public static RootUpperConfinedDto ToRootUpperConfinedDto(this Root root)
        {
            return new RootUpperConfinedDto
            {
                Latin = root.Latin,
                Own = root.Own,
                Words = root.Words.Select(w => w.ToWordUpperConfinedDto()).ToList()
            };
        }

        public static RootLowerConfinedDto ToRootLowerConfinedDto(this Root root)
        {
            return new RootLowerConfinedDto
            {
                Latin = root.Latin,
                Own = root.Own,
            };
        }
    }
}