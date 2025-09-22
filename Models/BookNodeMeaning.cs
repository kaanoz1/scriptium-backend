using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Models;

public class BookNodeMeaning
{
    [Key, Column("id", TypeName = Utility.DBType32bitInteger)]
    public required int Id { get; set; }

    [Required, Column("meaning", TypeName = Utility.DBTypeNVARCHAR50)]
    public required string Meaning { get; set; }

    [Required, Column("book_node_id", TypeName = Utility.DBType64bitInteger), ForeignKey("BookNode")]
    public long BookNodeId { get; set; }
    public virtual BookNode BookNode { get; set; } = null!;

    [Required, Column("language_id", TypeName = Utility.DBType8bitInteger)]
    public byte LanguageId { get; set; }
    public virtual Language Language { get; set; } = null!;
}