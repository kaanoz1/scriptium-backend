using System.Collections.Generic;
using ScriptiumBackend.Dto.Sealed.Meaning;
using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Interfaces;

public interface IHasMeaning
{
    List<Meaning> Meanings { get; set; }
}

public interface IHasPlainMeaning
{
    public List<Plain> Meanings { get; set; }
}