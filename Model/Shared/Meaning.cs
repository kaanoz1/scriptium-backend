using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Shared;

[Table("s_meaning")]
public class Meaning
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; init; }

    [Required, MaxLength(100), Column("content")]
    public required string Content { get; init; }

    [ForeignKey(nameof(LanguageId))] public required Language Language { get; init; }

    public int LanguageId { get; init; }
}