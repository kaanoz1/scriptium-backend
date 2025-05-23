using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scriptium_backend_dotnet.Models
{
    public class CacheR
    {
        [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
        public long Id { get; set; }

        [Required, Column("cache_id", TypeName = Utility.DBType64bitInteger)]
        public long CacheId { get; set; }

        public DateTime FetchedAt { get; set; } = DateTime.UtcNow;

        public virtual Cache Cache { get; set; } = null!;
    }

}