using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Models;

public class BookTranslationBookTranslator
{
    [Key, Column(TypeName = Utility.DBType64bitInteger)]
    public long Id { get; set; }
    
    [Required, Column("translator_id", TypeName = Utility.DBType16bitInteger)]
    public short TranslatorId { get; set; }

    [ForeignKey("TranslatorId")]
    public virtual Translator Translator { get; set; } = null!;

    [Required, Column("translation_id", TypeName = Utility.DBType16bitInteger)]
    public short TranslationId { get; set; }

    [ForeignKey("TranslationId")]
    public virtual BookTranslation Translation { get; set; } = null!;

    [Column("assigned_on", TypeName = Utility.DBTypeDateTime)]
    public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
}