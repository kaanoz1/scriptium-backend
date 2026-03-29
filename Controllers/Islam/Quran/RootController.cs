using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Root;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class RootController(
    ScriptiumDbContext db,
    ILogger<VerseController> logger,
    ICacheService cacheService)
    : Controller
{
    [HttpGet("root/{latin}")]
    public async Task<IActionResult> Get([FromRoute, StringLength(5, MinimumLength = 3), Required] string latin)
    {
        //TODO: Add validation.

        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<UpToQuran>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var root = await db.QRoots
                .AsNoTracking()
                .AsSplitQuery()
                .Include(r => r.Words)
                .Include(r => r.Words).ThenInclude(w => w.Transliterations).ThenInclude(t => t.Language)
                .Include(r => r.Words).ThenInclude(w => w.Meanings).ThenInclude(m => m.Language)
                .Include(r => r.Words).ThenInclude(w => w.Verse)
                .Include(r => r.Words).ThenInclude(w => w.Verse).ThenInclude(v => v.Translations)
                .ThenInclude(t => t.Translation).ThenInclude(t => t.Authors).ThenInclude(t => t.Language)
                .Include(r => r.Words).ThenInclude(w => w.Verse).ThenInclude(v => v.Translations)
                .ThenInclude(t => t.Translation).ThenInclude(t => t.Authors).ThenInclude(t => t.NameTranslations)
                .Include(r => r.Words).ThenInclude(w => w.Verse).ThenInclude(v => v.Translations)
                .ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                .Include(r => r.Words).ThenInclude(w => w.Verse).ThenInclude(v => v.Translations)
                .ThenInclude(t => t.Footnotes)
                .Include(r => r.Words).ThenInclude(w => w.Verse).ThenInclude(v => v.Transliterations)
                .ThenInclude(t => t.Language)
                .Include(r => r.Words).ThenInclude(w => w.Verse).ThenInclude(v => v.Chapter)
                .ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                .FirstOrDefaultAsync(r => r.TextInLatin == latin);

            if (root is null)
                return NotFound("No root found.");

            var data = root.UpToQuran();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while processing root. Latin: {Latin}",
                latin);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpGet("root/{latin}/plain")]
    public async Task<IActionResult> GetPlain([FromRoute, StringLength(5, MinimumLength = 3), Required] string latin)
    {
        //TODO: Add validation.

        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<UpToVerse>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var root = await db.QRoots
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.TextInLatin == latin);

            if (root is null)
                return NotFound("No root found.");

            var data = root.ToPlainDto();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while processing root. Latin: {Latin}",
                latin);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }


    [HttpGet("root/list")]
    public async Task<IActionResult> List()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<List<Plain>>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var roots = await db.QRoots
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();


            var data = roots.Select(r => r.ToPlainDto()).ToList();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while processing root list.");

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpGet("root/list/advanced")]
    public async Task<IActionResult> ListAdvanced()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<List<WithWordCount>>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var roots = await db.QRoots.Include(r => r.Words).OrderBy(r => r.Words.Count)
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .ToListAsync();


            var data = roots.Select(r => r.ToWithWordCount()).ToList();

            var savedCacheRow = await cacheService.Save(cacheKey, data);

            logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while processing root list.");

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }
}