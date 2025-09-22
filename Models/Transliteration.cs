using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ScriptiumBackend.Models
{
    public class Transliteration
    {
        [Key, Column("id", TypeName = Utility.DBType32bitInteger)]
        public int Id { get; set; }

        [Required, Column("text", TypeName = Utility.DBTypeVARCHARMAX)]
        public required string Text { get; set; }

        [Required, Column("language_id", TypeName = Utility.DBType8bitInteger), ForeignKey("Language")]
        public byte LanguageId { get; set; }

        public virtual Language Language { get; set; } = null!;

        [Required, Column("verse_id", TypeName = Utility.DBType32bitInteger), ForeignKey("Verse")]
        public int VerseId { get; set; }

        public virtual Verse Verse { get; set; } = null!;
    }
}
