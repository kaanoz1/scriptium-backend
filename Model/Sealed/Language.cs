using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Sealed;

[Table("s_language")]
public sealed class Language
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; init; }

    [Required, StringLength(50), Column("name")]
    public required string Name { get; init; }

    [Required, StringLength(50), Column("english_name")]
    public required string EnglishName { get; init; }

    [Required, StringLength(2), Column("code")]
    public required string Code { get; init; }
}