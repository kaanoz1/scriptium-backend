using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Sealed;

[Table("s_meaning")]
public sealed class Meaning
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; init; }

    [Required, MaxLength(100), Column("content")]
    public required string Content { get; init; }

    [ForeignKey("s_language_id")] public required Language Language { get; init; }

}