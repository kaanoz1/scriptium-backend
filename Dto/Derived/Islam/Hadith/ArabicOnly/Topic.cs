using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

public class TopicPlain
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}

public class TopicChild
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int ChildCount { get; init; }

    /// <summary>Total descendants (from the nested set).</summary>
    public required int DescendantCount { get; init; }

    public MeaningGroupPlain? Group { get; init; }
}

public class TopicComplete : ICacheable
{
    /// <summary>Null for the virtual root.</summary>
    public TopicPlain? Topic { get; init; }
    public required List<TopicPlain> Path { get; init; }
    public required List<TopicChild> Children { get; init; }
    public MeaningGroupPlain? Group { get; init; }
}
