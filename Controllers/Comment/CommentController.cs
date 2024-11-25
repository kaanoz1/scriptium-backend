using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using writings_backend_dotnet.Controllers.Validation;
using writings_backend_dotnet.DB;
using writings_backend_dotnet.DTOs;
using writings_backend_dotnet.Models;
using writings_backend_dotnet.Models.Util;

/**
The Writings project has an unusual comment system in line with the project purpose. With the purpose of represent, you can inspect the diagram below in which capital letters represent users and symbols prefixed with "c" represents the comment made my corresponding users. Besides that, capital letters with sign ~ indicates that his users account is private. Finally, the parenthesis rightmost represent if external user is able to see relevant comment.
For instance, let suppose that external user is A and comment schema looks like this:

cA(+)
     cB~(+)

Implies that there is top comment which is cA, and a comment whom wants to reply that one which is cB with private account, and external user which is user A can see both.

In Writings comment system grounds that statements:
1. Users can see the comment made by ones they follow.
2. Users can see their comments.
3. Users cannot see replies to their comments unless the replier’s account is private and the user does not follow the replier.
4. Users can reply comments whichsoever they see.

For demonstrating the comments system the writings project has, there are 2 state following.

State 1.

Lets suppose that external user is X and user V followed by user K and Z. User X follows user V and Z.

cV(+) 1*
     cX(+) 2*
     cK(-) 3*
          cV(-) 4*
     cZ(+) 5*
          cV(+) 1*

1*: User X can see this comment because he/she follows user V.
2*: User X can see this comment as it is his/her comment.
3*: User X cannot see this comment because he/she does NOT follow user K.
4*: User X cannot see this comment despite to following user V, since this comment attached the comment belongs to user V does not follow.
5*: User X can see this comment because user Z is followed by user X.

State 2:

Let's assume that user X is the external user. And user X follows no one.

cX(+) 1*
     cK(+) 2*
          cK(-) 3*
          cX(+) 4*
    cV(+) 5*
          cX(+) 6*
     cS~(-) 7*
           cV(-) 8*

1*: User X can see this comment as this comment belongs to his/herself
2*: User X can see this comment even if he/she does NOT follow user K. Because this comment attached on his own comment and user K does not have private account.
3*: User X cannot see this comment. Because of not following the user K, it does not matter that user X can see comment number 2.
4*: User X can see this comment because the reason mentioned for comment number 1.
5*: User X can see this comment because of following the user V.
6*: User X can see this comment because the reason mentioned for comment number 1.
7*: User X cannot see this comment even if this comment replies his comment. Because user S has private account and relevant user does not follow user S.
8*: User X cannot see this comment because he does not follow user S.

As for notes. Notes considered top comment for user whoever belongs to. (Will be implemented)
 */

namespace writings_backend_dotnet.Controllers.CommentHandler
{

