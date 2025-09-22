using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ScriptiumBackend.Models
{
    public class WordMeaning
    {
        [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
        public long Id { get; set; }

        [Required, Column("word_meaning", TypeName = Utility.DBTypeVARCHAR100), MaxLength(100)]
        public string Meaning { get; set; } = null!;

        [Required, ForeignKey("Word"), Column("word_id", TypeName = Utility.DBType64bitInteger)]
        public long WordId { get; set; }

        public virtual Word Word { get; set; } = null!;

        [Required, ForeignKey("Language"), Column("language_id", TypeName = Utility.DBType8bitInteger)]
        public byte LanguageId { get; set; }

        public virtual Language Language { get; set; } = null!;
    }
}
