namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

public class UpToVerse : Complete
{
    public required Dto.Islam.Quranic.Verse.Complete Verse { get; set; }
}