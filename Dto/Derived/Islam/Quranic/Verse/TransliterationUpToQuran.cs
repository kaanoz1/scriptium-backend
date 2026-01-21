namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;

public class TransliterationUpToQuran: PlainUpToQuran
{
    public required List<Dto.Sealed.Transliteration.Plain> Transliterations { get; set; }
}