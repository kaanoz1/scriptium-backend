/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    [NotMapped]
    public class LikeNote
    {
        [Key]
        public long LikeId { get; set; }

        [Required]
        public long NoteId { get; set; }

        public virtual Like Like { get; set; } = null!;

        public virtual Note Note { get; set; } = null!;
    }
}
*/