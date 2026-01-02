using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Common;

[Table("c_word")]
public class Word
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] public required int SequenceNumber { get; init; }

    [Required] public required string Content { get; init; }

    [Required] public List<Root> Roots { get; init; }

    public long VerseId { get; init; } // Foreign Key

    [ForeignKey(nameof(VerseId))] public required Verse Verse { get; init; }
}