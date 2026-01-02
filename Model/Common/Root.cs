using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Common;

[Table("c_root")]
public class Root
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }

    [Required, Column("content"), MaxLength(25)]
    public required string Content { get; init; }

    [Required, Column("latin"), MaxLength(25)]
    public required string Latin { get; init; }
    
    public List<Word> Words { get; init; } = [];
}