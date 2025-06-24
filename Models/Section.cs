using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace scriptium_backend_dotnet.Models
{
    public class Section
    {
        [Key, Column("id", TypeName = Utility.DBType16bitInteger)]
        public short Id { get; set; }

        [Required, Column("name", TypeName = Utility.DBTypeNVARCHAR255)]
        public required string Name { get; set; }

        [Required, Column("number", TypeName = Utility.DBType16bitInteger)]
        public required short Number { get; set; }

        [NotMapped]
        public short ChapterCount => (short)(Chapters?.Count ?? -1);

        [Required, Column("scripture_id", TypeName = Utility.DBType16bitInteger)]
        public short ScriptureId { get; set; }

        public virtual Scripture Scripture { get; set; } = null!;

        public virtual List<Chapter> Chapters { get; set; } = [];

        public virtual List<SectionMeaning> Meanings { get; set; } = [];

    }
}