using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Common;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Islam.Quranic;

[Table("i_q_word")]
public class Word
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; init; }

    [Required] public required Common.Word WordC;

    [Required]
    public required List<Transliteration> Transliterations { get; init; }
    
    [Required]
    public required List<Meaning> Meanings { get; init; }

}