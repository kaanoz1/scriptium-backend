using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Interfaces;

public interface IHasMeaning
{
    List<Meaning> Meanings { get; set; }
}