using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models;

public class BookText
{
    [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
    public required long Id { get; set; }

    [Required, Column("text", TypeName = Utility.DBTypeNVARCHARMAX)]
    public required string Text { get; set; }

    [Required, Column("sequence_number", TypeName = Utility.DBType16bitInteger)]
    public required short SequenceNumber { get; set; }
    
    public virtual List<BookTranslationText> TranslationTexts { get; set; } = [];
}