using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Shared;

[Table("s_transliteration")]
public class Transliteration
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; set; }

    [Required, Column("content")]
    public required string Content { get; init; }

    [Required, ForeignKey("language_id")] public required Language Language { get; init; }
}