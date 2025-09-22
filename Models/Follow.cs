/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models.Util;

namespace ScriptiumBackend.Models
{
    [NotMapped]
    public class Follow
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public Guid FollowerId { get; set; }

        [Required]
        public Guid FollowedId { get; set; }

        [Required]
        public FollowStatus Status { get; set; }

        [Required]
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("FollowerId")]
        public virtual User Follower { get; set; } = null!;

        [ForeignKey("FollowedId")]
        public virtual User Followed { get; set; } = null!;
    }
}
*/