/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.

 
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.DB;
using ScriptiumBackend.Models;
using ScriptiumBackend.Validation;

namespace ScriptiumBackend.Controllers.SuggestionHandler
{

    [ApiController, Route("suggestion"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class SuggestionController(ApplicationDbContext db, UserManager<User> userManager, ILogger<SuggestionController> logger) : ControllerBase
    {

        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
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
                return NotFound(new { message = "User not found." });

            try
            {

                TranslationText? TranslationTextBeAttached = await _db.TranslationText.FirstOrDefaultAsync(tt => tt.Id == model.TranslationTextId && tt.Translation.EagerFrom.HasValue);

                if (TranslationTextBeAttached == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to create a suggestion on non-exist TranslationText");
                    return NotFound(new { message = "Translation is not found." });
                }

                Suggestion? ExistingSuggestion = await _db.Suggestion.FirstOrDefaultAsync(s => s.UserId == UserRequested.Id && s.TranslationTextId == TranslationTextBeAttached.Id);

                if (ExistingSuggestion != null)
                {
                    _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to create another suggestion on TranslationText: [Id: {TranslationTextBeAttached.Id}]");
                    return Conflict(new { message = "There is already a Suggestion belongs to you attached this Translation" });

                }
                Suggestion SuggestionCreated = new()
                {
                    SuggestionText = model.SuggestionText,
                    UserId = UserRequested.Id,
                    TranslationTextId = TranslationTextBeAttached.Id
                };

                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: From User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] the saved Suggestion: [Id: {SuggestionCreated.Id}]");
                return Ok(new { message = "The suggestion is successfully attached." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to create a suggestion on TranslationText: [Id: {model.TranslationTextId}] Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
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
                return NotFound(new { message = "User not found." });

            try
            {
                TranslationText? TranslationTextBeDeAttached = await _db.TranslationText.FirstOrDefaultAsync(tt => tt.Id == model.TranslationTextId && tt.Translation.EagerFrom.HasValue);

                if (TranslationTextBeDeAttached == null)
                {
                    _logger.LogWarning($"Not Found, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to remove a suggestion on non-exist TranslationText");


                    return NotFound(new { message = "Translation is not found." });
                }



                Suggestion? SuggestionDeleted = await _db.Suggestion.FirstOrDefaultAsync(s => s.UserId == UserRequested.Id && s.TranslationTextId == TranslationTextBeDeAttached.Id);

                if (SuggestionDeleted == null)
                {
                    _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] tried to remove non-exist suggestion on TranslationText: [Id: {TranslationTextBeDeAttached.Id}]");
                    return NotFound(new { message = "There is no a Suggestion belongs to you attached this Translation" });
                }

                _db.Suggestion.Remove(SuggestionDeleted);


                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed: From User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] the removed Suggestion: [Id: {SuggestionDeleted.Id}]");

                return Ok(new { message = "The suggestion is successfully removed." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to remove a suggestion on TranslationText: [Id: {model.TranslationTextId}] Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }
    }
}

*/