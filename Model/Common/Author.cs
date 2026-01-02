using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Shared;
using ScriptiumBackend.Utils.Constants;

namespace ScriptiumBackend.Model.Common;

[Table("c_author")]
public class Author
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id;

    [Column("name")]
    [Required] public required string Name;

    [Column("language")]
    public required Language Language;

    public List<Meaning> NameTranslations = [];
}