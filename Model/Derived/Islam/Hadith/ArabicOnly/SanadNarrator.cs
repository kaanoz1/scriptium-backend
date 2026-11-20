using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

[Table("i_h_ao_sanad_narrator")]
public class SanadNarrator
{
    [Required, ForeignKey(ForeignKeys.Sanad)]
    public required Sanad Sanad { get; init; }

    [Column("position")] public required short Position { get; init; }

    [Required, ForeignKey(ForeignKeys.Narrator)]
    public required Narrator Narrator { get; init; }

    [Required, ForeignKey(ForeignKeys.Hadith)]
    public required Hadith Hadith { get; init; }
}
