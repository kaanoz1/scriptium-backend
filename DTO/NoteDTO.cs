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
        
    }
    
    public class NoteOwnVerseDTO : NoteOwnDTO
    {

        public required VerseUpperMeanDTO Verse { get; set; }
    }

    public class NoteOwnerDTO : NoteOwnDTO
    {
        public required UserDTO Creator { get; set; }
    }
    
    public class NoteOwnerVerseDTO : NoteOwnerDTO
    {
        public required VerseUpperMeanDTO Verse { get; set; }
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
            };
        }
        
        
        public static NoteOwnDTO ToNoteOwnDTO(this Note note, bool isLiked)
        {
            return new NoteOwnDTO
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = isLiked,
            };
        }

        public static NoteOwnerDTO ToNoteOwnerDTO(this Note note, User UserRequested)
        {
            return new NoteOwnerDTO
            {
                Id = note.Id,
                Text = note.Text,
                Creator = note.User.ToUserDTO(),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes?.Any(ln => ln.Like != null && ln.Like.UserId == UserRequested.Id) ?? default,

            };
        }

        
        public static NoteOwnVerseDTO ToNoteOwnVerseDTO(this Note note, User userRequested)
        {
            return new NoteOwnVerseDTO
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes.Any(n => n.Like.UserId == userRequested.Id),
                Verse = note.Verse.ToVerseUpperMeanDTO()
            };
        }
        
        public static NoteOwnerVerseDTO ToNoteOwnerVerseDTO(this Note note, User userRequested)
        {
            return new NoteOwnerVerseDTO
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes.Any(n => n.Like.UserId == userRequested.Id),
                Verse = note.Verse.ToVerseUpperMeanDTO(),
                Creator = note.User.ToUserDTO()
            };
        }
        
        public static NoteOwnVerseDTO ToNoteOwnVerseDTO(this Note note, bool isLiked)
        {
            return new NoteOwnVerseDTO
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = isLiked,
                Verse = note.Verse.ToVerseUpperMeanDTO()
            };
        }
        public static NoteOwnerDTO ToNoteOwnerDTO(this Note note, bool isLiked)
        {
            return new NoteOwnerDTO
            {
                Id = note.Id,
                Text = note.Text,
                Creator = note.User.ToUserDTO(),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = isLiked,

            };
        }
    }
}