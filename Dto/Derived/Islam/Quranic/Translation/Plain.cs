namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Translation;

public class Plain
{
    public required int Id { get; set; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required Dto.Sealed.Language.Plain Language { get; init; }
}