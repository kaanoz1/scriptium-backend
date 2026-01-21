using System.Collections.Generic;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

public class Meaning: Plain, IHasMeaning
{
    public required List<Model.Sealed.Meaning> Meanings { get; set; }
}