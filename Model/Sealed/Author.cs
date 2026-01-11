using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScriptiumBackend.Model.Abstract;

namespace ScriptiumBackend.Model.Sealed;

[Table("s_author")]
public sealed class Author
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public required long Id { get; init; }

    [Column("name"), Required, StringLength(1000)]
    public required string Name { get; init; }

    [Column("language"), ForeignKey("s_language_id")]
    public required Language Language { get; init; }

    [Column("description"), StringLength(1000)]
    public required string? Description { get; init; }

    [Column("url"), StringLength(1000)] public required string? Url { get; init; }

    public required List<Meaning> NameTranslations { get; init; }

    public required List<Translation> Translations { get; init; }
}