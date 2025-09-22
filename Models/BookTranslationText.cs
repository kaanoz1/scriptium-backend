using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Models;

public class BookTranslationText
{
    [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
    public long Id { get; set; }

    [Required, Column("text", TypeName = Utility.DBTypeNVARCHARMAX)]
    public required string Text { get; set; }

    [Required, Column("translation_id", TypeName = Utility.DBType16bitInteger), ForeignKey("BookTranslation")]
    public short TranslationId { get; set; }

    public virtual BookTranslation Translation { get; set; } = null!;

    [Required, Column("book_text_id", TypeName = Utility.DBType64bitInteger), ForeignKey("BookText")]
    public long BookTextId { get; set; }

    public virtual BookText BookText { get; set; } = null!;
    
    public List<BookTranslationTextFootNote> Footnotes { get; set; } = [];
}