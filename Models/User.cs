using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using scriptium_backend_dotnet.Models.Util;


namespace scriptium_backend_dotnet.Models
{
    [Table("user")]
    public class User : IdentityUser<Guid>
    {

        [Required, MaxLength(30)]
        public required string Name { get; set; } = null!;

        [MaxLength(30)]
        public string? Surname { get; set; } = null!;

        public byte[]? Image { get; set; } = null!;

        [MaxLength(1)]
        public string? Gender { get; set; }

        [MaxLength(256)]
        public string? Biography { get; set; }

        public DateTime? EmailVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastActive { get; set; }

        public DateTime? IsFrozen { get; set; }

        public DateTime? IsPrivate { get; set; } = DateTime.UtcNow;

        [Column("preferred_languageId", TypeName = Utility.DBType8bitInteger)]
        public byte PreferredLanguageId { get; set; } = 1;

        [ForeignKey("PreferredLanguageId")]
        public virtual Language PreferredLanguage { get; set; } = null!;

        public virtual List<Session>? Sessions { get; set; }

        [NotMapped]
        public int SessionCount => Sessions?.Count ?? 0;

        public virtual List<Collection>? Collections { get; set; }

        [NotMapped]
        public int CollectionCount => Collections?.Count ?? 0;

        public virtual List<Note>? Notes { get; set; }

        [NotMapped]
        public int NoteCount => Notes?.Count ?? 0;

        public virtual List<Comment>? Comments { get; set; }

        [NotMapped]
        public int CommentCount => Comments?.Count ?? 0;

        public virtual List<Follow>? Followers { get; set; }


        [NotMapped]
        public int FollowerCount => Followers?.Count ?? 0;

        public virtual List<Follow>? Followings { get; set; }

        [NotMapped]
        public int FollowingCount => Followings?.Count ?? 0;

        public virtual List<FollowR>? FollowerRs { get; set; }

        public virtual List<FollowR>? FollowRing { get; set; }

        public virtual List<Block>? BlockedUsers { get; set; }

        public virtual List<Block>? BlockedByUsers { get; set; }

        public virtual List<FreezeR>? FreezeRecords { get; set; }

        public virtual List<Like>? Likes { get; set; }

        [NotMapped]
        public int LikeCount => Likes?.Count ?? 0;

        public virtual List<Notification>? NotificationsReceived { get; set; }

        public virtual List<Notification>? NotificationsSent { get; set; }

        public virtual List<Suggestion>? Suggestions { get; set; }

        [NotMapped]
        public int SuggestionCount => Suggestions?.Count ?? 0;

        public virtual List<UserUpdateR>? UpdateRecords { get; set; }

        [NotMapped]
        public int UpdateCount => UpdateRecords?.Count ?? 0;

    }
}
