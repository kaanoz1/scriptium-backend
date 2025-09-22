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

namespace ScriptiumBackend.Controllers.SavingHandler
{
    [ApiController, Route("saving"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class SavingController(ApplicationDbContext db, UserManager<User> userManager, ILogger<SavingController> logger) : ControllerBase
    {
        //TODO: Amend
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
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
            List<CollectionProcessResultDto> succeed = [];
            List<CollectionProcessResultDto> failed = [];


            foreach (string CollectionName in model.CollectionNames)
            {
                CollectionProcessResultDto Dto;

                Collection? Collection = await _db.Collection
                    .FirstOrDefaultAsync(c => c.Name == CollectionName && c.UserId == UserRequested.Id);

                if (Collection == null)
                {
                    Dto = Collection.GetCollectionProcessResultDto(CollectionName);
                    failed.Add(Dto);

                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to save something on Claimed Collection: [CollectionName: {CollectionName}] Claimed Collection not found.");
                    continue;
                }

                bool isAlreadyExists = await _db.CollectionVerse.AnyAsync(cv => cv.CollectionId == Collection.Id && cv.VerseId == VerseAttached.Id);

                if (isAlreadyExists)
                {
                    Dto = Collection.GetCollectionProcessResultDto(CollectionStatus.AlreadyDone);

                    succeed.Add(Dto);
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
                    Dto = Collection.GetCollectionProcessResultDto(CollectionStatus.Succeed);

                    succeed.Add(Dto);
                }
                catch (Exception ex)
                {
                    Dto = Collection.GetCollectionProcessResultDto(CollectionStatus.Error);
                    _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to add Verse: [Id: {VerseAttached.Id}] to Collection: [Id: {Collection.Id}]. Error Details: {ex}");

                    failed.Add(Dto);
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
            CollectionProcessResultDto Dto;

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
            List<CollectionProcessResultDto> succeed = [];
            List<CollectionProcessResultDto> failed = [];


            foreach (string CollectionName in model.CollectionNames)
            {

                // Check if the collection exists for the user
                var Collection = await _db.Collection
                    .FirstOrDefaultAsync(c => c.Name == CollectionName && c.UserId == UserRequested.Id);

                if (Collection == null)
                {
                    Dto = Collection.GetCollectionProcessResultDto(CollectionName);

                    failed.Add(Dto);
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
                        Dto = Collection.GetCollectionProcessResultDto(CollectionStatus.NotFound);

                        failed.Add(Dto);
                        _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to unsave something on Collection: [Id: {Collection.Id}]. Collection has already NOT this item.");

                        continue;
                    }

                    //All non-succeed conditions were passed. So we are ready to make process.


                    _db.CollectionVerse.Remove(CollectionVerse);

                    await _db.SaveChangesAsync();

                    _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has removed Verse: [Id: {VerseRemoved.Id}] to Collection: [Id: {Collection.Id}]");

                    Dto = Collection.GetCollectionProcessResultDto(CollectionStatus.Succeed);
                    succeed.Add(Dto);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to remove Verse: [Id: {VerseRemoved.Id}] from Collection: [Id: {Collection.Id}]. Error Details: {ex}");

                    Dto = Collection.GetCollectionProcessResultDto(CollectionStatus.Error);
                    failed.Add(Dto);
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
*/