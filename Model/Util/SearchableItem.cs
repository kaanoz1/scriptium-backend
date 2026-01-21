using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace ScriptiumBackend.Model.Util;

[Table("u_searchable_item")]
public abstract class SearchableItem
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required long Id { get; init; }

    [Column("embedding", TypeName = "vector(768)")]
    public Vector? Embedding { get; set; }
    
    [Column("last_embedded_at")] public DateTime? LastEmbeddedAt { get; set; }
}