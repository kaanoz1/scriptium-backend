using System.Collections.Generic;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public class Down : Complete, ICacheable
{
    public required List<Word.Down> Words { get; set; }
}