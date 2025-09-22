using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ScriptiumBackend.Models;

public class Book
{
    [Key, Column("id", TypeName = Utility.DBType32bitInteger)]
    public int Id { get; init; }
    
    [Required, Column(TypeName = Utility.DBTypeNVARCHAR255), MaxLength(50)]
    public required string Name { get; init; }

    
    [Column("description", TypeName = Utility.DBTypeVARCHARMAX)]
    public string? Description { get; init; }    
    
    public virtual List<BookMeaning> Meanings { get; init; } = [];
    
    public virtual List<BookNode> Nodes { get; init; } = [];
    
    
    
}