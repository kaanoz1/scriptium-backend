/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models.Util;

namespace ScriptiumBackend.Models
{
    [NotMapped]
    public class Notification
    {
        public long Id { get; set; }

        [Required]
        public Guid RecipientId { get; set; }

        [Required]
        public Guid ActorId { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        public EntityType? EntityType { get; set; }

        public Guid? EntityId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public virtual User Recipient { get; set; } = null!;

        public virtual User Actor { get; set; } = null!;
    }

}
*/