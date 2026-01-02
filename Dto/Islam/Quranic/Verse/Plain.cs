using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Verse;

public class Plain: IHasSequence
{
    public int Sequence { get; set; }

    public required string Simple { get; init; }

    public required string SimplePlain { get; init; }

    public required string SimpleMinimal { get; init; }

    public required string SimpleClean { get; init; }

    public required string Uthmani { get; init; }

    public required string UthmaniMinimal { get; init; }
}