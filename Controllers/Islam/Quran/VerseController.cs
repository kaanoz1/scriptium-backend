using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class VerseController(
    ScriptiumDbContext db,
    ILogger<VerseController> logger,
    ICacheService cacheService)
    : Controller
{
    [HttpGet("verse/{chapterSequence}/{verseSequence}")]
    public async Task<IActionResult> Get([FromRoute, Range(1, 114)] int chapterSequence,
        [FromRoute, Range(0, 286)] int verseSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<Both>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var verse = await db.QVerses
                .AsNoTracking()
                .AsSplitQuery()
                .Include(v => v.Chapter)
                .Include(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                .Include(v => v.Words)
                .Include(v => v.Words).ThenInclude(w => w.Roots)
                .Include(v => v.Words).ThenInclude(w => w.Meanings).ThenInclude(w => w.Language)
                .Include(v => v.Words).ThenInclude(w => w.Transliterations).ThenInclude(t => t.Language)
                .Include(v => v.Translations).ThenInclude(t => t.Footnotes)
                .Include(v => v.Translations).ThenInclude(t => t.Language)
                .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                .ThenInclude(t => t.Language)
                .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                .ThenInclude(t => t.NameTranslations)
                .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                .Include(v => v.Transliterations).ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(v => v.Chapter.Sequence == chapterSequence && v.Number == verseSequence);

            if (verse is null)
                return NotFound("No verse found.");

            var data = verse.ToBothDto();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while processing verse sequence: {ChapterSequence}:{VerseSequence}",
                chapterSequence, verseSequence);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpGet("verse/{chapterSequence}/{verseSequence}/plain")]
    public async Task<IActionResult> GetPlainVerseWithItsChapter([FromRoute, Range(1, 114)] int chapterSequence,
        [FromRoute, Range(0, 286)] int verseSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<Down>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var verse = await db.QVerses
                .AsNoTracking()
                .AsSplitQuery()
                .Include(v => v.Chapter)
                .Include(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                .FirstOrDefaultAsync(v => v.Chapter.Sequence == chapterSequence && v.Number == verseSequence);

            if (verse is null)
                return NotFound("No verse found.");

            var data = verse.ToPlainWithPlainChapterDto();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while processing verse sequence: {ChapterSequence}:{VerseSequence}",
                chapterSequence, verseSequence);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }
}