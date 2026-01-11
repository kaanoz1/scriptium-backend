namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

public class UpToVerse : Complete
{
    public required Derived.Islam.Quranic.Verse.Complete Verse { get; set; }
}