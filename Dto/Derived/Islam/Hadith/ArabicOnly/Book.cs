using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

public class BookPlain
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string AuthorName { get; init; }
    public int? AuthorDeathYear { get; init; }
    public required BookCategoryKind CategoryId { get; init; }
    public required int HadithCount { get; init; }
}

public class BookComplete : ICacheable
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string AuthorName { get; init; }
    public int? AuthorDeathYear { get; init; }
    public string? City { get; init; }
    public string? Publisher { get; init; }
    public string? Edition { get; init; }
    public required BookCategoryPlain Category { get; init; }
    public required int HadithCount { get; init; }
    public required int FirstNumber { get; init; }
    public required int LastNumber { get; init; }
    public required List<CodeCount> GradeDistribution { get; init; }
    public required List<CodeCount> TypeDistribution { get; init; }
    public required int ChainCount { get; init; }
    public required int QuranQuoteCount { get; init; }
}

public class BookCategoryPlain
{
    public required BookCategoryKind Id { get; init; }
    public required string Name { get; init; }
}

public class BookCategoryWithBooks
{
    public required BookCategoryKind Id { get; init; }
    public required string Name { get; init; }
    public required List<BookPlain> Books { get; init; }
}
