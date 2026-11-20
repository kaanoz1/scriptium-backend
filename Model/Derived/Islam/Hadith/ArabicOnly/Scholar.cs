using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>A critic of narrators (أئمة الجرح والتعديل).</summary>
[Table("i_h_ao_scholar")]
public class Scholar
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, Column("name")] public required string Name { get; init; }

    [Column("nickname")] public string? Nickname { get; init; }

    /// <summary>The name the scholar is famous by (شهرة).</summary>
    [Required, Column("known_as")] public required string KnownAs { get; init; }

    /// <summary>Honorific (لقب).</summary>
    [Column("title")] public string? Title { get; init; }

    [Column("generation")] public required short Generation { get; init; }

    [Column("death_year")] public required int DeathYear { get; init; }

    [Column("rank_no")] public required NarratorRank RankNo { get; init; }

    [Column("rank")] public string? Rank { get; init; }

    [Column("statement_count")] public required int StatementCount { get; init; }

    [Column("notes")] public string? Notes { get; init; }

    public List<ScholarStatement> Statements { get; init; } = [];
}
