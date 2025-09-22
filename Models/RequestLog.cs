using System.ComponentModel.DataAnnotations;


namespace ScriptiumBackend.Models
{

    public class RequestLog
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(126)]
        public required string Identifier { get; set; }

        [Required, MaxLength(126)]
        public required string Endpoint { get; set; }

        [Required, MaxLength(10)]
        public required string Method { get; set; }

        [Required]
        public int StatusCode { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }

}