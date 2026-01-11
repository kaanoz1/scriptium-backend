using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Abstract;

namespace ScriptiumBackend.Model.Derived.Islam.Quranic;

[Table("i_q_verse_translation")]
public class VerseTranslation : TranslationUnit
{
    [Required, ForeignKey("i_q_verse_id")] public required Verse Verse { get; init; }


    [Required, ForeignKey("i_q_translation_id")] public required Translation Translation { get; init; }
}