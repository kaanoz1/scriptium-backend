using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Sealed;

[Table("s_transliteration")]
public sealed class Transliteration
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required long Id { get; init; }

    [Required, Column("content"), StringLength(2_500)]
    public required string Content { get; init; }

    [Required, ForeignKey("s_language_id")]
    public required Language Language { get; init; }
}