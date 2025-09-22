using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    public class Cache
    {
        [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
        public long Id { get; set; }

        [Required, MaxLength(126)]
        public required string Key { get; set; }

        [Required, Column("data", TypeName = Utility.DBTypeVARCHARMAX)]
        public required string Data { get; set; }

        [Required, Column(TypeName = Utility.DBTypeDateTime)]
        public DateTime ExpirationDate { get; set; } = DateTime.UtcNow;

        public virtual List<CacheRecord>? Records { get; set; }
    }
}
