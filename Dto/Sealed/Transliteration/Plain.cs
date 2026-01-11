namespace ScriptiumBackend.Dto.Sealed.Transliteration;

public class Plain
{
    public required string Text { get; set; }
    public required Sealed.Language.Plain Language { get; set; }
}