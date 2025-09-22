using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ScriptiumBackend.Models
{
    public class Chapter
    {
        [Key, Column("id", TypeName = Utility.DBType16bitInteger)]
        public short Id { get; set; }

        [Required, Column("name", TypeName = Utility.DBTypeNVARCHAR255)]
        public required string Name { get; set; }

        [Required, Column("number", TypeName = Utility.DBType8bitInteger)]
        public required byte Number { get; set; }

        [NotMapped]
        public short VerseCount => (short)(Verses?.Count ?? -1);

        [Required, Column("section_id", TypeName = Utility.DBType16bitInteger)]
        public short SectionId { get; set; }

        public virtual Section Section { get; set; } = null!;

        public virtual List<Verse> Verses { get; set; } = [];

        public virtual List<ChapterMeaning> Meanings { get; set; } = [];

    }
}