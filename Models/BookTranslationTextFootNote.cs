using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Models;

public class BookTranslationTextFootNote
{
    [Key, Column("id")]
    public long Id { get; set; }

    [Column(TypeName = Utility.DBType32bitInteger)]
    public int Index { get; set; }

    [Required, Column("text", TypeName = Utility.DBTypeNVARCHARMAX)]
    public string Text { get; set; } = string.Empty;
    
    [Required, Column("indicator", TypeName = Utility.DBTypeNVARCHAR30)]
    public string Indicator { get; set; } = string.Empty;

    [Required, Column("book_text_translation_text_id", TypeName = Utility.DBType64bitInteger)]
    public required long TranslationTextId { get; set; }

    public virtual BookTranslationText TranslationText { get; set; } = null!;
}