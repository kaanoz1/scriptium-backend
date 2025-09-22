using ScriptiumBackend.Models;
/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


namespace DTO
{

    public class NoteOwnDto
    {
        public required long Id { get; set; }

        public required string Text { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required DateTime? UpdatedAt { get; set; }

        public required int LikeCount { get; set; }

        public required int ReplyCount { get; set; }

        public bool IsLiked { get; set; } = false;
        
    }
    
    public class NoteOwnVerseDto : NoteOwnDto
    {

        public required VerseUpperMeanDto Verse { get; set; }
    }

    public class NoteOwnerDto : NoteOwnDto
    {
        public required UserDto Creator { get; set; }
    }
    
    public class NoteOwnerVerseDto : NoteOwnerDto
    {
        public required VerseUpperMeanDto Verse { get; set; }
    }




    public static class NoteDtoExtension
    {

        public static NoteOwnDto ToNoteOwnDto(this Note note, User userRequested)
        {
            return new NoteOwnDto
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
        
        
        public static NoteOwnDto ToNoteOwnDto(this Note note, bool isLiked)
        {
            return new NoteOwnDto
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

        public static NoteOwnerDto ToNoteOwnerDto(this Note note, User UserRequested)
        {
            return new NoteOwnerDto
            {
                Id = note.Id,
                Text = note.Text,
                Creator = note.User.ToUserDto(),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes?.Any(ln => ln.Like != null && ln.Like.UserId == UserRequested.Id) ?? default,

            };
        }

        
        public static NoteOwnVerseDto ToNoteOwnVerseDto(this Note note, User userRequested)
        {
            return new NoteOwnVerseDto
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes.Any(n => n.Like.UserId == userRequested.Id),
                Verse = note.Verse.ToVerseUpperMeanDto()
            };
        }
        
        public static NoteOwnerVerseDto ToNoteOwnerVerseDto(this Note note, User userRequested)
        {
            return new NoteOwnerVerseDto
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = note.Likes.Any(n => n.Like.UserId == userRequested.Id),
                Verse = note.Verse.ToVerseUpperMeanDto(),
                Creator = note.User.ToUserDto()
            };
        }
        
        public static NoteOwnVerseDto ToNoteOwnVerseDto(this Note note, bool isLiked)
        {
            return new NoteOwnVerseDto
            {
                Id = note.Id,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = isLiked,
                Verse = note.Verse.ToVerseUpperMeanDto()
            };
        }
        public static NoteOwnerDto ToNoteOwnerDto(this Note note, bool isLiked)
        {
            return new NoteOwnerDto
            {
                Id = note.Id,
                Text = note.Text,
                Creator = note.User.ToUserDto(),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                LikeCount = note.Likes?.Count ?? 0,
                ReplyCount = note.Comments?.Count ?? 0,
                IsLiked = isLiked,

            };
        }
    }
}

*/