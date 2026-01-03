namespace ScriptiumBackend.Dto.Shared.Transliteration;

public class Plain
{
    public required string Text { get; set; }
    public required Language.Plain Language { get; set; }
}