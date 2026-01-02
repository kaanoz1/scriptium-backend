using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Islam.Quranic.Chapter;

public class Complete : Plain, IHasMeaning
{
    public required List<Model.Shared.Meaning> Meanings { get; set; }
}