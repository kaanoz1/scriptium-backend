using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

public class ScholarPlain
{
    public required int Id { get; init; }
    public required string KnownAs { get; init; }
    public required string Name { get; init; }
    public required int DeathYear { get; init; }
    public required NarratorRank RankNo { get; init; }
    public required int StatementCount { get; init; }
}

public class ScholarComplete : ICacheable
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Nickname { get; init; }
    public required string KnownAs { get; init; }
    public string? Title { get; init; }
    public required short Generation { get; init; }
    public required int DeathYear { get; init; }
    public required NarratorRank RankNo { get; init; }
    public string? Rank { get; init; }
    public required int StatementCount { get; init; }
    public string? Notes { get; init; }
}

public class ScholarList : ICacheable
{
    public required List<ScholarPlain> Scholars { get; init; }
}
