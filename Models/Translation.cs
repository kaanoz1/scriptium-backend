using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Models
{
    public class Translation
    {
        [Key, Column("id", TypeName = Utility.DBType16bitInteger)]
        public required short Id { get; set; }

        [Required, Column("name", TypeName = Utility.DBTypeVARCHAR250), MaxLength(300)]
        public required string Name { get; set; }

        [Column("production_year")]
        public DateTime? ProductionTime { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [Column("eager_from", TypeName = Utility.DBTypeDateTime)]
        public DateTime? EagerFrom { get; set; }

        [Required, ForeignKey("Language"), Column("language_id", TypeName = Utility.DBType8bitInteger)]
        public byte LanguageId { get; set; } = 1;

        public virtual Language Language { get; set; } = null!;

        [Required, ForeignKey("Scripture"), Column("scripture_id", TypeName = Utility.DBType16bitInteger)]
        public short ScriptureId { get; set; } = 1;

        public virtual Scripture Scripture { get; set; } = null!;

        public virtual List<TranslatorTranslation> TranslatorTranslations { get; set; } = [];

        
        public virtual List<TranslationText> TranslationTexts { get; set; } = [];
    }
}
