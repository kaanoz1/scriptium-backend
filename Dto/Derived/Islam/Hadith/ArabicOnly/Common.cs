using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

/// <summary>One page of a longer list.</summary>
public class Paged<T> : ICacheable
{
    public required List<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }

    /// <summary>True when <see cref="TotalCount"/> was capped (search).</summary>
    public bool TotalIsLowerBound { get; init; }
}

/// <summary>Count of rows per code (grade, type, rank, ...).</summary>
public class CodeCount
{
    public required int Code { get; init; }
    public required int Count { get; init; }
}

/// <summary><c>[Start, End)</c> character range inside a text.</summary>
public class TextRange
{
    public required int Start { get; init; }
    public required int End { get; init; }
}

public class BookCount
{
    public required BookPlain Book { get; init; }
    public required int Count { get; init; }
}
