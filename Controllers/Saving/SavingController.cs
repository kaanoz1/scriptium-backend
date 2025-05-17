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

namespace scriptium_backend_dotnet.Controllers.SavingHandler
{
    [ApiController, Route("saving"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class SavingController(ApplicationDBContext db, UserManager<User> userManager, ILogger<SavingController> logger) : ControllerBase
    {
        //TODO: Amend
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<SavingController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        [HttpPost, Route("save")]
        public async Task<IActionResult> Save([FromBody] SavingProcessModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            Verse? VerseAttached = await _db.Verse.FirstOrDefaultAsync(v => v.Id == model.VerseId);

            if (VerseAttached == null)
                return NotFound(new { message = "Verse not found." });


            //This lists will indicate users which insertion has been succeed or failed.
            List<CollectionProcessResultDTO> succeed = [];
            List<CollectionProcessResultDTO> failed = [];


            foreach (string CollectionName in model.CollectionNames)
            {
                CollectionProcessResultDTO DTO;

                Collection? Collection = await _db.Collection
                    .FirstOrDefaultAsync(c => c.Name == CollectionName && c.UserId == UserRequested.Id);

                if (Collection == null)
                {
                    DTO = Collection.GetCollectionProcessResultDTO(CollectionName);
                    failed.Add(DTO);

                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to save something on Claimed Collection: [CollectionName: {CollectionName}] Claimed Collection not found.");
                    continue;
                }

                bool isAlreadyExists = await _db.CollectionVerse.AnyAsync(cv => cv.CollectionId == Collection.Id && cv.VerseId == VerseAttached.Id);

                if (isAlreadyExists)
                {
                    DTO = Collection.GetCollectionProcessResultDTO(CollectionStatus.AlreadyDone);

                    succeed.Add(DTO);
                    _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to save something on Collection: [Id: {Collection.Id}]. Collection has already this item.");
                    continue;
                }

                //All non-succeed conditions were passed. So we are ready to make process.

                CollectionVerse collectionVerse = new()
                {
                    CollectionId = Collection.Id,
                    VerseId = VerseAttached.Id
                };

                _db.CollectionVerse.Add(collectionVerse);

                try
                {
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"Operation phase completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has added Verse: [Id: {VerseAttached.Id}] to Collection: [Id: {Collection.Id}]");
                    DTO = Collection.GetCollectionProcessResultDTO(CollectionStatus.Succeed);

                    succeed.Add(DTO);
                }
                catch (Exception ex)
                {
                    DTO = Collection.GetCollectionProcessResultDTO(CollectionStatus.Error);
                    _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to add Verse: [Id: {VerseAttached.Id}] to Collection: [Id: {Collection.Id}]. Error Details: {ex}");

                    failed.Add(DTO);
                    _db.Entry(collectionVerse).State = EntityState.Detached;
                }
            }

            var data = new
            {
                succeed,
                failed
            };

            return Ok(new { success = succeed.Count > 0, data });
        }

        [HttpDelete, Route("unsave")]
        public async Task<IActionResult> Unsave([FromBody] SavingProcessModel model)
        {
            CollectionProcessResultDTO DTO;

            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            Verse? VerseRemoved = await _db.Verse.FirstOrDefaultAsync(v => v.Id == model.VerseId);

            if (VerseRemoved == null)
                return NotFound(new { message = "Verse not found." });

            //This lists will indicate users which insertion has been succeed or failed.
            List<CollectionProcessResultDTO> succeed = [];
            List<CollectionProcessResultDTO> failed = [];


            foreach (string CollectionName in model.CollectionNames)
            {

                // Check if the collection exists for the user
                var Collection = await _db.Collection
                    .FirstOrDefaultAsync(c => c.Name == CollectionName && c.UserId == UserRequested.Id);

                if (Collection == null)
                {
                    DTO = Collection.GetCollectionProcessResultDTO(CollectionName);

                    failed.Add(DTO);
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to UNsave something on Claimed Collection: [CollectionName: {CollectionName}] Claimed Collection not found.");

                    continue;
                }
                try
                {
                    // Check if the verse exists in the collection
                    CollectionVerse? CollectionVerse = await _db.CollectionVerse
                        .FirstOrDefaultAsync(cv => cv.CollectionId == Collection.Id && cv.VerseId == VerseRemoved.Id);

                    if (CollectionVerse == null)
                    {
                        DTO = Collection.GetCollectionProcessResultDTO(CollectionStatus.NotFound);

                        failed.Add(DTO);
                        _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to unsave something on Collection: [Id: {Collection.Id}]. Collection has already NOT this item.");

                        continue;
                    }

                    //All non-succeed conditions were passed. So we are ready to make process.


                    _db.CollectionVerse.Remove(CollectionVerse);

                    await _db.SaveChangesAsync();

                    _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has removed Verse: [Id: {VerseRemoved.Id}] to Collection: [Id: {Collection.Id}]");

                    DTO = Collection.GetCollectionProcessResultDTO(CollectionStatus.Succeed);
                    succeed.Add(DTO);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to remove Verse: [Id: {VerseRemoved.Id}] from Collection: [Id: {Collection.Id}]. Error Details: {ex}");

                    DTO = Collection.GetCollectionProcessResultDTO(CollectionStatus.Error);
                    failed.Add(DTO);
                }
            }

            var data = new
            {
                succeed,
                failed
            };

            return Ok(new { success = succeed.Count > 0, data });
        }

    }
}