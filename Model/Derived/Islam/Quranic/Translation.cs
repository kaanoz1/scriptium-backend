using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Quranic;

[Table("i_q_translation")]
public class Translation : Abstract.Translation
{
    [Required] public required List<VerseTranslation> VerseTranslations { get; init; }
}