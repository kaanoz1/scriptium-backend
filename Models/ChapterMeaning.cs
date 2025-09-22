using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    public class ChapterMeaning
    {
        [Key, Column("id", TypeName = Utility.DBType32bitInteger)]
        public int Id { get; set; }

        [Column("meaning", TypeName = Utility.DBTypeVARCHAR100)]
        public required string Meaning { get; set; }

        [Required, Column("chapter_id", TypeName = Utility.DBType16bitInteger)]
        public short ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; } = null!;

        [Required, Column("language_id", TypeName = Utility.DBType8bitInteger)]
        public byte LanguageId { get; set; }

        public virtual Language Language { get; set; } = null!;
    }
}