using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Common;

[Table("c_chapter")]
public class Chapter
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; init; }

    [Required, Column("name"), MaxLength(50)]
    public virtual required string Name { get; init; }
}