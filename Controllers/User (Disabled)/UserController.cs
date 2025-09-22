/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.

 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.DB;
using ScriptiumBackend.Models;
using ScriptiumBackend.Models.Util;

namespace ScriptiumBackend.Controllers.UserHandler
{
    [ApiController, Route("user"), EnableRateLimiting(policyName: "InteractionControllerRateLimit"), Authorize]
    public class UserController(UserManager<User> userManager, ApplicationDbContext db) : ControllerBase
    {
        private readonly UserManager<User> _userManager =
            userManager ?? throw new ArgumentNullException(nameof(userManager));

        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));


        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser([FromRoute] string username)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });

            User? UserFetched = await _userManager.FindByNameAsync(username);

            if (UserFetched == null || UserFetched.IsFrozen.HasValue)
                return NotFound(new { message = "User not found!" });


            bool IsUserInspectingBlocked =
                await _db.Block.AnyAsync(b => b.BlockerId == UserFetched.Id && b.BlockedId == UserRequested.Id);

            if (IsUserInspectingBlocked)
                return NotFound(new { message = "User not found!" });

            var roles = (await _userManager.GetRolesAsync(UserFetched)).ToList();

            long followerCount = await _db.Follow.CountAsync(r =>
                r.FollowedId == UserFetched.Id && !r.Follower.IsFrozen.HasValue && r.Status == FollowStatus.Accepted);
            long followedCount = await _db.Follow.CountAsync(r =>
                r.FollowerId == UserFetched.Id && !r.Followed.IsFrozen.HasValue && r.Status == FollowStatus.Accepted);
            long commentCount = await _db.Comment.CountAsync(c => c.UserId == UserFetched.Id);
            long noteCount = await _db.Note.CountAsync(n => n.UserId == UserFetched.Id);
            long suggestionCount = await _db.Suggestion.CountAsync(s => s.UserId == UserFetched.Id);


            Follow? FollowStatusOfUserInspectingToUserInspected =
                await _db.Follow.FirstOrDefaultAsync(f =>
                    f.FollowerId == UserRequested.Id && f.FollowedId == UserFetched.Id);
            Follow? FollowStatusOfUserInspectedToUserInspecting =
                await _db.Follow.FirstOrDefaultAsync(f =>
                    f.FollowedId == UserRequested.Id && f.FollowerId == UserFetched.Id);

            bool IsUserInspectedBlocked =
                await _db.Block.AnyAsync(b => b.BlockerId == UserRequested.Id && b.BlockedId == UserFetched.Id);


            UserFetchedDto data = new()
            {
                Id = UserFetched.Id,
                Username = UserFetched.UserName,
                Name = UserFetched.Name,
                Surname = UserFetched.Surname,
                Biography = UserFetched.Biography,
                Image = UserFetched.Image,
                FollowedCount = followedCount,
                FollowerCount = followerCount,
                ReflectionCount = commentCount,
                NoteCount = noteCount,
                SuggestionCount = suggestionCount,
                PrivateFrom = UserFetched.IsPrivate,
                CreatedAt = UserFetched.CreatedAt,
                Roles = roles,
                IsFrozen = UserFetched.IsFrozen.HasValue,
                followStatusUserInspecting = FollowStatusOfUserInspectingToUserInspected?.Status.ToString(),
                followStatusUserInspected = FollowStatusOfUserInspectedToUserInspecting?.Status.ToString(),
                IsUserInspectedBlocked = IsUserInspectedBlocked
            };

            return Ok(new { data });
        }

        [HttpGet("comments/{userId}")]
        public async Task<IActionResult> GetUsersComments([FromRoute(Name = "userId")] string userFetchedUserId)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            User? userFetched = await _userManager.FindByIdAsync(userFetchedUserId);

            if (userFetched == null || userFetched.IsFrozen.HasValue)
                return NotFound(new { message = "User not found!" });


            try
            {
                bool IsUserInspectedFollowed = await _db.Follow.AnyAsync(f =>
                    f.FollowerId == UserRequested.Id && f.FollowedId == UserRequested.Id);
                bool IsUserInspectingBlocked =
                    await _db.Block.AnyAsync(b => b.BlockerId == userFetched.Id && b.BlockedId == UserRequested.Id);

                bool isUserInspectedHasPermission = IsUserInspectedFollowed || IsUserInspectingBlocked;

                if (isUserInspectedHasPermission)
                    return NotFound(new { message = "User not found!" });


                List<CommentOwnVerseDto> verseComments = await _db.Comment
                    .Where(c => c.LikeComments.Any(lc => lc.Like.UserId == userFetched.Id) && c.CommentVerse != null)
                    .Include(c => c.CommentVerse).ThenInclude(cv => cv!.Verse).ThenInclude(v => v.Chapter)
                    .ThenInclude(c => c.Section).ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings)
                    .ThenInclude(m => m.Language) //cv is not null. Checked in Where()
                    .Include(c => c.CommentVerse).ThenInclude(cv => cv!.Verse).ThenInclude(v => v.Chapter)
                    .ThenInclude(c => c.Section).ThenInclude(s => s.Meanings)
                    .ThenInclude(m => m.Language) //cv is not null. Checked in Where()
                    .Include(c => c.CommentVerse).ThenInclude(cv => cv!.Verse).ThenInclude(v => v.Chapter)
                    .ThenInclude(c => c.Meanings).ThenInclude(m => m.Language) //cv is not null. Checked in Where()
                    .AsSplitQuery()
                    .Select(comment => comment.ToCommentOwnVerseDto(true, UserRequested)) //Already checked in Where()
                    .ToListAsync();

                List<CommentOwnNoteDto> noteComments = await _db.Comment
                    .Where(c => c.LikeComments.Any(lc => lc.Like.UserId == userFetched.Id) && c.CommentNote != null)
                    .Include(c => c.CommentNote).ThenInclude(cn => cn!.Note).ThenInclude(cv => cv.Verse)
                    .ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Scripture)
                    .ThenInclude(s => s.Meanings).ThenInclude(m => m.Language) //cn is not null. Checked in Where()
                    .Include(c => c.CommentNote).ThenInclude(cn => cn!.Note).ThenInclude(cv => cv.Verse)
                    .ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Meanings)
                    .ThenInclude(m => m.Language) //cn is not null. Checked in Where()
                    .Include(c => c.CommentNote).ThenInclude(cn => cn!.Note).ThenInclude(cv => cv.Verse)
                    .ThenInclude(v => v.Chapter).ThenInclude(c => c.Meanings)
                    .ThenInclude(m => m.Language) //cn is not null. Checked in Where()
                    .AsSplitQuery()
                    .Select(comment => comment.ToCommentOwnNoteDto(true)) //Already checked in Where()
                    .ToListAsync();


                return Ok(new { data = new { verseComments, noteComments } });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }

        [HttpGet("notes/{userId}")]
        public async Task<IActionResult> GetUsersNotes([FromRoute(Name = "userId")] string userFetchedUserId)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized();

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null)
                return NotFound(new { message = "User not found." });


            User? UserFetched = await _userManager.FindByIdAsync(userFetchedUserId);

            if (UserFetched == null || UserFetched.IsFrozen.HasValue)
                return NotFound(new { message = "User not found!" });

            if (UserFetched.IsPrivate.HasValue && UserRequested.Id != UserFetched.Id)
            {
                bool isUserFetchedFollowed = await _db.Follow.AnyAsync(f => f.FollowerId == UserRequested.Id && f.FollowedId == UserFetched.Id && f.Status == FollowStatus.Accepted);
                
                if(!isUserFetchedFollowed)
                    return Unauthorized(new { message = "User not found!" });
                
            }

            try
            {
                bool IsUserInspectingBlocked =
                    await _db.Block.AnyAsync(b => b.BlockerId == UserFetched.Id && b.BlockedId == UserRequested.Id);

                if (IsUserInspectingBlocked)
                    return NotFound(new { message = "User not found!" });


                List<NoteOwnerVerseDto>? data = await _db.Note
                    .Where(n => n.UserId == UserFetched.Id && n.Likes != null)
                    .Include(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section)
                    .ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                    .Include(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section)
                    .ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                    .Include(cv => cv.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Meanings)
                    .ThenInclude(m => m.Language)
                    .Include(n => n.User)
                    .Include(n => n.Likes)
                    .ThenInclude(l => l.Like)
                    .ThenInclude(l => l.User)
                    .AsSplitQuery()
                    .Select(note => note.ToNoteOwnerVerseDto(UserRequested))
                    .ToListAsync();


                return Ok(new { data });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Something went unexpectedly wrong?" });
            }
        }


        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
                return Unauthorized(new { message = "You are not logged in!" });

            User? UserRequested = await _userManager.FindByIdAsync(UserId);

            if (UserRequested == null || UserRequested.IsFrozen.HasValue)
                return NotFound(new { message = "User not found!" });

            var roles = (await _userManager.GetRolesAsync(UserRequested)).ToList();

            UserOwnDto data = new()
            {
                Id = UserRequested.Id,
                Username = UserRequested.UserName,
                Name = UserRequested.Name,
                Surname = UserRequested.Surname,
                Image = UserRequested.Image,
                Email = UserRequested.Email,
                Gender = UserRequested.Gender,
                Biography = UserRequested.Biography,
                CreatedAt = UserRequested.CreatedAt,
                Roles = roles,
                LangId = UserRequested.PreferredLanguageId,
                PrivateFrom = UserRequested.IsPrivate,
            };

            return Ok(new { data });
        }
    }
}

*/