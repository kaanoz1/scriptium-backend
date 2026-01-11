namespace ScriptiumBackend.Dto.Sealed.Meaning;

public class Plain
{
    public required string Text { get; set; }
    public required Sealed.Language.Plain Language { get; set; }
}