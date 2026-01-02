using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Islam.Quranic;

[Table("i_q_chapter")]
public class Chapter
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public short Id { get; init; }

    [Required] public required Common.Chapter ChapterC;

    [Required, Column("sequence")]
    public required int Sequence { get; init; }

    [Required]
    public required List<Meaning> Meanings { get; init; }

    public List<Verse> Verses { get; init; } = [];
}