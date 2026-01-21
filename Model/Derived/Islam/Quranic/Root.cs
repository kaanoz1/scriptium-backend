using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Quranic;

[Table("i_q_root")]
public class Root : Abstract.Root
{
    [Required, Column("latin"), StringLength(25)]
    public required string TextInLatin { get; init; }

    public required List<Word> Words { get; init; }
}