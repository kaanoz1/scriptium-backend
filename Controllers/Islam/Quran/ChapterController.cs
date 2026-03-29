using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class ChapterController(
    ScriptiumDbContext db,
    ILogger<ChapterController> logger,
    ICacheService cacheService)
    : Controller
{
    [HttpGet("chapter/{chapterSequence}")]
    public async Task<IActionResult> Get([FromRoute, Range(1, 114)] int chapterSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<WithVerses>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var chapter = await db.QChapters
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Verses)
                .Include(c => c.Meanings)
                .ThenInclude(m => m.Language)
                .Include(v => v.Verses).ThenInclude(v => v.Translations).ThenInclude(t => t.Footnotes)
                .Include(v => v.Verses).ThenInclude(v => v.Translations).ThenInclude(t => t.Language)
                .Include(v => v.Verses).ThenInclude(v => v.Translations).ThenInclude(t => t.Translation)
                .ThenInclude(t => t.Authors).ThenInclude(t => t.Language)
                .Include(v => v.Verses).ThenInclude(v => v.Translations).ThenInclude(t => t.Translation)
                .ThenInclude(t => t.Authors).ThenInclude(t => t.NameTranslations)
                .Include(v => v.Verses).ThenInclude(v => v.Translations).ThenInclude(t => t.Translation)
                .ThenInclude(t => t.Language)
                .Include(c => c.Verses).ThenInclude(v => v.Transliterations).ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(c => c.Sequence == chapterSequence);

            if (chapter is null)
                return NotFound("No chapter found.");

            var data = chapter.ToChapterWithVersesDto();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Chapter. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing chapter sequence: {ChapterSequence}",
                chapterSequence);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpGet("chapter/{chapterSequence}/plain")]
    public async Task<IActionResult> GetPlain([FromRoute, Range(1, 114)] int chapterSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<WithVerses>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var chapter = await db.QChapters
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Meanings)
                .ThenInclude(m => m.Language)
                .FirstOrDefaultAsync(c => c.Sequence == chapterSequence);

            if (chapter is null)
                return NotFound("No chapter found.");

            var data = chapter.ToCompleteDto();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Chapter. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing chapter sequence: {ChapterSequence}",
                chapterSequence);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpGet("chapter/list")]
    public async Task<IActionResult> GetAll()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<List<WithVerseCount>>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var chapters = await db.QChapters
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Meanings)
                .ThenInclude(m => m.Language)
                .Include(c => c.Verses).ToListAsync();

            var data = chapters.Select(c => c.ToVerseCountDto()).ToList();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: List<Chapter>. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing chapter list");

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }
}