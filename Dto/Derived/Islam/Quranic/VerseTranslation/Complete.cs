namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation;

public class Complete : Plain
{
    public required List<Sealed.Footnote.Plain> Footnotes { get; init; } = [];

    public required Sealed.Language.Plain Language { get; init; }

    public required List<Sealed.Author.Complete> Authors { get; init; }
}