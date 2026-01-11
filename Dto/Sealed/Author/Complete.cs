namespace ScriptiumBackend.Dto.Sealed.Author;

public class Complete : Plain
{
    public required Language.Plain Language { get; init; }

    public required List<Meaning.Plain> NameTranslations { get; init; }
}