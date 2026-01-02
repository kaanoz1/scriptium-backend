using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Common;


[Table("c_section")]
public class Section
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required, Column("name"), MaxLength(50)]
    public virtual required string Name { get; init; }
}