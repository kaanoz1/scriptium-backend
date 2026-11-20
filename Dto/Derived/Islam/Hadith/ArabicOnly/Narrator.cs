using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

public class NarratorPlain
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Nickname { get; init; }
    public required NarratorRank RankNo { get; init; }
    public string? Rank { get; init; }
    public required short Generation { get; init; }
    public int? DeathYear { get; init; }
    public required bool IsStub { get; init; }
    public required int HadithCount { get; init; }
}

public class NarratorComplete : ICacheable
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Nickname { get; init; }
    public required NarratorRank RankNo { get; init; }
    public string? Rank { get; init; }
    public required short Generation { get; init; }
    public required bool IsBukhari { get; init; }
    public required bool IsMuslim { get; init; }
    public required bool HasIkhtilat { get; init; }
    public required bool HasTadlis { get; init; }
    public required int RiwayaCount { get; init; }
    public int? BirthYear { get; init; }
    public string? BirthYearRaw { get; init; }
    public int? DeathYear { get; init; }
    public string? DeathYearRaw { get; init; }
    public string? AgeRaw { get; init; }
    public string? Profession { get; init; }
    public string? Lineage { get; init; }
    public string? Residence { get; init; }
    public string? DeathPlace { get; init; }
    public required bool IsStub { get; init; }
    public required int HadithCount { get; init; }

    public required List<StatementWithScholar> Statements { get; init; }
    public required List<NarratorLinkItem> Teachers { get; init; }
    public required int TeacherCount { get; init; }
    public required List<NarratorLinkItem> Students { get; init; }
    public required int StudentCount { get; init; }
    public required List<BookCount> Books { get; init; }
    public required List<CodeCount> GradeDistribution { get; init; }
}

public class NarratorLinkItem
{
    public required NarratorPlain Narrator { get; init; }
    public required int ChainCount { get; init; }
}

public class StatementWithScholar
{
    public required int Id { get; init; }
    public required string Text { get; init; }
    public required ScholarPlain Scholar { get; init; }
}

public class StatementWithNarrator
{
    public required int Id { get; init; }
    public required string Text { get; init; }
    public required NarratorPlain Narrator { get; init; }
}
