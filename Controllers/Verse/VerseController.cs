using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ScriptiumBackend.Controllers.Validation;
using ScriptiumBackend.Models;
using ScriptiumBackend.Services;
using DTO;
using ScriptiumBackend.Interface;
using ScriptiumBackend.DB;

namespace ScriptiumBackend.Controllers.VerseHandler
{
    [ApiController, Route("verse"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class VerseController(ApplicationDbContext db, ICacheService cacheService, ILogger<VerseController> logger)
        : ControllerBase
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

        private readonly ICacheService _cacheService =
            cacheService ?? throw new ArgumentNullException(nameof(cacheService));

        private readonly ILogger<VerseController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet("{ScriptureNumber}/{SectionNumber}/{ChapterNumber}/{VerseNumber}")]
        public async Task<IActionResult> GetVerse([FromRoute] VerseValidatedModel model)
        {
            string requestPath = Request.Path.ToString();

            VerseBothDto? cache = await _cacheService.GetCachedDataAsync<VerseBothDto>(requestPath);

            VerseBothDto data;

            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                data = cache;
            }
            else
            {
                Verse? verse = await _db.Verses
                    .IgnoreAutoIncludes()
                    .AsNoTracking()
                    .Where(v => v.Number == model.VerseNumber &&
                                v.Chapter.Number == model.ChapterNumber &&
                                v.Chapter.Section.Number == model.SectionNumber &&
                                v.Chapter.Section.Scripture.Number == model.ScriptureNumber)
                    .Include(v => v.Chapter)
                    .ThenInclude(c => c.Section)
                    .ThenInclude(c => c.Scripture)
                    .ThenInclude(c => c.Meanings)
                    .ThenInclude(m => m.Language)
                    .Include(v => v.Chapter)
                    .ThenInclude(c => c.Section.Meanings)
                    .ThenInclude(m => m.Language)
                    .Include(v => v.Chapter)
                    .ThenInclude(c => c.Meanings)
                    .ThenInclude(m => m.Language)
                    .Include(v => v.Words)
                    .ThenInclude(w => w.Roots)
                    .Include(v => v.Transliterations)
                    .ThenInclude(t => t.Language)
                    .Include(v => v.TranslationTexts)
                    .ThenInclude(t => t.Translation)
                    .ThenInclude(t => t.Language)
                    .Include(v => v.TranslationTexts)
                    .ThenInclude(t => t.Translation)
                    .ThenInclude(t => t.TranslatorTranslations)
                    .ThenInclude(tt => tt.Translator)
                    .ThenInclude(t => t.Language)
                    .Include(v => v.TranslationTexts)
                    .ThenInclude(t => t.FootNotes)
                    .ThenInclude(f => f.FootNoteText)
                    .AsSplitQuery() //With the purpose of prevent Cartesian explosion. Reference : https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries
                    .FirstOrDefaultAsync();

                if (verse == null)
                {
                    _logger.LogCritical(
                        $"An verse flaw is found. Verse: [ScriptureNumber: {model.ScriptureNumber}, SectionNumber: {model.SectionNumber}, ChapterNumber: {model.ChapterNumber}, VerseNumber: {model.VerseNumber}] ");
                    return NotFound("There is no verse matches with this information.");
                }

                data = verse.ToVerseBothDto();

                await _cacheService.SetCacheDataAsync(requestPath, data);
                _logger.LogInformation($"Cache data for URL {requestPath} is renewing");
            }

            /* Disabled since Scriptium does not hold and process user information data.
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId != null &&
                await _db.CollectionVerse.AnyAsync(c =>
                    c.Collection.UserId.ToString() == UserId && c.VerseId == data.Id))
                data.IsSaved = true;
            */

            return Ok(new { data });
        }


