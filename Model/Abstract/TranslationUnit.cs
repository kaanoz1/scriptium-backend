using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Sealed;
using ScriptiumBackend.Model.Util;

namespace ScriptiumBackend.Model.Abstract;

[Table("a_translationUnit")]
public abstract class TranslationUnit : SearchableItem, ISearchableContent
{
    [Required, Column("text"), StringLength(2500)]
    public required string Text { get; init; }

    [Required] public required List<Footnote> Footnotes { get; init; }

    [Required, ForeignKey("s_language_id")]
    public required Language Language { get; init; }
    
    public string GetContentForEmbedding() => Text;
}