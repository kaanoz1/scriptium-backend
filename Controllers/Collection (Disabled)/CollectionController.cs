/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using System.Security.Claims;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Controllers.Validation;
using ScriptiumBackend.DB;
using ScriptiumBackend.Models;


namespace ScriptiumBackend.Controllers.CollectionHandler
{
    [ApiController, Route("collection"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class CollectionController(
        ApplicationDbContext db,
        UserManager<User> userManager,
        ILogger<CollectionController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

        private readonly UserManager<User> _userManager =
            userManager ?? throw new ArgumentNullException(nameof(userManager));

        private readonly ILogger<CollectionController> _logger =
            logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet, Route("")]
        public async Task<IActionResult> GetCollections()
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            List<CollectionDto> data = await _db.Collection.Where(c => c.UserId == UserRequested.Id).Select(c =>
                new CollectionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    SaveCount = c.Verses != null ? c.Verses.Count : 0
                }).ToListAsync();

            _logger.LogInformation(
                $"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded his collection records. {data.Count} row has ben returned.");

            return Ok(new { data });
        }

        [HttpGet, Route("{CollectionId}")]
        public async Task<IActionResult> GetCollectionVerses([FromRoute] int CollectionId, [FromQuery] int Page = 1)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            Collection? collection = await _db.Collection
                .FirstOrDefaultAsync(c => c.Id == CollectionId && c.UserId == UserRequested.Id);

            if (collection == null)
            {
                _logger.LogWarning(
                    $"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get verses from a non-existing or unauthorized Collection: [Id: {CollectionId}]");
                return NotFound(new { message = "Collection not found or unauthorized." });
            }

            int Total = await _db.CollectionVerse
                .CountAsync(cv => cv.CollectionId == CollectionId);

            int Limit = 30;
            int Skip = (Page - 1) * Limit;

            List<VerseUpperDto> data = await _db.CollectionVerse
                .Where(cv => cv.CollectionId == CollectionId)
                .Include(v => v.Verse)
                .ThenInclude(v => v.Chapter)
                .ThenInclude(c => c.Section)
                .ThenInclude(c => c.Scripture)
                .ThenInclude(c => c.Meanings)
                .ThenInclude(m => m.Language)
                .Include(v => v.Verse)
                .ThenInclude(v => v.Chapter)
                .ThenInclude(c => c.Section)
                .ThenInclude(s => s.Meanings)
                .ThenInclude(m => m.Language)
                .Include(v => v.Verse)
                .ThenInclude(v => v.Chapter)
                .ThenInclude(c => c.Meanings)
                .ThenInclude(m => m.Language)
                .Include(v => v.Verse)
                .ThenInclude(v => v.Transliterations)
                .ThenInclude(t => t.Language)
                .Include(v => v.Verse)
                .ThenInclude(v => v.TranslationTexts)
                .ThenInclude(t => t.Translation)
                .ThenInclude(t => t.Language)
                .Include(v => v.Verse)
                .ThenInclude(v => v.TranslationTexts)
                .ThenInclude(t => t.Translation)
                .ThenInclude(t => t.TranslatorTranslations)
                .ThenInclude(tt => tt.Translator)
                .ThenInclude(t => t.Language)
                .Include(v => v.Verse)
                .ThenInclude(v => v.TranslationTexts)
                .ThenInclude(t => t.FootNotes)
                .ThenInclude(f => f.FootNoteText)
                .OrderBy(cv => cv.Id)
                .Skip(Skip)
                .Take(Limit)
                .IgnoreAutoIncludes()
                .Select(cv => cv.Verse)
                .AsNoTracking()
                .Select(v => v.ToVerseUpperDto(true)) //Already check in where
                .ToListAsync();


            _logger.LogInformation(
                $@"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded collection verses from Collection [Id: {CollectionId}]. {data.Count} row has been returned for page: {Page}.");


