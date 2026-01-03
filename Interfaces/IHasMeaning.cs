using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Interfaces;

public interface IHasMeaning
{
    List<Meaning> Meanings { get; set; }
}

public interface IDtoHasMeaning
{
    public List<Dto.Shared.Meaning.Plain> Meanings { get; set; }
}