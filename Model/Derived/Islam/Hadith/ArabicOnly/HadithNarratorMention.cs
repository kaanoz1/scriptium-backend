using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>
/// A narrator name inside <see cref="Hadith.Text"/>, as <c>[Start, End)</c>
/// offsets, in text order. <see cref="Narrator"/> is null when the source
/// link was malformed or points at an undescribed narrator.
/// </summary>
[Table("i_h_ao_hadith_narrator_mention")]
public class HadithNarratorMention
{
    [Required, ForeignKey(ForeignKeys.Hadith)]
    public required Hadith Hadith { get; init; }

    [Column("sequence")] public required short Sequence { get; init; }

    [ForeignKey(ForeignKeys.Narrator)]
    public Narrator? Narrator { get; init; }

    [Column("start")] public required int Start { get; init; }

    [Column("end")] public required int End { get; init; }
}