        [HttpGet("{ScriptureNumber}/{SectionNumber}/{ChapterNumber}")]
        public async Task<IActionResult> GetChapter([FromRoute] ChapterValidatedModel model)
        {
            string requestPath = Request.Path.ToString();

            ChapterUpperAndOneLevelLowerDto? cache =
                await _cacheService.GetCachedDataAsync<ChapterUpperAndOneLevelLowerDto>(requestPath);

            if (cache != null) //Checking cache
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            ChapterUpperAndOneLevelLowerDto? data = await _db.Chapters
                .Where(c => c.Number == model.ChapterNumber &&
                            c.Section.Number == model.SectionNumber &&
                            c.Section.Scripture.Number == model.ScriptureNumber)
                .AsNoTracking()
                .Include(c => c.Section)
                .ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(c => c.Section)
                .ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(c => c.Verses)
                .ThenInclude(v => v.TranslationTexts)
                .ThenInclude(tt => tt.Translation)
                .ThenInclude(t => t.Language)
                .Include(c => c.Verses)
                .ThenInclude(v => v.TranslationTexts)
                .ThenInclude(tt => tt.Translation)
                .ThenInclude(t => t.TranslatorTranslations)
                .ThenInclude(trt => trt.Translator)
                .ThenInclude(tr => tr.Language)
                .Include(c => c.Meanings)
                .ThenInclude(m => m.Language)
                .Include(c => c.Verses)
                .ThenInclude(v => v.TranslationTexts)
                .ThenInclude(tt => tt.FootNotes)
                .ThenInclude(f => f.FootNoteText)
                .Select(c => c.ToChapterUpperAndOneLevelLowerDto())
                .FirstOrDefaultAsync();


            if (data == null)
            {
                _logger.LogCritical(
                    $"A chapter flaw is found. Chapter: [ScriptureNumber: {model.ScriptureNumber}, SectionNumber: {model.SectionNumber}, ChapterNumber: {model.ChapterNumber}] ");
                return NotFound("There is no scripture matches with this information.");
            }


            await _cacheService.SetCacheDataAsync(requestPath, data);
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }


        [HttpGet("{ScriptureNumber}/{SectionNumber}")]
        public async Task<IActionResult> GetSection([FromRoute] SectionValidatedModel model)
        {
            string requestPath = Request.Path.ToString();

            SectionOneLevelBothDto? cache = await _cacheService.GetCachedDataAsync<SectionOneLevelBothDto>(requestPath);

            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }


            SectionOneLevelBothDto? data = await _db.Sections
                .Where(s => s.Number == model.SectionNumber && s.Scripture.Number == model.ScriptureNumber)
                .AsNoTracking()
                .Include(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Chapters).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                .Select(s => s.ToSectionOneLevelBothDto()).FirstOrDefaultAsync();


            if (data == null)
            {
                _logger.LogCritical(
                    $"A section flaw is found. Section: [ScriptureNumber: {model.ScriptureNumber}, SectionNumber: {model.SectionNumber}] ");
                return NotFound("There is no scripture matches with this information.");
            }


            await _cacheService.SetCacheDataAsync(requestPath, data, TimeSpan.FromDays(30));
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }


        [HttpGet("{ScriptureNumber}")]
        public async Task<IActionResult> GetScripture([FromRoute] ScriptureValidatedModel model)
        {
            string requestPath = Request.Path.ToString();

            ScriptureOneLevelLowerDto? cache =
                await _cacheService.GetCachedDataAsync<ScriptureOneLevelLowerDto>(requestPath);

            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }


            ScriptureOneLevelLowerDto? data = await _db.Scriptures
                .Where(s => s.Number == model.ScriptureNumber)
                .AsNoTracking()
                .AsSplitQuery()
                .Include(s => s.Sections).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Meanings).ThenInclude(m => m.Language)
                .Select(s => s.ToScriptureOneLevelLowerDto()).FirstOrDefaultAsync();


            if (data == null)
            {
                _logger.LogCritical($"An scripture flaw is found. Verse: [ScriptureNumber: {model.ScriptureNumber}] ");
                return NotFound("There is no scripture matches with this information.");
            }


            await _cacheService.SetCacheDataAsync(requestPath, data);
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }
    }
}