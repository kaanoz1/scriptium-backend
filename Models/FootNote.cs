using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    public class FootNote
    {
        [Key, Column("id")]
        public long Id { get; set; }

        [Column(TypeName = Utility.DBType16bitInteger)]
        public short Index { get; set; }

        [Required, Column("footnote_text_id", TypeName = Utility.DBType64bitInteger)]
        public long FootNoteTextId { get; set; }

        public virtual FootNoteText FootNoteText { get; set; } = null!;

        [Required, Column("translation_text_id", TypeName = Utility.DBType64bitInteger)]
        public required long TranslationTextId { get; set; }

        public virtual TranslationText TranslationText { get; set; } = null!;
    }
}