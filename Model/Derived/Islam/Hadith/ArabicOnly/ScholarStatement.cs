using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>A jarh wa ta'dil statement (قول) of a scholar about a narrator.</summary>
[Table("i_h_ao_scholar_statement")]
public class ScholarStatement
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, ForeignKey(ForeignKeys.Scholar)]
    public required Scholar Scholar { get; init; }

    [Required, ForeignKey(ForeignKeys.Narrator)]
    public required Narrator Narrator { get; init; }

    [Required, Column("text")] public required string Text { get; init; }
}
