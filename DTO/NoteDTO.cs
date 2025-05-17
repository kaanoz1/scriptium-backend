using scriptium_backend_dotnet.Models;

namespace DTO
{

    public class NoteOwnDTO
    {
        public required long Id { get; set; }

        public required string Text { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required DateTime? UpdatedAt { get; set; }

        public required int LikeCount { get; set; }

        public required int ReplyCount { get; set; }

        public bool IsLiked { get; set; } = false;

        public required VerseUpperMeanDTO Verse { get; set; }

    }

    public class NoteOwnerDTO : NoteOwnDTO
    {
        public required UserDTO User { get; set; }
    }



    public static class NoteDTOExtension
    {

        public static NoteOwnDTO ToNoteOwnDTO(this Note note, User userRequested)
        {
            return new NoteOwnDTO
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes?.Any(ln => ln.Like != null && ln.Like.UserId == userRequested.Id) ?? default,
                Verse = note.Verse.ToVerseUpperMeanDTO(),
            };
        }

        public static NoteOwnerDTO ToNoteOwnerDTO(this Note note, User UserRequested)
        {
            return new NoteOwnerDTO
            {
                Id = note.Id,
                Text = note.Text,
                User = note.User.ToUserDTO(),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes?.Any(ln => ln.Like != null && ln.Like.UserId == UserRequested.Id) ?? default,
                Verse = note.Verse.ToVerseUpperMeanDTO(),

            };
        }

        public static NoteOwnerDTO ToNoteOwnerDTO(this Note note, bool isLiked)
        {
            return new NoteOwnerDTO
            {
                Id = note.Id,
                Text = note.Text,
                User = note.User.ToUserDTO(),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = isLiked,
                Verse = note.Verse.ToVerseUpperMeanDTO(),

            };
        }
    }
}