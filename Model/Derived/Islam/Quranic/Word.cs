using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Model.Derived.Islam.Quranic;

[Table("i_q_word")]
public class Word : Abstract.Word
{
    [Required] public required List<Transliteration> Transliterations { get; init; }

    [Required] public required List<Meaning> Meanings { get; init; }

    [Required, ForeignKey("i_q_verse_id")] public required Verse Verse { get; init; }

    [Required] public required List<Root> Roots { get; init; }
}