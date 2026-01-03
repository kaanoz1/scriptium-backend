using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Islam.Quranic.Chapter;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class ChapterController(
    ScriptiumDbContext context,
    ILogger<ChapterController> logger,
    ICacheService cacheService)
    : Controller
{
    readonly ScriptiumDbContext _context = context;
    private readonly ILogger<ChapterController> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;


    [HttpGet("/chapter/{chapterSequence}")]
    public async Task<IActionResult> Get([FromRoute] int chapterSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await _cacheService.GetCache(cacheKey) is { } cacheData)
            {
                _logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", cacheData.Id);
                return Ok(cacheData.Data);
            }


            var chapter = await _context.ChaptersQ.Include(c => c.ChapterC).Include(c => c.Verses)
                .ThenInclude(v => v.VerseC)
                .FirstOrDefaultAsync(c => c.Sequence == chapterSequence);

            if (chapter is null)
                return NotFound("No chapter found.");

            var data = chapter.ToChapterWithVersesDto();

            var savedCacheRow = await _cacheService.Save(cacheKey, data);

            _logger.LogInformation("Cache saved. Model name: Chapter. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing chapter sequence: {ChapterSequence}",
                chapterSequence);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }
}