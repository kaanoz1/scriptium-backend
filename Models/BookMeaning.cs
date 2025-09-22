using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Models;

public class BookMeaning
{
    [Key, Column("id", TypeName = Utility.DBType32bitInteger)]
    public required int Id { get; set; }

    [Required, Column("meaning", TypeName = Utility.DBTypeVARCHAR50)]
    public required string Meaning { get; set; }

    [Required, Column("book_id", TypeName = Utility.DBType32bitInteger), ForeignKey("Book")]
    public int BookId { get; set; }
    public virtual Book Book { get; set; } = null!;

    [Required, Column("language_id", TypeName = Utility.DBType8bitInteger)]
    public byte LanguageId { get; set; }
    public virtual Language Language { get; set; } = null!;
}