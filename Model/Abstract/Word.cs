using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Abstract;

[Table("a_word")]
public abstract class Word
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required int Id { get; init; }

    [Required, Column("sequence")] public required int Sequence { get; init; }

    [Required, Column("text"), StringLength(100)]
    public required string Text { get; init; }
}