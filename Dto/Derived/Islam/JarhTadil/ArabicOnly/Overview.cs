using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.JarhTadil.ArabicOnly;

public class JarhTadilCounts
{
    /// <summary>Narrators with a biography (placeholders excluded).</summary>
    public required int Narrators { get; init; }

    public required int Companions { get; init; }
    public required int Scholars { get; init; }
    public required int Statements { get; init; }

    /// <summary>Distinct teacher → student pairs seen in the chains.</summary>
    public required int Links { get; init; }
}

public class JarhTadilOverview : ICacheable
{
    public required JarhTadilCounts Counts { get; init; }
    public required List<CodeCount> RankDistribution { get; init; }
    public required List<NarratorPlain> TopNarrators { get; init; }
    public required List<ScholarPlain> TopScholars { get; init; }
}

public class JarhTadilSitemapSummary : ICacheable
{
    public required int Narrators { get; init; }
    public required int Scholars { get; init; }
    public required int PageSize { get; init; }
}
