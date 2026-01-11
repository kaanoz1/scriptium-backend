using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Interfaces;

public interface IHasTransliteration
{
    public List<Transliteration> Transliterations { get; set; }
}

public interface IHasPlainTransliteration
{
    public List<Dto.Sealed.Transliteration.Plain> Transliterations { get; set; }
}