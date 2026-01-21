using System.Collections.Generic;
using ScriptiumBackend.Dto.Sealed.Language;

namespace ScriptiumBackend.Utils.Predefined.Model.Islam;

public class QuranPlain
{
    public string Code { get; } = "Q";
    public string Name { get; } = "القرآن الكريم";

    public List<Dto.Sealed.Meaning.Plain> Meanings { get; } =
    [
        new()
        {
            Language = Constants.Default.Language.English.ToPlainDto(),
            Text = "Al Quran Kareem",
        }
    ];
}