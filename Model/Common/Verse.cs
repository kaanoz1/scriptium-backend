using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Common;

[Table("c_verse")]
public class Verse
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; set; }

    [Required, Column("number")] public required int Number { get; init; }

    public required List<Word> Words { get; init; }
}