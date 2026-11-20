using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

[Table("i_h_ao_book")]
public class Book
{
    // Id's are not auto-generated because the source has them as a single sequence across all books.
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, Column("name")] public required string Name { get; init; }

    [Column("author_no")] public required int AuthorNo { get; init; }

    [Required, Column("author_name")] public required string AuthorName { get; init; }

    /// <summary>Hijri year.</summary>
    [Column("author_death_year")] public int? AuthorDeathYear { get; init; }

    [Column("city")] public string? City { get; init; }

    [Column("publisher")] public string? Publisher { get; init; }

    [Column("edition")] public string? Edition { get; init; }

    [Required, ForeignKey(ForeignKeys.Category)]
    public required BookCategory Category { get; init; }

    [Column("hadith_count")] public required int HadithCount { get; init; }

    public List<Hadith> Hadiths { get; init; } = [];
}
