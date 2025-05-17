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

namespace scriptium_backend_dotnet.Controllers.FollowHandler
{

    [ApiController, Route("follow"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class FollowController(ApplicationDBContext db, UserManager<User> userManager, ILogger<FollowController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<FollowController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet("follower/{followType}")]
        public async Task<IActionResult> GetFollower([FromRoute] FollowStatus followType)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            try
            {
                List<FollowerUserDTO>? data = await _db.Follow
                    .Where(f => f.FollowedId == UserRequested.Id && f.Status == followType).Include(f => f.Follower)
                    .Select(f => f.ToFollowerUserDTO())
                    .ToListAsync();

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded his followER records. {data.Count} row has ben returned.");

                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get followER records. Error Details: {ex}");
                return StatusCode(500, new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpGet, Route("followed/{followType}")]
        public async Task<IActionResult> GetFollowed([FromRoute] FollowStatus followType)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound();

            try
            {
                List<FollowedUserDTO>? data = await _db.Follow.OrderByDescending(f => f.OccurredAt)
                .Where(f => f.FollowerId == UserRequested.Id && f.Status == followType).Include(f => f.Followed)
                .Select(f => f.ToFollowedUserDTO()).ToListAsync();

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded his followED records. {data.Count} row has ben returned.");
                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to get followED records. Error Details: {ex}");

                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpPost, Route("follow")]
        public async Task<IActionResult> Follow([FromBody] UserNameModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserFollowed = await _userManager.FindByNameAsync(model.UserName);

            bool isBlocked;

            try
            {

                if (UserRequested == null || UserFollowed == null || UserFollowed.IsFrozen.HasValue || (isBlocked = await _db.Block.AnyAsync(b => b.BlockedId == UserFollowed.Id && b.BlockerId == UserRequested.Id)))
                    return NotFound(new { message = "User not found." });

                if (UserRequested.Id == UserFollowed.Id)
                {
                    _logger.LogWarning($"Conflict occurred: User: [Id: {UserRequested.Id}, UserName: {UserRequested.Name}] tried to follow himself.");
                    return BadRequest(new { message = "You cannot follow yourself." });
                }


                bool isAlreadyFollowing = await _db.Follow.AnyAsync(f => f.FollowerId == UserRequested.Id && f.FollowedId == UserFollowed.Id);

                if (isAlreadyFollowing)
                {
                    _logger.LogWarning($"Conflict occurred:  User: [Id: {UserRequested.Id}, UserName: {UserRequested.Name}] is already following the User: [Id: {UserFollowed.Id}, UserName: {UserFollowed.Name}] User tried to follow another user which already followed.");
                    return Ok(new { message = "You are already following this user or a follow request is pending." });
                }


                bool IsUserFollowedPrivate = UserFollowed.IsPrivate.HasValue;

                Follow follow = new()
                {
                    FollowerId = UserRequested.Id,
                    FollowedId = UserFollowed.Id,
                    Status = IsUserFollowedPrivate ? FollowStatus.Pending : FollowStatus.Accepted
                };


                _db.Follow.Add(follow);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed. User: [Id: {UserRequested.Id}, UserName: {UserRequested.Name}] has successfully followed the User: [Id: {UserFollowed.Id}, UserName: {UserFollowed.Name}]. User followed.");

                string message = IsUserFollowedPrivate ? "Follow request sent successfully." : "You are successfully following the user!";

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested?.Id}, UserName: {UserRequested?.Name}] trying to follow User: [Id: {UserFollowed?.Id}, UserName: {UserFollowed?.Name}]. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }


        [HttpPut, Route("accept")]
        public async Task<IActionResult> Accept([FromBody] UserNameModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserAccepted = await _userManager.FindByNameAsync(model.UserName);

            if (UserRequested == null || UserAccepted == null || UserAccepted.IsFrozen.HasValue)
                return NotFound(new { message = "User not found." });

            try
            {

                Follow? Follow = await _db.Follow.OrderByDescending(f => f.OccurredAt).FirstOrDefaultAsync(f => f.FollowedId == UserRequested.Id && f.FollowerId == UserAccepted.Id && f.Status == FollowStatus.Pending);

                if (Follow == null)
                {
                    _logger.LogWarning($"Not Found, while: Trying to ACCEPT follow request to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserAccepted.Id}, UserName: {UserAccepted.UserName}]. There is no pending follow request to ACCEPT.");
                    return BadRequest(new { message = "Follow request either is already accepted or retrieved from the user!" });
                }

                Follow.OccurredAt = DateTime.Now;
                Follow.Status = FollowStatus.Accepted;

                _db.Follow.Update(Follow);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed: The pending follow request has been ACCEPTED to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserAccepted.Id}, UserName: {UserAccepted.UserName}]. Follow request accepted.");
                return Ok(new { message = "Follow request is successfully accepted!" });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: Trying to ACCEPT pending follow request to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserAccepted.Id}, UserName: {UserAccepted.UserName}]. Error Details: {ex} ");
                return StatusCode(500, new { message = "Something went unexpectedly wrong?" });
            }

        }

        [HttpDelete, Route("reject")]
        public async Task<IActionResult> Reject([FromBody] UserNameModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserRejected = await _userManager.FindByNameAsync(model.UserName);

            if (UserRequested == null || UserRejected == null || UserRejected.IsFrozen.HasValue)
                return NotFound(new { message = "User not found." });

            try
            {
                Follow? Follow = await _db.Follow.OrderByDescending(f => f.OccurredAt).FirstOrDefaultAsync(f => f.FollowedId == UserRequested.Id && f.FollowerId == UserRejected.Id && f.Status == FollowStatus.Pending);

                if (Follow == null)
                {
                    _logger.LogWarning($"Not Found, while: Trying to REJECT follow request to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserRejected.Id}, UserName: {UserRejected.UserName}]. There is no pending follow request to REJECT.");
                    return BadRequest(new { message = "Follow request either is already accepted or retrieved from the user!" });

                }

                _db.Follow.Remove(Follow);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed: The pending follow request has been REJECTED to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserRejected.Id}, UserName: {UserRejected.UserName}]. Follow request rejected.");
                return Ok(new { message = "The request is successfully rejected!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}]  trying to unfollow User: [Id: {UserRejected.Id}, UserName: {UserRejected.UserName}]. Error Details: {ex} ");
                return StatusCode(500, new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpDelete, Route("unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] UserNameModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserUnfollowed = await _userManager.FindByNameAsync(model.UserName);

            if (UserRequested == null || UserUnfollowed == null || UserUnfollowed.IsFrozen.HasValue)
                return NotFound(new { message = "User not found." });

            try
            {
                Follow? Follow = await _db.Follow.OrderByDescending(f => f.OccurredAt).FirstOrDefaultAsync(f => f.FollowerId == UserRequested.Id && f.FollowedId == UserUnfollowed.Id && f.Status == FollowStatus.Accepted);

                if (Follow == null)
                {
                    _logger.LogWarning($"Conflict occurred:  User: [Id: {UserRequested.Id}, UserName: {UserRequested.Name}] is already NOT following the User: [Id: {UserUnfollowed.Id}, UserName: {UserUnfollowed.Name}]. User tried to follow another user which already followed.");
                    return BadRequest(new { message = "You are already not following the user!" });
                }

                _db.Follow.Remove(Follow);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, UserName: {UserRequested.Name}] has successfully UNfollowed the User: [Id: {UserUnfollowed.Id}, UserName: {UserUnfollowed.Name}]. User unfollowed.");
                return Ok(new { message = "The user is successfully unfollowed!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: The follow record is trying to be DELETED follow request to User: [Id: {UserUnfollowed.Id}, UserName: {UserUnfollowed.UserName}] from User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}]. Error Details: {ex} ");
                return StatusCode(500, new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpDelete, Route("remove")]
        public async Task<IActionResult> Remove([FromBody] UserNameModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserRemoved = await _userManager.FindByNameAsync(model.UserName);

            if (UserRequested == null || UserRemoved == null || UserRemoved.IsFrozen.HasValue)
                return NotFound(new { message = "User not found." });

            try
            {
                Follow? Follow = await _db.Follow.OrderByDescending(f => f.OccurredAt).FirstOrDefaultAsync(f => f.FollowedId == UserRequested.Id && f.FollowerId == UserRemoved.Id && f.Status == FollowStatus.Accepted);

                if (Follow == null)
                {
                    _logger.LogWarning($"There is no follow record to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserRemoved.Id}, UserName: {UserRemoved.UserName}]");
                    return BadRequest(new { message = "The user is not following you!" });
                }

                _db.Follow.Remove(Follow);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: The follow record has been DELETED to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserRemoved.Id}, UserName: {UserRemoved.UserName}]");

                return Ok(new { message = "You have successfully removed the user!" });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: DELETING follow record to User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}] from User: [Id: {UserRemoved.Id}, UserName: {UserRemoved.UserName}]. Error Details: {ex} ");

                return StatusCode(500, new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpDelete, Route("retrieve")]
        public async Task<IActionResult> Retrieve([FromBody] UserNameModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserRetrieved = await _userManager.FindByNameAsync(model.UserName);

            if (UserRequested == null || UserRetrieved == null || UserRetrieved.IsFrozen.HasValue)
                return NotFound(new { message = "User not found." });

            try
            {
                Follow? Follow = await _db.Follow.OrderByDescending(f => f.OccurredAt).FirstOrDefaultAsync(f => f.FollowerId == UserRequested.Id && f.FollowedId == UserRetrieved.Id && f.Status == FollowStatus.Pending);

                if (Follow == null)
                {
                    _logger.LogWarning($"There is pending request to RETRIEVE to User: [Id: {UserRetrieved.Id}, UserName: {UserRetrieved.UserName}] from User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}]");
                    return BadRequest(new { message = "You do not have pending request for this user!" });
                }

                _db.Follow.Remove(Follow);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"Operation completed: The pending follow request has been RETRIEVED to User: [Id: {UserRetrieved.Id}, UserName: {UserRetrieved.UserName}] from User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}]");

                return Ok(new { message = "The follow request is successfully retrieved!" });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: RETRIEVING follow request to User: [Id: {UserRetrieved.Id}, UserName: {UserRetrieved.UserName}] from User: [Id: {UserRequested.Id}, UserName: {UserRequested.UserName}]. Error Details: {ex} ");
                return StatusCode(500, new { message = "Something went unexpectedly wrong?" });
            }
        }
    }
}