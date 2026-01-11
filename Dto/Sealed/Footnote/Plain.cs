namespace ScriptiumBackend.Dto.Sealed.Footnote;

public class Plain
{
    public required string Text { get; set; }

    public required string Indicator { get; init; }

    public required long Index { get; init; }
}