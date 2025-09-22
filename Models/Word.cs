using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ScriptiumBackend.Models
{
    public class Word
    {
        [Key, Column("id", TypeName = Utility.DBType64bitInteger)]
        public long Id { get; set; }

        [Required, Column("sequence_number", TypeName = Utility.DBType16bitInteger)]
        public required short SequenceNumber { get; set; }

        [Required, Column("text", TypeName = Utility.DBTypeNVARCHAR50), MaxLength(50)]
        public required string Text { get; set; }

        [Column("text_without_vowel", TypeName = Utility.DBTypeNVARCHAR50), MaxLength(50)]
        public string? TextWithoutVowel { get; set; }

        [Column("text_simplified", TypeName = Utility.DBTypeNVARCHAR50), MaxLength(50)]
        public string? TextSimplified { get; set; }

        [Required, Column("verse_id", TypeName = Utility.DBType32bitInteger)]
        public int VerseId { get; set; }

        public virtual Verse Verse { get; set; } = null!;

        public virtual List<Root> Roots { get; set; } = [];

        public virtual List<WordMeaning> WordMeanings { get; set; } = [];

    }
}