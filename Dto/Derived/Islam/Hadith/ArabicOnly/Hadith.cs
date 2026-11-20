using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

/// <summary>Row of a hadith list (book pages, narrator pages, search, ...).</summary>
public class HadithListItem
{
    public required int Id { get; init; }
    public required BookPlain Book { get; init; }
    public required int Number { get; init; }
    public required HadithType Type { get; init; }
    public required HadithGrade Grade { get; init; }
    public int? MeaningGroupId { get; init; }

    /// <summary>The matn (or the opening of the text when no matn span exists), capped in length.</summary>
    public required string Matn { get; init; }
    public required bool MatnTruncated { get; init; }

    /// <summary>Search matches inside <see cref="Matn"/>.</summary>
    public List<TextRange>? Highlights { get; init; }
}

public class NarratorMention
{
    public required int Start { get; init; }
    public required int End { get; init; }
    public int? NarratorId { get; init; }
}

public class QuranQuote
{
    public required int Start { get; init; }
    public required int End { get; init; }
    public short? Chapter { get; init; }
    public short? VerseStart { get; init; }
    public short? VerseEnd { get; init; }
}

public class SanadLink
{
    public required short Position { get; init; }
    public required int NarratorId { get; init; }
}

public class SanadComplete
{
    public required int Id { get; init; }
    public required ChainGrade Grade { get; init; }
    public required string Verdict { get; init; }
    public NarratorRank? MaxRank { get; init; }
    public short? Length { get; init; }

    /// <summary>Ordered from the book author (position 0) up to the companion.</summary>
    public required List<SanadLink> Narrators { get; init; }
}

public class HadithComplete : ICacheable
{
    public required int Id { get; init; }
    public required BookPlain Book { get; init; }
    public required string CategoryName { get; init; }
    public required int Number { get; init; }
    public required int Page { get; init; }
    public required HadithType Type { get; init; }
    public required HadithGrade Grade { get; init; }
    public string? TarafText { get; init; }
    public required string Text { get; init; }
    public int? MatnStart { get; init; }
    public int? MatnEnd { get; init; }

    public required List<NarratorMention> Mentions { get; init; }
    public required List<QuranQuote> Quotes { get; init; }
    public required List<SanadComplete> Sanads { get; init; }

    /// <summary>Every narrator referenced by mentions or chains, by id.</summary>
    public required List<NarratorPlain> Narrators { get; init; }

    public MeaningGroupPlain? MeaningGroup { get; init; }
    public required List<List<TopicPlain>> TopicPaths { get; init; }

    /// <summary>A few narrations of the same meaning in other places.</summary>
    public required List<HadithListItem> OtherNarrations { get; init; }

    public int? PreviousNumber { get; init; }
    public int? NextNumber { get; init; }
}
