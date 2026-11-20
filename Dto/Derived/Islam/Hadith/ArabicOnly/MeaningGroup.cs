using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

public class MeaningGroupPlain
{
    public required int Id { get; init; }
    public required ChainGrade Grade { get; init; }
    public required bool IsQudsi { get; init; }
    public required string Text { get; init; }
    public required int HadithCount { get; init; }
    public required int CompanionCount { get; init; }
}

public class MeaningGroupComplete : ICacheable
{
    public required MeaningGroupPlain Group { get; init; }
    public required List<List<TopicPlain>> TopicPaths { get; init; }
    public required List<CodeCount> GradeDistribution { get; init; }
    public required List<BookCount> Books { get; init; }
    public required int ChainCount { get; init; }

    /// <summary>Companions at the top of the group's chains.</summary>
    public required List<NarratorLinkItem> Companions { get; init; }
}

/// <summary>Merged isnad graph of all chains of a meaning group.</summary>
public class IsnadGraph : ICacheable
{
    public required List<IsnadNode> Nodes { get; init; }
    public required List<IsnadEdge> Edges { get; init; }
    public required int ChainCount { get; init; }

    /// <summary>True when rare branches were dropped to keep the graph readable.</summary>
    public required bool Truncated { get; init; }
    public required int MinEdgeCount { get; init; }
}

public class IsnadNode
{
    public required NarratorPlain Narrator { get; init; }

    /// <summary>Distance from the top of the chain (0 = companion / earliest).</summary>
    public required int Depth { get; init; }

    /// <summary>Chains of the group passing through this narrator.</summary>
    public required int ChainCount { get; init; }

    /// <summary>True when the narrator is a book author (bottom of a chain).</summary>
    public required bool IsAuthor { get; init; }
}

public class IsnadEdge
{
    public required int TeacherId { get; init; }
    public required int StudentId { get; init; }
    public required int Count { get; init; }
}
