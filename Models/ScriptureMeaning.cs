using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace scriptium_backend_dotnet.Models
{
    public class ScriptureMeaning
    {
        [Key, Column("id", TypeName = Utility.DBType16bitInteger)]
        public required int Id { get; set; }

        [Required, Column("meaning", TypeName = Utility.DBTypeVARCHAR50)]
        public required string Meaning { get; set; }

        [Required, Column("scripture_id", TypeName = Utility.DBType16bitInteger)]
        public short ScriptureId { get; set; }
        public virtual Scripture Scripture { get; set; } = null!;

        [Required, Column("language_id", TypeName = Utility.DBType8bitInteger)]
        public byte LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;

    }
}