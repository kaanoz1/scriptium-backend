using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Models;

public class BookTranslation
{
    [Key, Column("id", TypeName = Utility.DBType16bitInteger)]
    public required short Id { get; set; }

    [Required, Column("name", TypeName = Utility.DBTypeVARCHAR250), MaxLength(300)]
    public required string Name { get; set; }

    [Required, ForeignKey("Language"), Column("language_id", TypeName = Utility.DBType8bitInteger)]
    public byte LanguageId { get; set; }

    public virtual Language Language { get; set; } = null!;
    
    public virtual List<BookTranslationBookTranslator> TranslationBookTranslators { get; set; } = [];

    public virtual List<BookTranslationText> TranslationTexts { get; set; } = [];
}
