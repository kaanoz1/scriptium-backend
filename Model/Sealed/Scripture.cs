using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Sealed;

[Table("s_scripture")]
public sealed class Scripture
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public short Id { get; init; }

    [Column("code")] public required char Code { get; init; }

    [Required, Column("name"), MaxLength(50)]
    public required string Name { get; init; }

    [Required] public required List<Meaning> Meanings { get; init; }
}