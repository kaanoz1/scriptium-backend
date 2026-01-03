using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Common;

[Table("c_root")]
public class Root
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }
}