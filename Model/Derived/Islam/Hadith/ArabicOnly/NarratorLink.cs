using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>
/// Teacher → student edge derived from consecutive chain positions
/// (the student at position n heard from the teacher at n + 1), with the
/// number of chains it occurs in. Computed by the loader.
/// </summary>
[Table("i_h_ao_narrator_link")]
public class NarratorLink
{
    [Required, ForeignKey(ForeignKeys.Teacher)]
    public required Narrator Teacher { get; init; }

    [Required, ForeignKey(ForeignKeys.Student)]
    public required Narrator Student { get; init; }

    [Column("chain_count")] public required int ChainCount { get; init; }
}
