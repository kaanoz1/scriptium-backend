using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Abstract;

[Table("a_chapter")]
public abstract class Chapter
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required short Id { get; init; }

    [Required, Column("name"), MaxLength(50)]
    public required string Name { get; init; }
}