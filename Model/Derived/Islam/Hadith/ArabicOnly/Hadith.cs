using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>
/// One narration as it appears in one book. <see cref="Text"/> holds the whole
/// narration (isnad + matn); <see cref="MatnStart"/>/<see cref="MatnEnd"/> are
/// <c>[start, end)</c> character (UTF-16 code unit) offsets of the matn inside it.
/// </summary>
[Table("i_h_ao_hadith")]
public class Hadith
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, ForeignKey(ForeignKeys.Book)]
    public required Book Book { get; init; }

    /// <summary>Number of the hadith inside its book.</summary>
    [Column("number")] public required int Number { get; init; }

    [Column("page")] public required int Page { get; init; }

    [Column("type")] public required HadithType Type { get; init; }

    [Column("grade")] public required HadithGrade Grade { get; init; }

    /// <summary>All narrations of the same report; null for most mawquf/maqtu' reports.</summary>
    [ForeignKey(ForeignKeys.MeaningGroup)]
    public MeaningGroup? MeaningGroup { get; init; }

    /// <summary>Narration-route cluster inside the meaning group.</summary>
    [Column("takhrij")] public int? Takhrij { get; init; }

    /// <summary>Opening phrase (طرف) of the hadith.</summary>
    [Column("taraf_text")] public string? TarafText { get; init; }

    [Required, Column("text")] public required string Text { get; init; }

    [Column("matn_start")] public int? MatnStart { get; init; }

    [Column("matn_end")] public int? MatnEnd { get; init; }

    /// <summary>
    /// Matn without diacritics and with normalised letters (see
    /// <c>ArabicNormalizer</c>); full-text indexed for search.
    /// </summary>
    [Required, Column("search_text")] public required string SearchText { get; init; }

    public List<Sanad> Sanads { get; init; } = [];

    public List<HadithNarratorMention> NarratorMentions { get; init; } = [];

    public List<HadithQuranQuote> QuranQuotes { get; init; } = [];
}
