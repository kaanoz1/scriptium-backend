using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    public class FootNoteText
    {
        [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
        public long Id { get; set; }

        [Required, Column("text", TypeName = Utility.DBTypeNVARCHARMAX)]
        public string Text { get; set; } = null!;

        public virtual List<FootNote>? FootNotes { get; set; }
    }
}
