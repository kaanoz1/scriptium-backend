/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    [NotMapped]
    public class LikeComment
    {
        [Key]
        public long LikeId { get; set; }

        [Required]
        public long CommentId { get; set; }

        public virtual Like Like { get; set; } = null!;

        public virtual Comment Comment { get; set; } = null!;
    }
}
*/