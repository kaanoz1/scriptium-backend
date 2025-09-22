/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.



using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    [Table("collection"), NotMapped]
    public class Collection
    {
        [Key, Column("id", TypeName = Utility.DBType32bitInteger)]
        public int Id { get; set; }

        [Required, Column("name", TypeName = Utility.DBTypeVARCHAR24), MaxLength(24)]
        public required string Name { get; set; }

        [MaxLength(72), Column("description", TypeName = Utility.DBTypeVARCHAR72)]
        public string? Description { get; set; }

        [Required, ForeignKey("User"), Column("user_id", TypeName = Utility.DBTypeUUID)]
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;

        [Column("created_at", TypeName = Utility.DBTypeDateTime)]
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public virtual List<CollectionVerse>? Verses { get; set; }

    }
}
*/