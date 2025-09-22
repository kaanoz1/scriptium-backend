/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    [Table("comment_note"), NotMapped]
    public class CommentNote
    {
        [Key, Column("comment_id", TypeName = Utility.DBType64bitInteger)]
        public long CommentId { get; set; }

        [Required, Column("note_id", TypeName = Utility.DBType64bitInteger)]
        public long NoteId { get; set; }

        public virtual Comment Comment { get; set; } = null!;

        public virtual Note Note { get; set; } = null!;
    }

}
*/