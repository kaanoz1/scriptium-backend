using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Model.Abstract;

[Table("a_translationUnit")]
public abstract class TranslationUnit
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required long Id { get; init; }

    [Required, Column("text"), StringLength(2500)]
    public required string Text { get; init; }

    [Required] public required List<Footnote> Footnotes { get; init; }

    [Required, ForeignKey("s_language_id")]
    public required Language Language { get; init; }
}