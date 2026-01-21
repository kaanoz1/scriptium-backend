using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Model.Derived.Islam.Quranic;

[Table("i_q_verse")]
public class Verse : Abstract.Verse
{
    [Required, ForeignKey("i_q_chapter_id")]
    public required Chapter Chapter { get; init; }

    // Content Variations

    [NotNull, Required, MaxLength(5_000), Column("simple")]
    public required string Simple { get; init; }

    [NotNull, Required, MaxLength(5_000), Column("simple_plain")]
    public required string SimplePlain { get; init; }

    [NotNull, Required, MaxLength(5_000), Column("simple_minimal")]
    public required string SimpleMinimal { get; init; }

    [NotNull, Required, MaxLength(5_000), Column("simple_clean")]
    public required string SimpleClean { get; init; }

    [NotNull, Required, MaxLength(5_000), Column("uthmani")]
    public required string Uthmani { get; init; }

    [NotNull, Required, MaxLength(5_000), Column("uthmani_minimal")]
    public required string UthmaniMinimal { get; init; }

    public required List<Word> Words { get; init; }

    public required List<VerseTranslation> Translations { get; init; }

    public required List<Transliteration> Transliterations { get; init; }
}