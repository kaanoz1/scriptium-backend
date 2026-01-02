using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Dto.Islam.Quranic.Word;

public class Meaning: Plain, IHasMeaning
{
    public required List<Model.Shared.Meaning> Meanings { get; set; }
}