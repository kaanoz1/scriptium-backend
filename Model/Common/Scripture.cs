using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Common;

[Table("c_scripture")]
public class Scripture
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public short Id { get; init; }
    
    [NotNull, Column("code")] public virtual char Code { get; init; }

    [Required, Column("name"), MaxLength(50)]
    public virtual required string Name { get; init; }

    [Required] public List<Meaning> Meanings { get; init; } = [];
}