using System.Security.Claims;
using System.Text.Json;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using scriptium_backend_dotnet.Controllers.Validation;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Models;

namespace scriptium_backend_dotnet.Controllers.BlockHandler
{
    [ApiController, Route("block"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class BlockController(ApplicationDBContext db, UserManager<User> userManager, ILogger<BlockController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<BlockController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet, Route("")]
        public async Task<IActionResult> GetBlocked()
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            List<BlockDTO> data = await _db.Block.Include(b => b.Blocked).Where(b => b.BlockerId == UserRequested.Id).Select(b => b.ToBlockDTO()).ToListAsync();

            _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has demanded hiss block records. {data.Count} rows has been returned.");

            return Ok(new { data });
        }

        [HttpPost, Route("block")]
        public async Task<IActionResult> Block([FromBody] BlockModel model)
        {
            Console.WriteLine(model.UserName);
            Console.WriteLine(model.Reason);

            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserBlocked = await _userManager.FindByNameAsync(model.UserName);

            if (UserRequested == null || UserBlocked == null || UserBlocked.IsFrozen.HasValue)
                return NotFound(new { message = "User not found." });

            if (UserRequested.Id == UserBlocked.Id)
            {
                _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}], tried to block the User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}]. User tried to block himself.");
                return Conflict(new { message = "You cannot block yourself." });
            }

            using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync();
            try
            {

                bool isAlreadyBlocked = await _db.Block.AnyAsync(f => f.BlockerId == UserRequested.Id && f.BlockedId == UserBlocked.Id);
                Console.WriteLine("PROCEED");
                if (isAlreadyBlocked)
                {
                    Console.Write("CONFLICT!");
                    _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}], tried to block User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}]. But already blocked.");
                    return Conflict(new { message = "You already did block this user." });
                }

                Block Block = new()
                {
                    BlockerId = UserRequested.Id,
                    BlockedId = UserBlocked.Id,
                    Reason = model.Reason
                };


                //Entities should be deleted whenever a user blocks another:
                List<Comment> CommentsToBeDeleted = await _db.Comment.Where(c => c.User.Id == UserBlocked.Id && c.ParentComment != null && c.ParentComment.User.Id == UserRequested.Id).ToListAsync();
                //TODO: Recursive child deletion
                List<Like> LikesToBeDeleted = await _db.Like.Where(l => l.UserId == UserBlocked.Id && ((l.LikeNote != null && l.LikeNote.Note.UserId == UserRequested.Id) || (l.LikeComment != null && l.LikeComment.Comment.UserId == UserRequested.Id))).ToListAsync();
                List<Notification> NotificationsToBeDeleted = await _db.Notification.Where(n => n.ActorId == UserRequested.Id && n.RecipientId == UserBlocked.Id).ToListAsync();
                List<Follow> FollowRecordsToBeDeleted = await _db.Follow.Where(f => (f.FollowerId == UserRequested.Id && f.FollowedId == UserBlocked.Id) || (f.FollowerId == UserBlocked.Id && f.FollowedId == UserRequested.Id)).ToListAsync();

                _db.Comment.RemoveRange(CommentsToBeDeleted);
                _db.Like.RemoveRange(LikesToBeDeleted);
                _db.Notification.RemoveRange(NotificationsToBeDeleted);
                _db.Follow.RemoveRange(FollowRecordsToBeDeleted);

                await _db.Block.AddAsync(Block);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation($"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has successfully blocked the User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}].");

                return Ok(new { message = "User has been successfully blocked!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to block User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}]. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpDelete, Route("unblock")]
        public async Task<IActionResult> Unblock([FromBody] UserNameModel model)
        {

            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            User? UserBlocked = await _userManager.FindByNameAsync(model.UserName);

            if (UserRequested == null || UserBlocked == null)
                return NotFound(new { message = "User not found." });

            if (UserRequested.Id == UserBlocked.Id)
            {
                _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}], tried to UNblock the User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}]. User tried to unblock himself.");
                return BadRequest(new { message = "You cannot unblock yourself." });
            }

            try
            {
                Block? BlockRecord = await _db.Block.FirstOrDefaultAsync(f => f.BlockerId == UserRequested.Id && f.BlockedId == UserBlocked.Id);

                if (BlockRecord == null)
                {
                    _logger.LogWarning($"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}], tried to UNblock the User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}]. User is already non-blocked.");
                    return Conflict(new { message = "You already did NOT blocked this user." });
                }

                _db.Block.Remove(BlockRecord);

                await _db.SaveChangesAsync();

                _logger.LogInformation($"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has successfully UNblocked the User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}].");

                return Ok(new { message = "User block has been successfully removed!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to UNblock User: [Id: {UserBlocked.Id}, Username: {UserBlocked.UserName}]. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }

        }

        [HttpDelete, Route("unblock/mass")]
        public async Task<IActionResult> UnblockMass([FromBody] UserNameMassModel model)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);
            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            User? selfUnblockUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == UserRequested.Id && model.UserNames.Contains(u.UserName));

            if (selfUnblockUser != null)
            {
                _logger.LogWarning($"Self-Unblock Attempt: UserId={UserRequested.Id}, Username={UserRequested.UserName}");
                return BadRequest(new { message = "You cannot unblock yourself." });
            }

            List<User>? usersToUnblock = await _userManager.Users
                .Where(u => model.UserNames.Contains(u.UserName))
                .ToListAsync();

            List<string>? notFoundUsernames = model.UserNames
                .Except(usersToUnblock.Select(u => u.UserName))
                .ToList();

            if (notFoundUsernames.Any())
            {
                _logger.LogWarning($"Mass Unblock - Users Not Found: {string.Join(", ", notFoundUsernames.Select(u => $"Username={u}"))}");
                return NotFound(new { message = $"Users not found: {string.Join(", ", notFoundUsernames)}" });
            }

            List<Block> BlockRecords = await _db.Block
                .Include(b => b.Blocked)
                .Where(b => b.BlockerId == UserRequested.Id && model.UserNames.Contains(b.Blocked.UserName))
                .ToListAsync();

            HashSet<string> blockedUsernames = BlockRecords.Select(b => b.Blocked.UserName).ToHashSet();

            List<string>? alreadyUnblockedUsernames = model.UserNames
                .Except(blockedUsernames)
                .ToList();

            if (alreadyUnblockedUsernames.Any())
            {
                _logger.LogWarning($"Mass Unblock - Users Already Unblocked: {string.Join(", ", alreadyUnblockedUsernames.Select(u => $"Username={u}"))}");
                return Conflict(new { message = $"Users already unblocked: {string.Join(", ", alreadyUnblockedUsernames)}" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Block.RemoveRange(BlockRecords);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var unblockedUsersDetails = usersToUnblock.Select(u => new { u.Id, u.UserName }).ToList();
                _logger.LogInformation($"Mass Unblock Successful: UserId={UserRequested.Id}, Username={UserRequested.UserName} has unblocked users: {JsonSerializer.Serialize(unblockedUsersDetails)}");

                return Ok(new { message = "All specified users have been successfully unblocked." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError($"Mass Unblock Error: UserId={UserRequested.Id}, Username={UserRequested.UserName} attempted to unblock users: {string.Join(", ", model.UserNames)}. Error: {ex}");
                return StatusCode(500, new { message = "An error occurred while attempting to unblock users. No changes were made." });
            }
        }

    }
}