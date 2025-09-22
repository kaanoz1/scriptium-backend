using ScriptiumBackend.Models;
/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.



namespace DTO
{
    public class CommentBaseDto
    {
        public required long Id { get; set; }

        public required string Text { get; set; }

        public required DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public required long LikeCount { get; set; }

        public required long ReplyCount { get; set; }

        public required bool IsLiked { get; set; }
    }

    public class ParentCommentDto : CommentBaseDto
    {
        public required UserDto? User { get; set; }
    }

    public class CommentOwnDto : CommentBaseDto
    {
        public ParentCommentDto? ParentComment { get; set; }
    }

    public class CommentOwnerDto : CommentOwnDto
    {
        public required UserDto Creator { get; set; }
    }

    public class CommentOwnNoteDto : CommentOwnDto
    {
        public required NoteOwnDto Note { get; set; }
    }

    public class CommentOwnVerseDto : CommentOwnDto
    {
        public required VerseUpperMeanDto Verse { get; set; }
    }

    public class CommentOwnerNoteDto : CommentOwnerDto
    {
        public required NoteOwnDto Note { get; set; }
    }

    public class CommentOwnerVerseDto : CommentOwnerDto
    {
        public required VerseUpperMeanDto Verse { get; set; }
    }

    public static class CommentParentUserDtoExtension
    {
        public static CommentBaseDto ToCommentBaseDto(this Comment comment)
        {
            return new CommentBaseDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = false
            };
        }

        public static CommentOwnDto ToCommentOwnDto(this Comment comment, bool hasPermissionToSeeParentCommentOwnerInformation, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        User = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.User.ToUserDto()
                            : default,
                        LikeCount = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.LikeCount
                            : -1,
                        ReplyCount = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.ReplyCount
                            : -1
                    }
                    : null
            };
        }

        public static CommentOwnerDto ToCommentOwnerDto(this Comment comment, bool hasPermissionToSeeParentCommentOwnerInformation, User userRequested)
        {
            if (comment.CommentVerseId == null) throw new ArgumentNullException();

            return new CommentOwnerDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                Creator = comment.User.ToUserDto(),
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        User = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.User.ToUserDto()
                            : default,
                        LikeCount = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.LikeCount
                            : -1,
                        ReplyCount = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.ReplyCount
                            : -1
                    }
                    : null
            };
        }

        public static CommentOwnNoteDto ToCommentOwnNoteDto(this Comment comment,
            bool hasPermissionToSeeParentCommentOwnerInformation, User userRequested)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnNoteDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Note = comment.CommentNote.Note.ToNoteOwnerDto(userRequested),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        User = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.User.ToUserDto()
                            : default,
                        LikeCount = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.LikeCount
                            : -1,
                        ReplyCount = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.ReplyCount
                            : -1
                    }
                    : null
            };
        }

        public static CommentOwnNoteDto ToCommentOwnNoteDto(this Comment comment, bool isLiked)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnNoteDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Note = comment.CommentNote.Note.ToNoteOwnerDto(isLiked),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        IsLiked = isLiked,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDto(),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnVerseDto ToCommentOwnVerseDto(this Comment comment, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnVerseDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDto(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDto(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnVerseDto ToCommentOwnVerseDto(this Comment comment, bool isLiked, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnVerseDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDto(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDto(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerNoteDto ToCommentOwnerNoteDto(this Comment comment, User userRequested)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnerNoteDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                Creator = comment.User.ToUserDto(),
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Note = comment.CommentNote.Note.ToNoteOwnerDto(userRequested),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDto(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),

                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerNoteDto ToCommentOwnerNoteDto(this Comment comment, bool isLiked, User userRequested)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnerNoteDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Creator = comment.User.ToUserDto(),
                Note = comment.CommentNote.Note.ToNoteOwnerDto(isLiked),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDto(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerVerseDto ToCommentOwnerVerseDto(this Comment comment, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnerVerseDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                Creator = comment.User.ToUserDto(),
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDto(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDto(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerVerseDto ToCommentOwnerVerseDto(this Comment comment, bool isLiked,
            User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnerVerseDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Creator = comment.User.ToUserDto(),
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDto(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDto
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        User = comment.ParentComment.User.ToUserDto(),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }
    }
}
*/