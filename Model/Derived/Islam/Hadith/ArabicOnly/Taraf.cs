using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>Canonical opening phrase (طرف) of a hadith meaning.</summary>
[Table("i_h_ao_taraf")]
public class Taraf
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, Column("text")] public required string Text { get; init; }

    [Column("grade")] public required ChainGrade Grade { get; init; }

    [Column("is_qudsi")] public required bool IsQudsi { get; init; }

    [ForeignKey(ForeignKeys.MeaningGroup)]
    public MeaningGroup? MeaningGroup { get; init; }
}
