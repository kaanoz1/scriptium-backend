using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models;

public class BookNode
{
    [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
    public required long Id { get; set; }
    
    [ForeignKey("ParentBookNode"), Column("parent_node_id", TypeName = Utility.DBType64bitInteger)]
    public long? ParentBookNodeId { get; set; }

    public virtual BookNode? ParentBookNode { get; set; }

    public virtual List<BookNode> ChildNodes { get; set; } = [];
    
    [ForeignKey("Book"), Column("BookId", TypeName = Utility.DBType32bitInteger)]
    public int? BookId { get; set; }

    public virtual Book? Book { get; set; }

    [Required, Column(TypeName = Utility.DBTypeNVARCHAR255), MaxLength(50)]
    public required string Name { get; set; }
    
    public virtual List<BookNodeMeaning> Meanings { get; set; } = [];
       
    public virtual List<BookText> Texts { get; set; } = [];
    
    
    [Column("description", TypeName = Utility.DBTypeVARCHARMAX)]
    public string? Description { get; set; }

    

}