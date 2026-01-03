using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Common;

[Table("c_word")]
public class Word
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; set; }

    [Required, Column("sequence_number")] public required int SequenceNumber { get; init; }

    [Required, Column("content")] public required string Content { get; init; }

}