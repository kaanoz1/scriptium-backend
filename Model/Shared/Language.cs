using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Shared;

[Table("s_language")]
public class Language
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; set; }

    [Required, StringLength(50), Column("name")]
    public required string Name { get; set; } = string.Empty;

    [Required, StringLength(50), Column("english_name")]
    public required string EnglishName { get; set; } = string.Empty;

    [Required, StringLength(2), Column("code")]
    public required string Code { get; set; } = string.Empty;
}