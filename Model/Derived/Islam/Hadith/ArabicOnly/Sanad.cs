using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>One chain of narration (إسناد) of a hadith. A hadith can have several.</summary>
[Table("i_h_ao_sanad")]
public class Sanad
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, ForeignKey(ForeignKeys.Hadith)]
    public required Hadith Hadith { get; init; }

    /// <summary>Grade of this chain; <see cref="Verdict"/> gives the reasoning.</summary>
    [Column("grade")] public required ChainGrade Grade { get; init; }

    /// <summary>Full verdict sentence on the isnad.</summary>
    [Required, Column("verdict")] public required string Verdict { get; init; }

    /// <summary>Same as the hadith's group; kept on the chain to query a group's chains directly.</summary>
    [ForeignKey(ForeignKeys.MeaningGroup)]
    public MeaningGroup? MeaningGroup { get; init; }

    [Column("takhrij")] public int? Takhrij { get; init; }

    /// <summary>Weakest narrator's rank in the chain (including the author).</summary>
    [Column("max_rank")] public NarratorRank? MaxRank { get; init; }

    /// <summary>Distinct narrators in the chain, including the book author.</summary>
    [Column("length")] public short? Length { get; init; }

    public List<SanadNarrator> Narrators { get; init; } = [];
}
