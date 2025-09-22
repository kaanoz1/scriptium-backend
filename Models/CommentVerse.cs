/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    [Table("comment_verse"), NotMapped]
    public class CommentVerse
    {
        [Key, Column("comment_id", TypeName = Utility.DBType64bitInteger)]
        public long CommentId { get; set; }

        [Required, Column("verse_id", TypeName = Utility.DBType32bitInteger)]
        public int VerseId { get; set; }

        public virtual Comment Comment { get; set; } = null!;

        public virtual Verse Verse { get; set; } = null!;
    }

}
*/