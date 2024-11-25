using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using writings_backend_dotnet.DB;
using writings_backend_dotnet.Models;
using writings_backend_dotnet.Validation;

namespace writings_backend_dotnet.Controllers.SuggestionHandler
{

    [ApiController, Route("suggestion"), Authorize]
    public class SuggestionController(ApplicationDBContext db, UserManager<User> userManager, ILogger<SuggestionController> logger) : ControllerBase
    {

        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<SuggestionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateSuggestion([FromBody] SuggestionCreateModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });
            TranslationText? TranslationTextBeAttached = null;
            try
            {

                TranslationTextBeAttached = await _db.TranslationText.FirstOrDefaultAsync(tt => tt.Id == model.TranslationTextId && tt.Translation.EagerFrom.HasValue);

                if (TranslationTextBeAttached == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to create a suggestion on non-exist TranslationText");
                    return NotFound(new { Message = "Translation is not found." });
                }

                Suggestion? ExistingSuggestion = await _db.Suggestion.FirstOrDefaultAsync(s => s.UserId == UserRequested.Id && s.TranslationTextId == TranslationTextBeAttached.Id);

                if (ExistingSuggestion != null)
                {
                    _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to create another suggestion on TranslationText: [Id: {TranslationTextBeAttached.Id}]");
                    return Conflict(new { Message = "There is already a Suggestion belongs to you attached this Translation" });

                }
                Suggestion SuggestionCreated = new()
                {
                    SuggestionText = model.SuggestionText,
                    UserId = UserRequested.Id,
                    TranslationTextId = TranslationTextBeAttached.Id
                };

                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: From User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] the saved Suggestion: [Id: {SuggestionCreated.Id}]");
                return Ok(new { Message = "The suggestion is successfully attached." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to create a suggestion on TranslationText: [Id: {TranslationTextBeAttached?.Id}] Error Details: {ex}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }

        }

        [HttpPost, Route("delete")]
        public async Task<IActionResult> DeleteSuggestion([FromBody] TranslationTextIdentifierModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { Message = "User not found." });

            TranslationText? TranslationTextBeDeAttached = null;
            try
            {
                TranslationTextBeDeAttached = await _db.TranslationText.FirstOrDefaultAsync(tt => tt.Id == model.TranslationTextId && tt.Translation.EagerFrom.HasValue);

                if (TranslationTextBeDeAttached == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to remove a suggestion on non-exist TranslationText");


                    return NotFound(new { Message = "Translation is not found." });
                }



                Suggestion? SuggestionDeleted = await _db.Suggestion.FirstOrDefaultAsync(s => s.UserId == UserRequested.Id && s.TranslationTextId == TranslationTextBeDeAttached.Id);

                if (SuggestionDeleted == null)
                {
                    _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to remove non-exist suggestion on TranslationText: [Id: {TranslationTextBeDeAttached.Id}]");
                    return NotFound(new { Message = "There is no a Suggestion belongs to you attached this Translation" });
                }

                _db.Suggestion.Remove(SuggestionDeleted);


                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed: From User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] the removed Suggestion: [Id: {SuggestionDeleted.Id}]");

                return Ok(new { Message = "The suggestion is successfully removed." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to remove a suggestion on TranslationText: [Id: {TranslationTextBeDeAttached?.Id}] Error Details: {ex}");
                return BadRequest(new { Message = "Something went unexpectedly wrong?" });
            }
        }
    }
}