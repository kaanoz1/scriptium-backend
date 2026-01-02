using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ScriptiumBackend.Model.Util;

[Table("u_cache")]
public class Cache
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; set; }


    [Required, Column("url")] public required string Url { get; set; }
    [Required, Column("data")] public required string Data { get; set; }
    [Required, Column("created_at")] public required DateTime CreatedAt { get; set; }
    [Required, Column("updated_at")] public required DateTime UpdatedAt { get; set; }

    [Required] public required List<CacheRecord> Records { get; set; }
}