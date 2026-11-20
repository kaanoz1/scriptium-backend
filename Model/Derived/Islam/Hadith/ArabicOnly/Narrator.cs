using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>A hadith narrator (راوي). Ids are the source ids.</summary>
[Table("i_h_ao_narrator")]
public class Narrator
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, Column("name")] public required string Name { get; init; }

    /// <summary>Kunya / short name the narrator is known by.</summary>
    [Required, Column("nickname")] public required string Nickname { get; init; }

    [Column("rank_no")] public required NarratorRank RankNo { get; init; }

    /// <summary>Verbal grade, e.g. ثقة حافظ.</summary>
    [Column("rank")] public string? Rank { get; init; }

    /// <summary>Generation layer (طبقة) as numbered by the source; 0 = unknown.</summary>
    [Column("generation")] public required short Generation { get; init; }

    [Column("is_bukhari")] public required bool IsBukhari { get; init; }

    [Column("is_muslim")] public required bool IsMuslim { get; init; }

    /// <summary>Became confused in later life (اختلاط).</summary>
    [Column("has_ikhtilat")] public required bool HasIkhtilat { get; init; }

    /// <summary>Known for concealment in transmission (تدليس).</summary>
    [Column("has_tadlis")] public required bool HasTadlis { get; init; }

    /// <summary>Narration count declared by the source.</summary>
    [Column("riwaya_count")] public required int RiwayaCount { get; init; }

    [Column("birth_year")] public int? BirthYear { get; init; }

    [Column("birth_year_raw")] public string? BirthYearRaw { get; init; }

    [Column("death_year")] public int? DeathYear { get; init; }

    [Column("death_year_raw")] public string? DeathYearRaw { get; init; }

    [Column("age_raw")] public string? AgeRaw { get; init; }

    [Column("profession")] public string? Profession { get; init; }

    /// <summary>Lineage / tribal attribution (نسب).</summary>
    [Column("lineage")] public string? Lineage { get; init; }

    /// <summary>Place(s) of residence (إقامة).</summary>
    [Column("residence")] public string? Residence { get; init; }

    [Column("death_place")] public string? DeathPlace { get; init; }

    /// <summary>Placeholder for a chain narrator the source never described.</summary>
    [Column("is_stub")] public required bool IsStub { get; init; }

    /// <summary>Normalised name + nickname, trigram indexed for search.</summary>
    [Required, Column("search_name")] public required string SearchName { get; init; }

    /// <summary>Distinct hadiths whose chains contain this narrator (computed by the loader).</summary>
    [Column("hadith_count")] public int HadithCount { get; set; }

    public List<ScholarStatement> Statements { get; init; } = [];
}
