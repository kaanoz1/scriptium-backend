using ScriptiumBackend.Dto.Islam.Quranic.Verse;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Dto.Islam.Quranic.Chapter;

using Dto = Quranic;

public class WithVerses : Complete, ICacheable
{
    public required List<Dto.Verse.Plain> Verses { get; init; }

}