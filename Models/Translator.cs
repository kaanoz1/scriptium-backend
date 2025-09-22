using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;


namespace ScriptiumBackend.Models
{
    public class Translator
    {
        [Key, Column("id", TypeName = Utility.DBType16bitInteger)]
        public short Id { get; set; }

        [Required, Column("name", TypeName = Utility.DBTypeVARCHAR250), MaxLength(250)]
        public required string Name { get; set; } = null!;

        [Column("description", TypeName = Utility.DBTypeVARCHARMAX)]
        public string? Description { get; set; }

        [Column("url", TypeName = Utility.DBTypeVARCHARMAX)]
        public string? Url { get; set; }

        [Required, Column("language_id"), ForeignKey("Language")]
        public byte LanguageId { get; set; }

        public virtual Language Language { get; set; } = null!;

        public virtual List<TranslatorTranslation>? TranslatorTranslations { get; set; }
        
        public virtual List<BookTranslationBookTranslator> TranslationBookTranslators { get; set; } = [];
        
    }
}
