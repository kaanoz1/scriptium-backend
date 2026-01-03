using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Islam.Quranic.Verse;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class VerseController(
    ScriptiumDbContext context,
    ILogger<VerseController> logger,
    ICacheService cacheService)
    : Controller
{
    private readonly ScriptiumDbContext _context = context;
    private readonly ILogger<VerseController> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;


    [HttpGet("/verse/{chapterSequence}/{verseSequence}")]
    public async Task<IActionResult> Get([FromRoute] int chapterSequence, [FromRoute] int verseSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await _cacheService.GetCache(cacheKey) is { } cacheData)
            {
                _logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", cacheData.Id);
                return Ok(cacheData.Data);
            }


            var verse = await _context.VersesQ.Include(v => v.Chapter).ThenInclude(c => c.ChapterC)
                .Include(v => v.VerseC)
                .Include(v => v.Words).ThenInclude(w => w.WordC).ThenInclude(wc => wc.Roots)
                .Include(v => v.Words).ThenInclude(w => w.Meanings).ThenInclude(w => w.Language)
                .Include(v => v.Words).ThenInclude(w => w.Transliterations).ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(v => v.Chapter.Sequence == chapterSequence && v.VerseC.Number == verseSequence);

            if (verse is null)
                return NotFound("No verse found.");

            var data = verse.ToDownDto();

            var savedCacheRow = await _cacheService.Save(cacheKey, data);

            _logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An unexpected error occurred while processing verse sequence: {ChapterSequence}:{VerseSequence}",
                chapterSequence, verseSequence);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }
}