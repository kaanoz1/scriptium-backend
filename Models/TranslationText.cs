using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    public class TranslationText
    {
        [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
        public long Id { get; set; }

        [Required, Column("text", TypeName = Utility.DBTypeVARCHARMAX)]
        public required string Text { get; set; }

        [Required, Column("translation_id", TypeName = Utility.DBType16bitInteger), ForeignKey("Translation")]
        public short TranslationId { get; set; }

        public virtual Translation Translation { get; set; } = null!;

        [Required, Column("verse_id", TypeName = Utility.DBType32bitInteger), ForeignKey("Verse")]
        public int VerseId { get; set; }

        public virtual Verse Verse { get; set; } = null!;

        public virtual List<FootNote> FootNotes { get; set; } = [];
/*
        Unused properties, but kept for future use or reference.

        public virtual List<Suggestion>? Suggestions { get; set; }
*/
    }
}
