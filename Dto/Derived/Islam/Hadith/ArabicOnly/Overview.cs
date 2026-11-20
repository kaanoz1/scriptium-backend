using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

public class CorpusCounts
{
    public required int Books { get; init; }
    public required int Hadiths { get; init; }
    public required int Chains { get; init; }
    public required int Narrators { get; init; }
    public required int Scholars { get; init; }
    public required int Statements { get; init; }
    public required int MeaningGroups { get; init; }
    public required int Topics { get; init; }
    public required int QuranQuotes { get; init; }
}

public class CorpusOverview : ICacheable
{
    public required CorpusCounts Counts { get; init; }
    public required List<BookCategoryWithBooks> Categories { get; init; }
    public required List<CodeCount> GradeDistribution { get; init; }
    public required List<CodeCount> TypeDistribution { get; init; }
    public required List<CodeCount> NarratorRankDistribution { get; init; }
    public required List<NarratorPlain> TopNarrators { get; init; }
    public required List<TopicChild> RootTopics { get; init; }
}

public class SitemapSummary : ICacheable
{
    public required int Hadiths { get; init; }
    public required int Books { get; init; }
    public required int MeaningGroups { get; init; }
    public required int PageSize { get; init; }
}

/// <summary>Compact list of valid routes for the sitemap.</summary>
public class SitemapHadithPage : ICacheable
{
    /// <summary>[bookId, number] pairs.</summary>
    public required List<int[]> Hadiths { get; init; }
}

public class SitemapIdPage : ICacheable
{
    public required List<int> Ids { get; init; }
}