    /// <summary>
    /// This controller consists of common operations for the 'Comment' model. This handler is also marked as "Authorize" because all operations this controller own require Authentication. For comprehensive information please check the following:
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item> 
    ///         <term>URL: ""</term>
    ///         
    ///         <c>GetComments()</c>
    ///         
    ///         <description>This function brings all comments requested user made as well as their parent if it does exist. Additionally, if the parent comment owner's account is private, furthermore, the requested user does not follows him/her related user info will be concealed. If he does, it will be not. Moreover that, it brings the entity information whichsoever it attached to. For now, it could be Verse or Note, check respectively: <see cref="Verse"/> and <see cref="Note"/> classes. </description>
    ///     </item>
    ///     
    ///     <item> 
    ///         <term>URL: "note/{model.NoteId}"</term>
    ///         
    ///         <c>GetCommentFromNote([FromRoute] NoteIdentifierModel model)</c>
    ///         
    ///         <description>This function brings all comments which corresponding note and requested user has permission to see. You can check the comment policy of this project above.  </description>
    ///     </item>
    ///     
    ///     <item> 
    ///         <term>URL: "verse/{model.ScriptureNumber}/{model.SectionNumber}/{model.ChapterNumber}/{model.VerseNumber}"</term>
    ///         
    ///         <c>GetCommentFromVerse([FromRoute] VerseValidatedModel model)</c>
    ///         
    ///         <description>This function brings all comments which attached to corresponding verse and requested user has permission to see. You can check the comment policy of this project above.  </description>
    ///     </item>
    ///     
    ///     <item> 
    ///         <term>URL: "create/verse"</term>
    ///         
    ///         <c>CreateCommentOnVerse([FromBody] EntityCommentCreateModel model)</c>
    ///         
    ///         <description>Allow for users to create comment on specified verse. Users can reply comment whichever they have permission to do.</description>
    ///     </item>
    ///     
    ///     <item> 
    ///         <term>URL: "create/note"</term>
    ///         
    ///         <c>CreateCommentOnNote([FromBody] EntityCommentCreateModel model)</c>
    ///         
    ///         <description>Allow for users to create comment on specified note. Users can reply comment whichever they have permission to do. In addition to create/verse, users must follow the owner of the note in order to comment.</description>
    ///     </item>
    ///     
    ///     <item> 
    ///         <term>URL: "update"</term>
    ///         
    ///         <c>UpdateComment([FromBody] CommentUpdateModel model)</c>
    ///         
    ///         <description>This endpoint, as the name implies, allow users to update their comments. Users has permission to alter their comment's text only.</description>
    ///     </item>
    ///     
    ///     <item>
    ///         <term>URL: "delete"</term>
    ///         
    ///         <c>DeleteComment([FromBody] CommentDeleteModel model)</c>
    ///         
    ///         <description>This endpoint, as is evident from its name, intercede for deleting the comments of a user. This is a RECURSIVE process, as it might be understood, the other comments which subtly connected the comment to be deleted also will be deleted.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    [ApiController, Route("comment"), Authorize]
    public class CommentController(ApplicationDBContext db, UserManager<User> userManager, ILogger<CommentController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<CommentController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// The endpoint which bring information about all comments user made. This handler is does not provide public information, implying private for each user individually.
        /// </summary>
        /// <returns>
        /// - 200 OK if the comments found and served to user successfully.
        /// - 401 Unauthorized if the user is not authenticated or lacks permissions.
        /// </returns>
        [HttpGet, Route("")]
        public async Task<IActionResult> GetComments()
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });

            HashSet<Guid> FollowedUserIds = _db.Follow
              .Where(f => f.FollowerId == UserRequested.Id && f.Followed.IsPrivate.HasValue && f.Status == FollowStatus.Accepted)
              .Select(f => f.FollowedId)
              .ToHashSet();

            List<CommentParentCommentDTO>? data = await _db.Comment
                                            .Where(c => c.UserId == UserRequested.Id)
                                            .Select(c => c.ToCommentParentUserDTO(c.ParentComment != null && FollowedUserIds.Contains(c.ParentComment.UserId))) //Does UserRequest follow the parent comment owner?
                                            .ToListAsync();

            _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded his comments records. {data.Count} row has ben returned.");

            return Ok(new { data });
        }


        /// <summary>
        /// Serves comment attached on note user specified by param: model.NoteId. Users can only see comments which observe the policy mentioned in the top of that file. For Validation model: <see cref="NoteIdentifierModel"/>
        /// </summary>
        /// <param name="model.NoteId"> Indicates that Id property of corresponding Note entity. </param>
        /// <returns>
        /// - 200 OK if the comments found and served to user successfully.
        /// - 401 Unauthorized if the user is not authenticated or does not follow the user who own the specified note.
        /// - 404 Not Found if the Note which uses claimed to have access and has specified identifier is not found.
        /// </returns>
        [HttpGet, Route("note/{NoteId}")]
        public async Task<IActionResult> GetCommentFromNote([FromRoute] NoteIdentifierModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                Note? NoteTarget = await _db.Note.FirstOrDefaultAsync(n => n.Id == model.NoteId);

                if (NoteTarget == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get comments attached Note: [model.NoteId: {model.NoteId}]. Note not found.");
                    return NotFound(new { Message = "Note not found." });
                }

                bool isFollowing = await _db.Follow.AnyAsync(f => f.FollowerId == UserRequested.Id && f.FollowedId == NoteTarget.UserId && f.Status == FollowStatus.Accepted);

                if (!isFollowing)
                {
                    _logger.LogWarning($"Unauthorize, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get comments attached Note: [Id: {NoteTarget.Id}, NoteTarget.UserId: {NoteTarget.UserId}, NoteTarget.User.UserName: {NoteTarget.User.UserName}]. User does not follow the owner.");

                    return Unauthorized(new { Message = "You do not have permission to attach comment to this note" });
                }

                List<GetCommentDTO> data = await _db.GetNoteCommentsAsync(UserRequested.Id, NoteTarget.Id);

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded comments on attached Note: [Id: {NoteTarget.Id}, NoteTarget.UserId: {NoteTarget.UserId}, NoteTarget.User.UserName: {NoteTarget.User.UserName}]. {data.Count} row has ben returned.");


                return Ok(new { data });
            }
            catch (Exception)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get comments attached Note: [model.NoteId: {model.NoteId}] Error Details: {model.NoteId}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }

        }

        /// <summary>
        /// Serves comment attached on verse. Users can only see comments which observe the policy mentioned in the top of that file. For Validation model: <see cref="VerseValidatedModel"/>
        /// </summary>
        /// <param name="model.ScriptureNumber">Indicate the number of the scripture which corresponding section conform and user wants to see comment attached to it.</param>
        /// <param name="model.SectionNumber">Indicate the number of the section which corresponding chapter conform and user wants to see comment attached to it.</param>
        /// <param name="model.ChapterNumber">Indicate the number of the chapter which corresponding verse conform and user wants to see comment attached to it.</param>
        /// <param name="model.VerseNumber">Indicate the number of the verse which user wants to see comment attached to it.</param>
        /// <returns>
        /// - 200 OK if the comment is created successfully.
        /// - 401 Unauthorized if the user is not authenticated.
        /// - 404 Not Found if the Verse does not exists.
        /// </returns>
        [HttpGet, Route("verse/{ScriptureNumber}/{SectionNumber}/{ChapterNumber}/{VerseNumber}")]
        public async Task<IActionResult> GetCommentFromVerse([FromRoute] VerseValidatedModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();


            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                Verse? VerseTarget = await _db.Verse
                          .FirstOrDefaultAsync(v => v.Number == model.VerseNumber &&
                                      v.Chapter.Number == model.ChapterNumber &&
                                      v.Chapter.Section.Number == model.SectionNumber &&
                                      v.Chapter.Section.Scripture.Number == model.ScriptureNumber);

                if (VerseTarget == null)
                    return NotFound(new { Message = "Verse not found." });

                List<GetCommentDTO> data = await _db.GetVerseCommentsAsync(UserRequested.Id, VerseTarget.Id);
                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded comments on attached Verse: [Id: {VerseTarget.Id},]. {data.Count} row has ben returned.");

                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get comments attached on Verse: [model.VerseNumber: {model.VerseNumber}, model.ChapterNumber: {model.ChapterNumber}, model.SectionNumber: {model.SectionNumber}, model.ScriptureNumber: {model.ScriptureNumber}] Error Details: {ex}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }


        }

        /// <summary>
        /// As the name implies, allow users to create comment on specified note. Users can reply the comment only they see. For Validation model: <see cref="EntityCommentCreateModel"/>
        /// </summary>
        /// <param name="model.EntityId">The Entity Identifier which demanded to attach comment. </param>
        /// <param name="model.CommentText">The text of the comment which demanded to create</param>
        /// <param name="model.ParentCommentId">OPTIONAL. Identifier of comment which wanted to reply.</param>
        /// <returns>
        /// - 200 OK if the comment is created successfully.
        /// - 400 Bad Request if there's an error during creation.
        /// - 401 Unauthorized if the user is not authenticated.
        /// - 404 Not Found if the Note or Parent Comment (if the parameter is specified) does not exist.
        /// </returns>
        [HttpPost, Route("create/note")]
        public async Task<IActionResult> CreateCommentOnNote([FromBody] EntityCommentCreateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });
            Note? NoteTarget = null;
            try
            {

                NoteTarget = await _db.Note.FirstOrDefaultAsync(n => n.Id == model.EntityId && n.UserId == UserRequested.Id);

                if (NoteTarget == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to create comment on Note: [model.NoteId: {model.EntityId}]. Note not found.");

                    return NotFound(new { Message = "Note not found." });
                }
                Comment? ParentComment = null;

                if (model.ParentCommentId != null)
                {
                    HashSet<long> ReplyableCommentIdSet = _db.GetAvailableNoteCommentIds(UserRequested.Id, NoteTarget.Id);

                    ParentComment = await _db.Comment.FirstOrDefaultAsync(c => c.CommentNote != null && c.CommentNote.NoteId == NoteTarget.Id && c.Id == model.ParentCommentId);

                    if (ParentComment == null)
                    {
                        _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to create comment on Note: [Id: {NoteTarget.Id}] and reply for ParentComment: [model.ParentCommentId: {model.ParentCommentId}]");

                        return NotFound(new { Message = "ParentComment not found." });
                    }

                    if (!ReplyableCommentIdSet.Contains(ParentComment.Id)) //Users can only reply the comments they see.
                    {
                        _logger.LogWarning($"Unauthorized, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to create comment on Note: [Id: {NoteTarget.Id}] and reply for ParentComment: [model.ParentCommentId: {model.ParentCommentId}]. User does not have permission to reply this comment.");

                        return Unauthorized(new { Message = "You do not have permission to reply this comment" });
                    }
                }

                Comment CommentCreated = new()
                {
                    Text = model.CommentText,
                    UserId = UserRequested.Id,
                    ParentCommentId = ParentComment?.Id
                };


                CommentNote CommentNoteCreated = new()
                {
                    Comment = CommentCreated,
                    NoteId = NoteTarget.Id
                };

                _db.Comment.Add(CommentCreated);
                _db.CommentNote.Add(CommentNoteCreated);


                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has created on Note: [Id: {NoteTarget.Id}, NoteTarget.UserId: {NoteTarget.UserId}, NoteTarget.User.UserName: {NoteTarget.User.UserName}].");

                return Ok(new { Message = "You have created comment on that note successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get comments attached on  Note: [Id: {NoteTarget?.Id}, NoteTarget.UserId: {NoteTarget?.UserId}, NoteTarget.User.UserName: {NoteTarget?.User.UserName}]. Error Details: {ex}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }

        }


        /// <summary>
        /// As the name implies, allow users to create comment on specified note. Users can reply the comment only they see. For Validation model: <see cref="EntityCommentCreateModel"/>
        /// </summary>
        /// <param name="model.EntityId">The Entity Identifier which demanded to attach comment. </param>
        /// <param name="model.CommentText">The text of the comment which demanded to create</param>
        /// <param name="model.ParentCommentId">OPTIONAL. Identifier of comment which wanted to reply.</param>
        /// <returns>
        /// - 200 OK if the comment is created successfully.
        /// - 400 Bad Request if there's an error during creation.
        /// - 401 Unauthorized if the user is not authenticated.
        /// - 404 Not Found if the Verse or Parent Comment (if the parameter is specified) does not exist.
        /// </returns>
        [HttpPost, Route("create/verse")]
        public async Task<IActionResult> CreateCommentOnVerse([FromBody] EntityCommentCreateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });

            Verse? VerseTarget = null;

            try
            {
                VerseTarget = await _db.Verse.FirstOrDefaultAsync(v => v.Id == model.EntityId);

                if (VerseTarget == null)
                    return NotFound(new { Message = "Verse not found." });


                Comment? ParentComment = null;

                if (model.ParentCommentId != null)
                {
                    HashSet<long> ReplyableCommentIdSet = _db.GetAvailableVerseCommentIds(UserRequested.Id, VerseTarget.Id);

                    ParentComment = await _db.Comment.FirstOrDefaultAsync(c => c.CommentVerse != null && c.CommentVerse.VerseId == VerseTarget.Id && c.Id == model.ParentCommentId);

                    if (ParentComment == null)
                    {
                        _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to create comment on Verse: [Id: {VerseTarget.Id}] and reply for ParentComment: [model.ParentCommentId: {model.ParentCommentId}]");
                        return NotFound(new { Message = "ParentComment not found." });
                    }

                    if (!ReplyableCommentIdSet.Contains(ParentComment.Id)) //Users can only reply the comments they see.

                    {
                        _logger.LogWarning($"Unauthorized, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to create comment on Verse: [Id: {VerseTarget.Id}] and reply for ParentComment: [model.ParentCommentId: {model.ParentCommentId}]. User does not have permission to reply this comment.");

                        return Unauthorized(new { Message = "You do not have permission to reply this comment" });
                    }
                }

                Comment CommentCreated = new()
                {
                    Text = model.CommentText,
                    UserId = UserRequested.Id,
                    ParentCommentId = ParentComment?.Id
                };

                CommentVerse CommentVerseCreated = new()
                {
                    Comment = CommentCreated,
                    VerseId = VerseTarget.Id
                };


                _db.Comment.Add(CommentCreated);
                _db.CommentVerse.Add(CommentVerseCreated);


                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has created on Verse: [Id: {VerseTarget.Id}].");

                return Ok(new { Message = "You have created comment on that verse successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get comments attached on Verse: [Id: {VerseTarget?.Id}], Error Details: {ex}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }

        }


        /// <summary>
        /// Allow users to update their comment.For Validation model: <see cref="CommentUpdateModel"/>
        /// </summary>
        /// <param name="model.NewCommentText">The new comment text which user wants to be changed to. </param>
        /// <param name="model.CommentId">Identifier of the comment will be altered.</param>
        /// <returns>
        /// - 200 OK if the comment is updated successfully.
        /// - 400 Bad Request if there's an error during updating.
        /// - 401 Unauthorized if the user is not authenticated.
        /// - 404 Not Found if the Comment to be updated or Parent Comment (if the parameter is specified) does not exist.
        /// </returns>
        [HttpPut, Route("update")]
        public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });
            Comment? CommentUpdated = null;
            try
            {
                CommentUpdated = await _db.Comment.FirstOrDefaultAsync(c => c.Id == model.CommentId && c.UserId == UserRequested.Id);

                if (CommentUpdated == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to update Comment: [model.CommentId: {model.CommentId}]. CommentUpdated is not found.");
                    return NotFound(new { Message = "Comment not found." });
                }



                CommentUpdated.Text = model.NewCommentText;
                CommentUpdated.UpdatedAt = DateTime.UtcNow;

                _db.Comment.Update(CommentUpdated);

                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}]. Updated Comment: [Id: {CommentUpdated.Id}]");
                return Ok(new { Message = "You have updated the comment!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to update Comment: [Id: {CommentUpdated?.Id}]. Error Details: {ex}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }

        }


        /// <summary>
        /// Deletes the specified comments. Users can only delete their own comments. But with that process, reply comments, no matter how deep they are, will be deleted as well. For Validation model: <see cref="CommentDeleteModel"/>
        /// </summary>
        /// <param name="model.CommentId">Identifier of the comment will be deleted.</param>
        /// <remarks> This process uses and recursive function: <see cref="DeleteCommentRecursively"/>       
        /// <returns>
        /// - 200 OK if the comment is deleted successfully.
        /// - 400 Bad Request if there's an error during deletion.
        /// - 401 Unauthorized if the user is not authenticated.
        /// - 404 Not Found if the Comment to be deleted does not exist.
        /// </returns>
        [HttpDelete, Route("delete")]
        public async Task<IActionResult> DeleteComment([FromBody] CommentDeleteModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });

            Comment? CommentDeleted = null;

            try
            {

                CommentDeleted = await _db.Comment.FirstOrDefaultAsync(c => c.Id == model.CommentId && c.UserId == UserRequested.Id);

                if (CommentDeleted == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to delete Comment: [model.CommentId: {model.CommentId}]. CommentDeleted is not found.");

                    return NotFound(new { Message = "Comment not found." });
                }
                await DeleteCommentRecursively(CommentDeleted);


                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}]. Updated Comment: [Id: {CommentDeleted.Id}]. Comment deleted.");

                return Ok(new { Message = "You have deleted the comment successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] trying to delete Comment: [Id: {CommentDeleted?.Id}]. Error Details: {ex}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }

        }

        /// <summary>
        /// Recursively deletes the specified comment and all of its descendant replies from the database.
        /// </summary>
        /// <param name="comment">The <see cref="Comment"/> entity to be deleted, including all its nested replies.</param>
        private async Task DeleteCommentRecursively(Comment comment)
        {
            await _db.Entry(comment).Collection(c => c.Replies).LoadAsync();

            if (comment.Replies != null && comment.Replies.Count != 0)
                foreach (var child in comment.Replies.ToList())
                    await DeleteCommentRecursively(child);

            _db.Comment.Remove(comment);
        }

    }
}