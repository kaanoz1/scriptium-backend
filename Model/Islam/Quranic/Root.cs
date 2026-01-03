using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Islam.Quranic;

[Table("i_q_root")]
public class Root
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; init; }

    [Required, ForeignKey("c_root_id")]
    public required Common.Root RootC { get; init; }

    [Required, Column("content"), MaxLength(25)]
    public required string Content { get; init; }

    [Required, Column("latin"), MaxLength(25)]
    public required string Latin { get; init; }

    public List<Word> Words { get; init; } = [];
}