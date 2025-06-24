using System.Security.Claims;
using AngleSharp.Dom;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using scriptium_backend_dotnet.Controllers.Validation;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Models;
using scriptium_backend_dotnet.Models.Util;

namespace scriptium_backend_dotnet.Controllers.LikeHandler
{
    [ApiController, Route("like"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class LikeController(ApplicationDBContext db, UserManager<User> userManager, ILogger<LikeController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<LikeController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        [HttpGet, Route("likes")]
        public async Task<IActionResult> GetLikes()
        {

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            User? userRequested = await _userManager.FindByIdAsync(userId);

            if (userRequested == null)
                return NotFound(new { message = "User not found." });

            try
            {
                List<CommentOwnVerseDTO> verseComments = await _db.Comment
                   .Where(c => c.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id) && c.CommentVerse != null)
                    .Include(c => c.CommentVerse).ThenInclude(cv => cv!.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language) //cv is not null. Checked in Where()
                    .Include(c => c.CommentVerse).ThenInclude(cv => cv!.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)  //cv is not null. Checked in Where()
                    .Include(c => c.CommentVerse).ThenInclude(cv => cv!.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)  //cv is not null. Checked in Where()
                    .AsSplitQuery() 
                    .Select(comment => comment.ToCommentOwnVerseDTO(true, userRequested)) //Already checked in Where()
                    .ToListAsync();

                List<CommentOwnNoteDTO> noteComments = await _db.Comment
                   .Where(c => c.LikeComments.Any(lc => lc.Like.UserId == userRequested.Id) && c.CommentNote != null)
                    .Include(c => c.CommentNote).ThenInclude(cn => cn!.Note).ThenInclude(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)  //cn is not null. Checked in Where()
                    .Include(c => c.CommentNote).ThenInclude(cn => cn!.Note).ThenInclude(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)  //cn is not null. Checked in Where()
                    .Include(c => c.CommentNote).ThenInclude(cn => cn!.Note).ThenInclude(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)  //cn is not null. Checked in Where()
                    .AsSplitQuery()
                    .Select(comment => comment.ToCommentOwnNoteDTO(true)) //Already checked in Where()
                    .ToListAsync();

                List<NoteOwnDTO> notes = await _db.Note
                   .Where(n => n.Likes.Any(ln => ln.Like.UserId == userRequested.Id))
                    .Include(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                    .Include(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                    .Include(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                    .AsSplitQuery()
                    .Select(n => n.ToNoteOwnDTO(true)) //Already checked in Where()
                     .ToListAsync();


                _logger.LogInformation($"Operation completed: User: [Id: {userRequested.Id}, Username: {userRequested.UserName}] has demanded his like records. C: {verseComments.Count} + NC {noteComments.Count} + N: {notes.Count} row has ben returned.");

                return Ok(new { data = new { verseComments, noteComments, notes } });
            }
            catch (Exception)
            {

                _logger.LogError($"Error occurred, while: User: [Id: {userRequested.Id}, Username: {userRequested.UserName}] is demanding like records.");

                return BadRequest(new { message = "Something went unexpectedly wrong?" });

            }
        }


        [HttpPost, Route("note")]
        public async Task<IActionResult> LikeNote([FromBody] NoteIdentifierModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                Note? NoteLiked = await _db.Note.FirstOrDefaultAsync(n => n.Id == model.NoteId);

                if (NoteLiked == null)
                {
                    _logger.LogWarning($"NotFound, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to Like Claimed Note: [model.NoteId: {model.NoteId}]");
                    return NotFound(new { message = "Note not found." });
                }

                bool isAlreadyLiked = await _db.LikeNote.Include(c => c.Like).AnyAsync(lc =>
                    lc.NoteId == NoteLiked.Id &&
                    lc.Like != null &&
                    lc.Like.UserId == UserRequested.Id
                );

                if (isAlreadyLiked)
                {
                    _logger.LogWarning($"Conflict, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to like Comment: [Id: {NoteLiked.Id}] User already liked this comment.");
                    return Conflict(new { message = "You have already liked this comment!" });
                }

                bool isFollowingOwner = await _db.Follow.AnyAsync(f => f.FollowerId == UserRequested.Id && f.FollowedId == NoteLiked.UserId && f.Status == FollowStatus.Accepted);

                if (!isFollowingOwner && UserRequested.Id != NoteLiked.UserId)
                {
                    _logger.LogWarning($"Unauthorized, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to like Note: [Id: {NoteLiked.Id}, NoteOwnerUsername: {NoteLiked.User.UserName}]");
                    return Unauthorized(new { message = "You do not have permission do like this note." });
                }


                Like LikeCreated = new()
                {
                    UserId = UserRequested.Id,
                };

                LikeNote LikeNoteCreated = new()
                {
                    Like = LikeCreated,
                    Note = NoteLiked,
                    NoteId = NoteLiked.Id
                };

                _db.Like.Add(LikeCreated);
                _db.LikeNote.Add(LikeNoteCreated);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation($"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has successfully liked Note: [Id: {NoteLiked.Id}, NoteOwnerUsername: {NoteLiked.User.UserName}]");
                return Ok(new { message = "Note is successfully liked" });
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();

                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to like Note: [Id: {model.NoteId}]");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }

        [HttpPost, Route("comment/note")]
        public async Task<IActionResult> LikeCommentOnNote([FromBody] CommentLikeProcessModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });



            try
            {
                Note? NoteTarget = await _db.Note.FirstOrDefaultAsync(n => n.Id == model.EntityId);

                if (NoteTarget == null)
                {
                    _logger.LogWarning($"NotFound, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to Like Comment On Claimed Note: [model.EntityId: {model.EntityId}]");
                    return NotFound(new { message = "Note not found." });
                }

                Comment? CommentLiked = await _db.Comment.FirstOrDefaultAsync(c => c.Id == model.CommentId && c.CommentNote != null && c.CommentNote.NoteId == NoteTarget.Id);

                if (CommentLiked == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to like Claimed Comment: [model.CommentId: {model.CommentId}] on Comment/Note");
                    return NotFound(new { message = "Comment not found." });
                }

                HashSet<long> LikeableNoteCommentIds = _db.GetAvailableNoteCommentIds(UserRequested.Id, NoteTarget.Id);

                bool isFollowing = await _db.Follow.AnyAsync(f => f.FollowerId == UserRequested.Id && f.FollowedId == NoteTarget.UserId && f.Status == FollowStatus.Accepted);

                if (!(isFollowing && LikeableNoteCommentIds.Contains(CommentLiked.Id)))
                {
                    _logger.LogWarning($"Unauthorized, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to like Comment: [Id: {CommentLiked.Id}, CommentOwnerUsername: {CommentLiked.User.UserName}] on Note: [Id: {NoteTarget.Id}, NoteOwnerUsername: {NoteTarget.User.UserName}]. User does not have permission to like.");
                    return Unauthorized(new { message = "You do not have permission to attach comment to this note" });
                }

                Like LikeCreated = new()
                {
                    UserId = UserRequested.Id,
                };

                LikeComment LikeNoteCreated = new()
                {
                    LikeId = LikeCreated.Id,
                    CommentId = CommentLiked.Id
                };

                _db.Like.Add(LikeCreated);

                _db.LikeComment.Add(LikeNoteCreated);

                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed.  User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has successfully liked Comment: [Id: {CommentLiked.Id}, CommentOwnerUsername: {CommentLiked.User.UserName}] on Note: [Id: {NoteTarget.Id}, NoteOwnerUsername: {NoteTarget.User.UserName}]");
                return Ok(new { message = "You have successfully liked the comment on this specified note!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to like Comment: [Id: {model.CommentId}] on Note: [Id: {model.EntityId}]. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }

        [HttpPost, Route("comment/verse")]
        public async Task<IActionResult> LikeCommentVerse([FromBody] CommentLikeProcessModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            User? userRequested = await _userManager.FindByIdAsync(userId);

            if (userRequested == null)
                return NotFound(new { message = "User not found." });

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                Verse? VerseTarget = await _db.Verse.FirstOrDefaultAsync(n => n.Id == model.EntityId);

                if (VerseTarget == null)
                    return NotFound(new { message = "Verse not found." });

                Comment? CommentLiked = await _db.Comment.FirstOrDefaultAsync(c => c.Id == model.CommentId && c.CommentVerse != null && c.CommentVerse.VerseId == VerseTarget.Id);

                if (CommentLiked == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {userRequested.Id}, Username: {userRequested.UserName}] is trying to like Claimed Comment: [model.CommentId: {model.CommentId}] on Verse: [Id: {VerseTarget.Id}]");
                    return NotFound(new { message = "Comment not found." });
                }

                bool isAlreadyLiked = await _db.LikeComment.Include(c => c.Like).AnyAsync(lc =>
                    lc.CommentId == CommentLiked.Id &&
                    lc.Like != null &&
                    lc.Like.UserId == userRequested.Id
                );

                if (isAlreadyLiked)
                {
                    _logger.LogWarning($"Conflict, while: User: [Id: {userRequested.Id}, Username: {userRequested.UserName}] is trying to like Comment: [Id: {CommentLiked.Id} on Verse: [Id: {VerseTarget.Id}]. User already liked this comment.");
                    return Conflict(new { message = "You have already liked this comment!" });
                }

                HashSet<long> LikeableVerseCommentIds = _db.GetAvailableVerseCommentIds(userRequested, VerseTarget.Id);

                if (!LikeableVerseCommentIds.Contains(CommentLiked.Id))
                {
                    _logger.LogWarning($"Unauthorized, while: User: [Id: {userRequested.Id}, Username: {userRequested.UserName}] is trying to like Comment: [Id: {CommentLiked.Id}, CommentOwnerUsername: {CommentLiked.User.UserName}] on Verse: [Id: {VerseTarget.Id}] User does not have permission to like.");
                    return Unauthorized(new { message = "You do not have permission to attach comment to this note" });
                }

                Like LikeCreated = new()
                {
                    UserId = userRequested.Id,
                };

                LikeComment LikeCommentCreated = new()
                {
                    CommentId = CommentLiked.Id,
                    Comment = CommentLiked,
                    Like = LikeCreated
                };

                _db.Like.Add(LikeCreated);
                _db.LikeComment.Add(LikeCommentCreated);

                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed.  User: [Id: {userRequested.Id}, Username: {userRequested.UserName}] has successfully liked Comment: [Id: {CommentLiked.Id}] on Verse: [Id: {VerseTarget.Id}]");

                await transaction.CommitAsync();

                return Ok(new { message = "You have successfully liked the comment!" });
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error occurred, while: User: [Id: {userRequested.Id}, Username: {userRequested.UserName}] is trying to like Comment: [Id: {model.EntityId}. Error Details: {ex}");


                await transaction.RollbackAsync();

                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }


        [HttpDelete, Route("unlike/note")]
        public async Task<IActionResult> UnlikeNote([FromBody] NoteIdentifierModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            Like? LikeDeleted = null;

            try
            {
                LikeDeleted = await _db.Like.FirstOrDefaultAsync(l => l.UserId == UserRequested.Id && l.LikeNote != null && l.LikeNote.NoteId == model.NoteId);

                if (LikeDeleted == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to remove on Note: [model.NoteId: {model.NoteId}]");
                    return NotFound(new { message = "There is no 'like' attached this note." });
                }

                _db.Like.Remove(LikeDeleted);

                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has removed his Like: [Id: {LikeDeleted.Id}] on Note");
                return Ok(new { message = "Like that attached this note is successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to remove Like: [Id: {LikeDeleted?.Id}] on Note. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }

        [HttpDelete, Route("unlike/comment")]
        public async Task<IActionResult> UnlikeVerse([FromBody] CommentLikeProcessModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            Like? LikeDeleted = null;

            try
            {
                LikeDeleted = await _db.Like.FirstOrDefaultAsync(l => l.UserId == UserRequested.Id && l.LikeComment != null && l.LikeComment.CommentId == model.CommentId);

                if (LikeDeleted == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to remove on Claimed Comment: [model.CommentId: {model.CommentId}]");
                    return NotFound(new { message = "There is no 'like' attached this comment." });
                }

                _db.Like.Remove(LikeDeleted);


                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has removed his Like: [Id: {LikeDeleted.Id}] on Comment");

                return Ok(new { message = "Like that attached this comment is successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to remove Like: [Id: {LikeDeleted?.Id}] on Comment. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }
    }
}