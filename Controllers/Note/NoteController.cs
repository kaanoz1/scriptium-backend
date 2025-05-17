using System.Security.Claims;
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

namespace scriptium_backend_dotnet.Controllers.NoteHandler
{

    [ApiController, Route("note"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class NoteController(ApplicationDBContext db, UserManager<User> userManager, ILogger<NoteController> logger) : ControllerBase
    {

        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<NoteController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet, Route("notes")]
        public async Task<IActionResult> GetNote()
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });
            try
            {

                List<NoteOwnDTO> data = await _db.Note.Where(n => n.UserId == UserRequested.Id)
                    .Include(n => n.Likes).ThenInclude(l => l.Like)
                    .Include(n => n.Verse).ThenInclude(n => n.Chapter).ThenInclude(c => c.Meanings)
                    .Include(n => n.Verse).ThenInclude(n => n.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Scripture)
                    .AsSplitQuery()
                    .Select(n => n.ToNoteOwnDTO(UserRequested)).ToListAsync();

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded his note records. {data.Count} row has ben returned.");
                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get note records. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }



        [HttpGet, Route("{verseId}")]
        public async Task<IActionResult> GetNote([FromRoute] int verseId)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });
            try
            {
                Verse? VerseAttached = await _db.Verse.FirstOrDefaultAsync(v => v.Id == verseId);

                if (VerseAttached == null)
                    return NotFound(new { message = "Verse not found." });


                HashSet<Guid> FollowedUserIds = _db.Follow
                        .Where(f => f.FollowerId == UserRequested.Id && f.Followed.IsPrivate.HasValue && f.Status == FollowStatus.Accepted)
                        .Select(f => f.FollowedId)
                        .ToHashSet();


                List<NoteOwnerDTO> data = await _db.Note
                    .Where(n => n.VerseId == VerseAttached.Id &&
                              (n.UserId == UserRequested.Id || FollowedUserIds.Contains(n.UserId)))
                               .Include(n => n.User)
                               .Include(n => n.Likes).ThenInclude(l => l.Like)
                               .Include(n => n.Comments)
                               .AsSplitQuery()
                                .Select(n => n.ToNoteOwnerDTO(UserRequested))
                               .ToListAsync();

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded his note records attached on Verse: [Id: {VerseAttached.Id}]. {data.Count} row has ben returned.");
                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get note records attached on Verse: [Id: {verseId}]. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }



        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            Verse? VerseAttached = null;


            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                VerseAttached = await _db.Verse.FirstOrDefaultAsync(v => v.Id == model.VerseId);

                if (VerseAttached == null)
                    return NotFound(new { message = "Verse not found." });

                int NoteCount = await _db.Note.CountAsync(n => n.UserId == UserRequested.Id && n.VerseId == VerseAttached.Id);

                if (NoteCount >= Utility.MAX_NOTE_COUNT_PER_VERSE)
                    return Unauthorized(new { message = "You cannot have notes attached on this verse." });


                Note NoteCreated = new()
                {
                    Text = model.NoteText,
                    UserId = UserRequested.Id,
                    VerseId = VerseAttached.Id
                };

                _db.Note.Add(NoteCreated);


                await _db.SaveChangesAsync();
                await transaction.CommitAsync();


                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has created note on Verse: [Id: {VerseAttached?.Id}].");
                return Ok(new { message = "Note is successfully created!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] creating a note on Verse: [Id: {VerseAttached?.Id}]. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }


        [HttpPut, Route("update")]
        public async Task<IActionResult> UpdateNote([FromBody] NoteUpdateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            try
            {

                Note? NoteUpdated = await _db.Note.FirstOrDefaultAsync(n => n.UserId == UserRequested.Id && n.Id == model.NoteId);

                if (NoteUpdated == null)
                    return NotFound(new { message = "Note not found." });

                NoteUpdated.Text = model.NoteText;

                _db.Note.Update(NoteUpdated);

                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has updating Note: [Id: {NoteUpdated?.Id}].");

                return Ok(new { message = "Note is successfully updated!" });
            }
            catch (Exception)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] updating a Note: [Id: {model.NoteId}]");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpDelete, Route("delete")]
        public async Task<IActionResult> DeleteNote([FromBody] NoteIdentifierModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            Note? NoteDeleted = null;

            try
            {
                NoteDeleted = await _db.Note.FirstOrDefaultAsync(n => n.UserId == UserRequested.Id && n.Id == model.NoteId);

                if (NoteDeleted == null)
                    return NotFound(new { message = "Note not found!" });

                _db.Note.Remove(NoteDeleted);

                await _db.SaveChangesAsync();

                return Ok(new { message = "Note is successfully deleted" });
            }
            catch (Exception)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] deleting a Note: [Id: {NoteDeleted?.Id}]");

                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }
    }
}