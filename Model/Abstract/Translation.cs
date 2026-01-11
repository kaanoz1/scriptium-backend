using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ScriptiumBackend.Model.Sealed;

namespace ScriptiumBackend.Model.Abstract;

[Table("a_translation")]
public abstract class Translation
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required short Id { get; init; }


    [Required, StringLength(100), Column("name")]
    public required string Name { get; init; }


    [StringLength(100), Column("description"), AllowNull]
    public required string Description { get; init; }


    [Required, Column("published_at")] public required DateOnly Date { get; init; }

    public required List<Author> Authors { get; init; }

    [Required, ForeignKey("s_language_id")]
    public required Language Language { get; init; }
}