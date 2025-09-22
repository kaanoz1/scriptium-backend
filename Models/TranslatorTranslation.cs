using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ScriptiumBackend.Models
{
    public class TranslatorTranslation
    {
        [Required, Column("translator_id", TypeName = Utility.DBType16bitInteger)]
        public short TranslatorId { get; set; }

        [ForeignKey("TranslatorId")]
        public virtual Translator Translator { get; set; } = null!;

        [Required, Column("translation_id", TypeName = Utility.DBType16bitInteger)]
        public short TranslationId { get; set; }

        [ForeignKey("TranslationId")]
        public virtual Translation Translation { get; set; } = null!;

        [Column("assigned_on", TypeName = Utility.DBTypeDateTime)]
        public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
    }
}
