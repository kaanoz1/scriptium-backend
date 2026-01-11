using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Abstract;

[Table("a_verse")]
public abstract class Verse
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required short Id { get; init; }

    [Required, Column("number")] public required int Number { get; init; }
}