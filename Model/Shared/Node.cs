using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Shared;

[Table("c_node")]
public class Node
{
    [Key, Required, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }
    
    [ForeignKey("parent_id")]
    public Node? Parent { get; init; }
    
    
    public virtual ICollection<Node> Children { get; init; } = [];
}