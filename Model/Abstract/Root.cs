using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Abstract;

[Table("a_root")]
public abstract class Root
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [Required, StringLength(100), Column("text")]
    public required string Text { get; init; }
}