using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class CommentBaseDTO
    {
        public required long Id { get; set; }

        public required string Text { get; set; }

        public required DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public required long LikeCount { get; set; }

        public required long ReplyCount { get; set; }

        public required bool IsLiked { get; set; }
    }

    public class ParentCommentDTO : CommentBaseDTO
    {
        public required UserDTO? User { get; set; }
    }

    public class CommentOwnDTO : CommentBaseDTO
    {
        public ParentCommentDTO? ParentComment { get; set; }
    }

    public abstract class CommentOwnerDTO : CommentOwnDTO
    {
        public required UserDTO Creator { get; set; }
    }

    public class CommentOwnNoteDTO : CommentOwnDTO
    {
        public required NoteOwnDTO Note { get; set; }
    }

    public class CommentOwnVerseDTO : CommentOwnDTO
    {
        public required VerseUpperMeanDTO Verse { get; set; }
    }

    public class CommentOwnerNoteDTO : CommentOwnerDTO
    {
        public required NoteOwnDTO Note { get; set; }
    }

    public class CommentOwnerVerseDTO : CommentOwnerDTO
    {
        public required VerseUpperMeanDTO Verse { get; set; }
    }

    public static class CommentParentUserDTOExtension
    {
        public static CommentBaseDTO ToCommentBaseDTO(this Comment comment)
        {
            return new CommentBaseDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = false
            };
        }

        public static CommentOwnDTO ToCommentOwnDTO(this Comment comment, bool hasPermissionToSeeParentCommentOwnerInformation, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        User = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.User.ToUserDTO()
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

        public static CommentOwnNoteDTO ToCommentOwnNoteDTO(this Comment comment,
            bool hasPermissionToSeeParentCommentOwnerInformation, User userRequested)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnNoteDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Note = comment.CommentNote.Note.ToNoteOwnerDTO(userRequested),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        User = hasPermissionToSeeParentCommentOwnerInformation
                            ? comment.ParentComment.User.ToUserDTO()
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

        public static CommentOwnNoteDTO ToCommentOwnNoteDTO(this Comment comment, bool isLiked)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnNoteDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Note = comment.CommentNote.Note.ToNoteOwnerDTO(isLiked),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        IsLiked = isLiked,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDTO(),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnVerseDTO ToCommentOwnVerseDTO(this Comment comment, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnVerseDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDTO(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDTO(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnVerseDTO ToCommentOwnVerseDTO(this Comment comment, bool isLiked, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnVerseDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDTO(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDTO(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerNoteDTO ToCommentOwnerNoteDTO(this Comment comment, User userRequested)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnerNoteDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                Creator = comment.User.ToUserDTO(),
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Note = comment.CommentNote.Note.ToNoteOwnerDTO(userRequested),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDTO(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),

                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerNoteDTO ToCommentOwnerNoteDTO(this Comment comment, bool isLiked, User userRequested)
        {
            if (comment.CommentNote == null) throw new ArgumentNullException();

            return new CommentOwnerNoteDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Creator = comment.User.ToUserDTO(),
                Note = comment.CommentNote.Note.ToNoteOwnerDTO(isLiked),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDTO(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerVerseDTO ToCommentOwnerVerseDTO(this Comment comment, User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnerVerseDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                Creator = comment.User.ToUserDTO(),
                IsLiked = comment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDTO(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        User = comment.ParentComment.User.ToUserDTO(),
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }

        public static CommentOwnerVerseDTO ToCommentOwnerVerseDTO(this Comment comment, bool isLiked,
            User userRequested)
        {
            if (comment.CommentVerse == null) throw new ArgumentNullException();

            return new CommentOwnerVerseDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt ?? default,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                IsLiked = isLiked,
                Creator = comment.User.ToUserDTO(),
                Verse = comment.CommentVerse.Verse.ToVerseUpperMeanDTO(),
                ParentComment = comment.ParentComment != null
                    ? new ParentCommentDTO
                    {
                        Id = comment.ParentComment.Id,
                        Text = comment.ParentComment.Text,
                        CreatedAt = comment.ParentComment.CreatedAt,
                        UpdatedAt = comment.ParentComment.UpdatedAt,
                        IsLiked = comment.ParentComment.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id),
                        User = comment.ParentComment.User.ToUserDTO(),
                        LikeCount = comment.ParentComment.LikeCount,
                        ReplyCount = comment.ParentComment.ReplyCount
                    }
                    : null
            };
        }
    }
}