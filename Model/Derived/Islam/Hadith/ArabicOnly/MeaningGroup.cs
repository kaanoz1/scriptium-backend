using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>
/// One hadith "meaning": every narration of the same report, across all books,
/// is linked to the same group.
/// </summary>
[Table("i_h_ao_meaning_group")]
public class MeaningGroup
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Column("is_qudsi")] public required bool IsQudsi { get; init; }

    /// <summary>Overall grade of the meaning.</summary>
    [Column("grade")] public required ChainGrade Grade { get; init; }

    /// <summary>Distinct companions narrating it (source-declared, 2017).</summary>
    [Column("companion_count")] public required int CompanionCount { get; init; }

    /// <summary>Occurrences across books (source-declared, 2017).</summary>
    [Column("repeat_count")] public required int RepeatCount { get; init; }

    [Required, Column("text")] public required string Text { get; init; }

    /// <summary>Live count of linked hadiths, computed by the loader.</summary>
    [Column("hadith_count")] public int HadithCount { get; set; }

    /// <summary>The group's canonical opening phrase (one per group).</summary>
    public Taraf? Taraf { get; init; }

    public List<Hadith> Hadiths { get; init; } = [];

    public List<Sanad> Sanads { get; init; } = [];

    public List<Topic> Topics { get; init; } = [];
}