            return Ok(new { data });
        }


        [HttpGet, Route("verse/{VerseId}")]
        public async Task<IActionResult> GetCollections([FromRoute] int VerseId)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            List<CollectionWithVerseSavedInformationDto> data = await _db.Collection
                .Where(c => c.UserId == UserRequested.Id).Select(c => new CollectionWithVerseSavedInformationDto
                {
                    Name = c.Name,
                    Description = c.Description,
                    IsSaved = _db.CollectionVerse.Any(cv =>
                        cv.Collection.UserId == UserRequested.Id && cv.CollectionId == c.Id && cv.VerseId == VerseId)
                }).ToListAsync();

            _logger.LogInformation(
                $"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded his simple collection records. {data.Count} row has ben returned.");

            return Ok(new { data });
        }

        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateCollection([FromBody] CollectionCreateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (UserRequested.Collections?.Any(c => c.Name == model.CollectionName) ?? false)
                {
                    _logger.LogWarning(
                        $"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has tried to create a Collection named: {model.CollectionName}, User has a collection with same name.");

                    return Conflict(new { message = "You have already the collection with same name." });
                }

                int CollectionCount = await _db.Collection.CountAsync(c => c.UserId == UserRequested.Id);

                if (CollectionCount > Utility.MAX_COLLECTION_COUNT)
                    return Unauthorized(new { message = "You cannot have more collections." });

                Collection CollectionCreated = new()
                {
                    Name = model.CollectionName,
                    Description = model.Description,
                    UserId = UserRequested.Id
                };

                _db.Collection.Add(CollectionCreated);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    $"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has created a Collection: [Id: {CollectionCreated.Id}, CollectionName: {CollectionCreated.Name}]");
                return Ok(new { message = "Collection is successfully created!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    $"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has tried to create the collection. Collection: [CollectionName: {model.CollectionName}], Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpPut, Route("update")]
        public async Task<IActionResult> UpdateCollection([FromBody] CollectionUpdateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                Collection? CollectionUpdated =
                    await _db.Collection.FirstOrDefaultAsync(c =>
                        c.Id == model.CollectionId && c.UserId == UserRequested.Id);

                if (CollectionUpdated == null)
                {
                    await transaction.DisposeAsync();

                    _logger.LogWarning(
                        $"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to update Non-existing Collection: [model.CollectionId: {model.CollectionId}] model.NewCollectionName: {model.NewCollectionName}");
                    return NotFound(new { message = "There is no collection with this name." });
                }


                if (model.NewCollectionName != null)
                {
                    int collectionCount = await _db.Collection.CountAsync(c =>
                        c.UserId == UserRequested.Id && c.Name == model.NewCollectionName);

                    if (collectionCount != 0)
                    {
                        await transaction.DisposeAsync();

                        return Conflict(new { message = "You have already the collection with same name." });
                    }

                    CollectionUpdated.Name = model.NewCollectionName;
                }

                if (model.NewCollectionDescription != null)
                    CollectionUpdated.Description = model.NewCollectionDescription;

                _db.Collection.Update(CollectionUpdated);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    $"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has changed his collection with id: {model.CollectionId} to {model.NewCollectionName} and description to {model.NewCollectionDescription}");
                return Ok(new { message = "Collection is successfully updated!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    $"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has tried to create the collection. Collection: [Id: {model.CollectionId}], Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpDelete, Route("delete")]
        public async Task<IActionResult> DeleteCollection([FromBody] CollectionDeleteModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            try
            {
                Collection? CollectionDeleted =
                    await _db.Collection.FirstOrDefaultAsync(c =>
                        c.Id == model.CollectionId && c.UserId == UserRequested.Id);

                if (CollectionDeleted == null)
                {
                    _logger.LogWarning(
                        $"Not Found, while. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to delete Collection : [model.CollectionId: {model.CollectionId}]");
                    return NotFound(new { message = "This collection might be already deleted or never been exist." });
                }

                _db.Collection.Remove(CollectionDeleted);
                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    $"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has deleted his Collection: [Id: {CollectionDeleted.Id}, CollectionName: {CollectionDeleted.Name}]");

                return Ok(new { message = "Collection is successfully deleted!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error occurred, while. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to delete his Collection: [CollectionId: {model.CollectionId}]. Error Details: {ex}");

                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }
    }
}

*/