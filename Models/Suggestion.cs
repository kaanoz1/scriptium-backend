/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{


    [NotMapped]
    public class Suggestion
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, ForeignKey("TranslationText")]
        public long TranslationTextId { get; set; }

        [Required]
        [MaxLength(500)]
        public required string SuggestionText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;

        public virtual TranslationText TranslationText { get; set; } = null!;
    }

}

*/