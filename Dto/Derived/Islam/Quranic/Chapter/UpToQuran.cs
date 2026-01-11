using ScriptiumBackend.Utils.Predefined.Model.Islam;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;

public class UpToQuran : Complete
{
    public required QuranPlain Scripture { get; set; }
}