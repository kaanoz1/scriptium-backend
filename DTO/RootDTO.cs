using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class RootBaseDTO
    {
        public required string Latin { get; set; }

        public required string Own { get; set; }
    }

    public class RootDTO : RootBaseDTO;

    public class RootUpperDTO : RootDTO
    {
        public required List<WordUpperDTO> Words { get; set; }
    }

    public class RootLowerDTO : RootDTO;

    public abstract class RootConfinedDTO : RootBaseDTO;

    public class RootUpperConfinedDTO : RootConfinedDTO
    {
        public required List<WordUpperConfinedDTO> Words { get; set; }
    }

    public class RootLowerConfinedDTO : RootConfinedDTO;


    
    
    public class RootScriptureConfinedDTO
    {
        public required string Latin { get; set; }

        public required string Own { get; set; }

        public required ScriptureUpperMeanDTO Scripture { get; set; }
        
    }
    
    
    
    public class RootIdentifierDTO
    {
        public required string Latin { get; set; }

        public required string Own { get; set; }

        public required int ScriptureNumber { get; set; }
    }
    public static class RootExtensions
    {
        public static RootDTO ToRootDTO(this Root root)
        {
            return new RootDTO
            {
                Own = root.Own,
                Latin = root.Latin
            };
        }

        public static RootUpperDTO ToRootUpperDTO(this Root root)
        {
            return new RootUpperDTO
            {
                Own = root.Own,
                Latin = root.Latin,
                Words = root.Words.Select(w => w.ToWordUpperDTO()).ToList()
            };
        }

        public static RootUpperConfinedDTO ToRootUpperConfinedDTO(this Root root)
        {
            return new RootUpperConfinedDTO
            {
                Latin = root.Latin,
                Own = root.Own,
                Words = root.Words.Select(w => w.ToWordUpperConfinedDTO()).ToList()
            };
        }

        public static RootLowerConfinedDTO ToRootLowerConfinedDTO(this Root root)
        {
            return new RootLowerConfinedDTO
            {
                Latin = root.Latin,
                Own = root.Own,
            };
        }
    }
}