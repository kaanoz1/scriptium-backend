using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Abstract;

namespace ScriptiumBackend.Model.Sealed;

[Table("s_footnote")]
public sealed class Footnote
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required long Id { get; init; }

    [Required, StringLength(1000), Column("text")]
    public required string Text { get; init; }

    [Required, StringLength(100), Column("indicator")]
    public required string Indicator { get; init; }

    [Required, Column("index")] public required long Index { get; init; }

    [Required, ForeignKey("a_translationUnit_id")]
    public required TranslationUnit Translation { get; init; }
}