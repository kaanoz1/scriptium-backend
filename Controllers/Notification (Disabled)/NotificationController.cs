/*
    THIS ROUTE IS TEMPORARILY DISABLED SINCE THE BACKEND SERVICE IS CLOSED FOR USER DATA.
    
    
    
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.DB;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Controllers.NotificationHandler
{

    [ApiController, Route("notification"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class NotificationController(ApplicationDbContext db, UserManager<User> userManager, ILogger<NotificationController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<NotificationController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet, Route("{quantity?}")]
        public async Task<IActionResult> GetNotifications([FromRoute] int? quantity)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            List<Notification> data;
            try
            {

                IQueryable<Notification> notificationsQuery = _db.Notification
                    .Where(n => n.RecipientId == UserRequested.Id)
                    .OrderByDescending(n => n.CreatedAt);

                if (quantity.HasValue)
                    notificationsQuery = notificationsQuery.Take(quantity.Value);

                data = await notificationsQuery.ToListAsync();

                _logger.LogError($"Operation completed: User: [Id {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded its notification records. {data.Count} rows has been returned.");

                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get notification records. Error details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpPut, Route("read")]
        public async Task<IActionResult> GetNotifications([FromBody] HashSet<long> ReadNotificationsIds)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            try
            {
                List<Notification> Notifications = await _db.Notification.Where(n => n.RecipientId == UserRequested.Id && ReadNotificationsIds.Contains(n.Id)).ToListAsync();

                foreach (Notification notification in Notifications)
                    notification.IsRead = true;

                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: User: [Id {UserRequested.Id}, Username: {UserRequested.UserName}] has read {Notifications.Count} notifications.");
                return Ok(new { message = "Successfully have read!" });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id {UserRequested.Id}, Username: {UserRequested.UserName}] trying to read his notifications. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }
    }
}

*/