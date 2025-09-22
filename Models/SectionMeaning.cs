using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    public class SectionMeaning
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("meaning", TypeName = Utility.DBTypeVARCHAR100)]
        public required string Meaning { get; set; }

        [Required, Column("section_id", TypeName = Utility.DBType16bitInteger)]
        public short SectionId { get; set; }

        public virtual Section Section { get; set; } = null!;

        [Required, Column("language_id", TypeName = Utility.DBType8bitInteger)]
        public byte LanguageId { get; set; }

        public virtual Language Language { get; set; } = null!;

    }
}