using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

[Table("i_h_ao_book_category")]
public class BookCategory
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required BookCategoryKind Id { get; init; }

    [Required, Column("name")] public required string Name { get; init; }

    public List<Book> Books { get; init; } = [];
}
