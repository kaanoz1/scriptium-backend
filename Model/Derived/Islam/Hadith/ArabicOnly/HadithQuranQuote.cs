using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>
/// A Qur'an quotation inside <see cref="Hadith.Text"/> (<c>[Start, End)</c>
/// offsets). The referenced verse range is parsed from the source's
/// "سورة … آية …" suffix; null when it could not be resolved.
/// Stored as chapter/verse numbers rather than a foreign key so hadith data
/// does not depend on the Qur'an tables being loaded.
/// </summary>
[Table("i_h_ao_hadith_quran_quote")]
public class HadithQuranQuote
{
    [Required, ForeignKey(ForeignKeys.Hadith)]
    public required Hadith Hadith { get; init; }

    [Column("sequence")] public required short Sequence { get; init; }

    [Column("start")] public required int Start { get; init; }

    [Column("end")] public required int End { get; init; }

    [Column("chapter")] public short? Chapter { get; init; }

    [Column("verse_start")] public short? VerseStart { get; init; }

    [Column("verse_end")] public short? VerseEnd { get; init; }
}
