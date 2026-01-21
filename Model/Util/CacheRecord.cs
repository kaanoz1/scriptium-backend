using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Util;

[Table(("u_cache_record"))]
public class CacheRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public long Id { get; set; }


    [Required, ForeignKey("cache_id")] public required Cache Cache { get; set; }

    [Required, Column("fetched_at")] public required DateTime FetchedAt { get; set; }
}