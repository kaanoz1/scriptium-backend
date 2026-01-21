using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Root;

public class Plain: ICacheable
{
    public required string Text { get; set; }
    public required string Latin { get; set; }
}