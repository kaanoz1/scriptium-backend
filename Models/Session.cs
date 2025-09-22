/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Models
{
    [Table("session"), NotMapped]
    public class Session
    {
        [Key]
        public string Key { get; set; } = null!;

        // [Required, Column(TypeName = Utility.DBTypeVARBINARYMAX)] //TODO:Binary Data
        // public byte[] Data { get; set; } = null!; 

        public required Guid UserId { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

}

*/