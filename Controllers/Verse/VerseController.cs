using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using scriptium_backend_dotnet.Controllers.Validation;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Models;
using scriptium_backend_dotnet.Services;
using DTO;

namespace scriptium_backend_dotnet.Controllers.VerseHandler
{
    [ApiController, Route("verse"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class VerseController(ApplicationDBContext db, ICacheService cacheService, ILogger<VerseController> logger)
        : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));

        private readonly ICacheService _cacheService =
            cacheService ?? throw new ArgumentNullException(nameof(cacheService));

        private readonly ILogger<VerseController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet("{ScriptureNumber}/{SectionNumber}/{ChapterNumber}/{VerseNumber}")]
        public async Task<IActionResult> GetVerse([FromRoute] VerseValidatedModel model)
        {
            string requestPath = Request.Path.ToString();

            VerseBothDTO? cache = await _cacheService.GetCachedDataAsync<VerseBothDTO>(requestPath);

            VerseBothDTO data;

            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                data = cache;
            }
            else
            {
                Verse? verse = await _db.Verse
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

                data = verse.ToVerseBothDTO();

                await _cacheService.SetCacheDataAsync(requestPath, data);
                _logger.LogInformation($"Cache data for URL {requestPath} is renewing");
            }

            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId != null &&
                await _db.CollectionVerse.AnyAsync(c =>
                    c.Collection.UserId.ToString() == UserId && c.VerseId == data.Id))
                data.IsSaved = true;


            return Ok(new { data });
        }


        [HttpGet("{ScriptureNumber}/{SectionNumber}/{ChapterNumber}")]
        public async Task<IActionResult> GetChapter([FromRoute] ChapterValidatedModel model)
        {
            string requestPath = Request.Path.ToString();

            ChapterUpperAndOneLevelLowerDTO? cache =
                await _cacheService.GetCachedDataAsync<ChapterUpperAndOneLevelLowerDTO>(requestPath);

            if (cache != null) //Checking cache
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            ChapterUpperAndOneLevelLowerDTO? data = await _db.Chapter
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
                .Select(c => c.ToChapterUpperAndOneLevelLowerDTO())
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

            SectionOneLevelBothDTO? cache = await _cacheService.GetCachedDataAsync<SectionOneLevelBothDTO>(requestPath);

            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }


            SectionOneLevelBothDTO? data = await _db.Section
                .Where(s => s.Number == model.SectionNumber && s.Scripture.Number == model.ScriptureNumber)
                .AsNoTracking()
                .Include(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Chapters).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                .Select(s => s.ToSectionOneLevelBothDTO()).FirstOrDefaultAsync();


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

            ScriptureOneLevelLowerDTO? cache =
                await _cacheService.GetCachedDataAsync<ScriptureOneLevelLowerDTO>(requestPath);

            if (cache != null) //Checking cache
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            //Link queries make it impossible to fetch.
            ScriptureOneLevelLowerDTO? data = await _db.Scripture
                .Where(s => s.Number == model.ScriptureNumber)
                .AsNoTracking()
                .AsSplitQuery()
                .Include(s => s.Sections).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Meanings).ThenInclude(m => m.Language)
                .Select(s => s.ToScriptureOneLevelLowerDTO()).FirstOrDefaultAsync();


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