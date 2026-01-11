using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Model.Derived.Islam.Quranic;

[Table("i_q_chapter")]
public class Chapter : Abstract.Chapter
{
    [Required, Column("sequence")] public required int Sequence { get; init; }

    [Required] public required List<Meaning> Meanings { get; init; }

    public required List<Verse> Verses { get; init; }
}