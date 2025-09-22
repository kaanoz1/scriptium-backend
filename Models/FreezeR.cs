/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;
using ScriptiumBackend.Models.Util;

namespace ScriptiumBackend
{

    [NotMapped]
    public class FreezeR
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public required FreezeStatus Status { get; set; }

        [Required]
        public required Guid UserId { get; set; }

        [Required]
        public DateTime ProceedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
*/