using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using scriptium_backend_dotnet.Controllers.Validation;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Models;
using scriptium_backend_dotnet.Models.Util;

namespace scriptium_backend_dotnet.Controllers.SessionHandler
{
    [ApiController, Route("session"), Authorize, EnableRateLimiting(policyName: "InteractionControllerRateLimit")]
    public class SessionController(
        ApplicationDBContext db,
        ILogger<SessionController> logger,
        SignInManager<User> signInManager,
        UserManager<User> userManager) : ControllerBase
    {
        private readonly ILogger<SessionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private readonly SignInManager<User> _signInManager =
            signInManager ?? throw new ArgumentNullException(nameof(signInManager));

        private readonly UserManager<User> _userManager =
            userManager ?? throw new ArgumentNullException(nameof(userManager));

        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));

        [HttpPost, Route("logout")]
        public async Task<IActionResult> Logout()
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized(new { message = "You are not logged in!" });

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "Something went wrong!" });

            _logger.LogInformation($"User {UserRequested.UserName} has tried to log out.");

            await _signInManager.SignOutAsync();

            _logger.LogInformation(
                $"User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] successfully logged out.");

            return Ok(new { message = "Successfully logged out" });
        }

        [HttpPut, Route("alter")]
        public async Task<IActionResult> Alter()
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized(new { message = "You are not logged in!" });

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "Something went wrong!" });

            bool IsPrivate = UserRequested.IsPrivate.HasValue;

            if (IsPrivate)
            {
                List<Follow> FollowsAccepted = await _db.Follow
                    .Where(f => f.FollowedId == UserRequested.Id && f.Status == FollowStatus.Pending).ToListAsync();

                foreach (Follow follow in FollowsAccepted)
                    follow.Status = FollowStatus.Accepted;

                await _db.SaveChangesAsync();
            }

            UserRequested.IsPrivate = IsPrivate ? null : DateTime.UtcNow;
            try
            {
                string AccountStatus = IsPrivate ? "public" : "private";

                await _userManager.UpdateAsync(UserRequested);
                _logger.LogInformation(
                    $"Operation completed: User: [Id: {UserRequested.Id} Username: {UserRequested.UserName}] has changed his account to {AccountStatus}");
                return Ok(new { message = $"You account is now {AccountStatus}!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error occurred, while: User: [Id: {UserRequested.Id} Username: {UserRequested.UserName}] alter his account. Error Detail: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpPost, Route("freeze"), EnableRateLimiting(policyName: "UpdateActionRateLimit")]
        public async Task<IActionResult> Freeze([FromBody] PasswordModel model)
        {
            FreezeR? FreezeRecord;

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "You are not logged in!" });

            User? UserRequested = await _userManager.FindByIdAsync(userId);
            if (UserRequested == null)
                return NotFound(new { message = "Something went wrong!" });

            bool isPasswordTrue = await _userManager.CheckPasswordAsync(UserRequested, model.Password);

            if (!isPasswordTrue) return Unauthorized(new { message = "Invalid Credentials!" });

            try
            {
                FreezeRecord = await _db.FreezeR.OrderByDescending(r => r.ProceedAt)
                    .FirstOrDefaultAsync(r =>
                        r.UserId.ToString() == userId && r.Status == FreezeStatus.Frozen &&
                        r.ProceedAt < DateTime.UtcNow.AddDays(7));

                if (FreezeRecord != null)
                {
                    _logger.LogWarning(
                        $"Conflict occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is trying to freeze his account twice within 7 days.");
                    return Unauthorized(new { message = "You cannot freeze your account twice within 7 days!" });
                }

                FreezeRecord = new FreezeR
                {
                    UserId = UserRequested.Id,
                    Status = FreezeStatus.Frozen,
                    ProceedAt = DateTime.UtcNow,
                };

                UserRequested.IsFrozen = DateTime.UtcNow;

                _db.FreezeR.Add(FreezeRecord);

                List<Session> ActiveSessions = await _db.Session.Where(s => s.UserId == UserRequested.Id).ToListAsync();

                foreach (Session session in ActiveSessions)
                    _db.Session.Remove(session);

                await _db.SaveChangesAsync();

                await _userManager.UpdateAsync(UserRequested);
                await _signInManager.SignOutAsync();
                _logger.LogInformation(
                    $"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has frozen his account.");
                return Ok(new { message = "Your account has been successfully frozen!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] trying to freeze his account. Error Details: {ex}");
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpDelete, Route("delete"), EnableRateLimiting(policyName: "UpdateActionRateLimit")]
        public async Task<IActionResult> Delete()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "You are not logged in!" });

            User? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found!" });

            if (user.DeletedAt != null)
                return BadRequest(new { message = "Your account is already scheduled for deletion." });

            await using IDbContextTransaction? transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                user.DeletedAt = DateTime.UtcNow;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                    throw new InvalidOperationException("Failed to mark user as scheduled for deletion.");

                List<Session> sessions = await _db.Session.Where(s => s.UserId == user.Id).ToListAsync();
                _db.Session.RemoveRange(sessions);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                await _signInManager.SignOutAsync();

                _logger.LogInformation($"User {user.Id} scheduled account deletion at {user.DeletedAt}");

                return Ok(new
                {
                    message =
                        "Your account is scheduled to be permanently deleted in 15 days. You can log in before then to cancel the deletion."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while scheduling deletion for user: {userId}");
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "An unexpected error occurred while processing your request." });
            }
        }
        
        
        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { message = "Unauthorized access." });

            User? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized(new { message = "User not found." });

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning($"Email change failed: Invalid password for user ID: {user.Id}");
                return BadRequest(new { message = "Invalid password." });
            }

            IdentityResult result = await _userManager.SetEmailAsync(user, model.NewEmail);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    _logger.LogWarning($"Email change error: {err.Code} - {err.Description}");

                return BadRequest(new { message = "Failed to change email." });
            }

            user.UserName = model.NewEmail;
            await _userManager.UpdateAsync(user);
            

            _logger.LogInformation($"User email updated successfully. User ID: {user.Id}");

            return Ok(new { message = "Email updated successfully." });
        }



        [HttpPut, Route("update")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "You are not logged in!" });

            User? UserRequested = await _userManager.FindByIdAsync(userId);

            if (UserRequested == null)
                return NotFound(new { message = "UserRequested not found!" });

            string UpdateLogRow = "";

            UserUpdateR UpdateRecord = new()
            {
                UserId = UserRequested.Id
            };

            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                var existingUser = await _userManager.FindByNameAsync(model.Username);

                if (existingUser != null)
                    return BadRequest(new { message = "Username is already taken!" });
            }

            if (model.LanguageId.HasValue)
            {
                bool languageExists = await _db.Language
                    .AnyAsync(l => l.Id == model.LanguageId);

                if (!languageExists)
                    return BadRequest(new { message = "Invalid language ID!" });
            }

            if (model.Image != null && model.Image.Length > 0)
            {
                using var memoryStream = new MemoryStream();

                await model.Image.CopyToAsync(memoryStream);

                byte[] UpdatedImage = memoryStream.ToArray();

                UserRequested.Image = UpdatedImage;
                UpdateRecord.Image = UpdatedImage;
                UpdateLogRow += $" Profile Image updated.";
            }


            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                string UpdatedName = model.Name.Trim();

                UpdateLogRow += $" Name: {UserRequested.Name} -> {UpdatedName}";

                UserRequested.Name = UpdatedName;
                UpdateRecord.Name = UpdatedName;
            }

            if (model.Surname != null)
            {
                string trimmed = model.Surname.Trim();
                UpdateLogRow += $" Surname: {UserRequested.Surname} -> {trimmed}";
                UserRequested.Surname = trimmed;
                UpdateRecord.Surname = trimmed;
            }

            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                string UpdatedUsername = model.Username.Trim();

                UpdateLogRow += $" UserName: {UserRequested.UserName} -> {UpdatedUsername}";
                UserRequested.UserName = UpdatedUsername;
                UpdateRecord.Username = UpdatedUsername;
            }

            if (model.Biography != null)
            {
                string trimmed = model.Biography.Trim();
                UpdateLogRow += $" Biography: {UserRequested.Biography} -> {trimmed}";
                UserRequested.Biography = trimmed;
                UpdateRecord.Biography = trimmed;
            }


            if (!string.IsNullOrWhiteSpace(model.Gender))
            {
                string UpdatedGender = model.Gender.Trim();

                UpdateLogRow += $" Gender: {UserRequested.Gender} -> {UpdatedGender}";
                UserRequested.Gender = UpdatedGender;
                UpdateRecord.Gender = UpdatedGender;
            }

            if (model.LanguageId.HasValue)
            {
                byte UpdatedLanguageId = model.LanguageId.Value;

                UpdateLogRow += $" Preferred LanguageId: {UserRequested.PreferredLanguageId} -> {UpdatedLanguageId}";
                UserRequested.PreferredLanguageId = UpdatedLanguageId;
                UpdateRecord.PreferredLanguageId = UpdatedLanguageId;
            }

            var updateResult = await _userManager.UpdateAsync(UserRequested);

            if (!updateResult.Succeeded)
            {
                _logger.LogError(
                    $"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is update his profile: {UpdateLogRow}. Error Details: {updateResult.Errors}");
                return BadRequest(new
                {
                    message = "Failed to update profile!",
                    errors = updateResult.Errors.Select(e => e.Description)
                });
            }

            _db.UserUpdateRs.Add(UpdateRecord);
            await _db.SaveChangesAsync();


            _logger.LogInformation(
                $"Operation completed. User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has updated his account: {UpdateLogRow}");
            return Ok(new
            {
                message = "Profile updated successfully!",
            });
        }

        [HttpPut, Route("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "You are not logged in!" });

            User? UserRequested = await _userManager.FindByIdAsync(userId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found!" });

            var passwordCheck = await _userManager.CheckPasswordAsync(UserRequested, model.OldPassword);

            if (!passwordCheck)
                return BadRequest(new { message = "The old password is incorrect." });

            if (model.OldPassword == model.NewPassword)
                return BadRequest(new { message = "The new password cannot be the same as the old password." });

            var result = await _userManager.ChangePasswordAsync(UserRequested, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                _logger.LogError(
                    $"Error occurred, while: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] is update his password. Error Details: {result.Errors}");

                return BadRequest(new
                {
                    message = "Failed to change password.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            _logger.LogInformation(
                $"Operation completed: User: [Id: {UserRequested.Id}, Username: {UserRequested.UserName}] has updated his password.");
            return Ok(new { message = "Password changed successfully!" });
        }
    }
}