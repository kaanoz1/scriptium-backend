using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Classes.ResponseClasses.Statistics;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;
using ScriptiumBackend.Services.ServiceInterfaces;
using Quranic = ScriptiumBackend.Dto.Derived.Islam.Quranic;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class StatisticsController(
    ScriptiumDbContext db,
    ILogger<StatisticsController> logger,
    ICacheService cacheService) : Controller
{
    [HttpGet("verses/top")]
    public async Task<IActionResult> GetTopVerses()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();
        const int defaultLimit = 50;

        try
        {
            if (await cacheService.Get<TopFetchedCountStatisticsOf<Quranic.Verse.UpToQuran>>(cacheKey) is
                { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var topUrls = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => r.Cache.Url.Contains("/verse/"))
                .GroupBy(r => r.Cache.Url)
                .Select(g => new { Url = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(defaultLimit)
                .ToListAsync();

            if (!topUrls.Any())
            {
                var emptyResult = new TopFetchedCountStatisticsOf<Quranic.Verse.UpToQuran>([]);
                await cacheService.Save(cacheKey, emptyResult, TimeSpan.FromDays(30));
                return Ok(new { data = emptyResult });
            }

            var resultList = new List<FetchCountStatisticsOf<Quranic.Verse.UpToQuran>>();

            foreach (var item in topUrls)
            {
                var segments = item.Url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var verseIndex = Array.IndexOf(segments, "verse");

                if (verseIndex != -1 && verseIndex + 2 < segments.Length &&
                    int.TryParse(segments[verseIndex + 1], out int cSeq) &&
                    int.TryParse(segments[verseIndex + 2], out int vSeq))
                {
                    var verse = await db.QVerses
                        .AsNoTracking()
                        .Include(v => v.Chapter)
                        .Include(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                        .Include(v => v.Words)
                        .Include(v => v.Words).ThenInclude(w => w.Roots)
                        .Include(v => v.Words).ThenInclude(w => w.Meanings).ThenInclude(w => w.Language)
                        .Include(v => v.Words).ThenInclude(w => w.Transliterations).ThenInclude(t => t.Language)
                        .Include(v => v.Transliterations).ThenInclude(t => t.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Footnotes)
                        .Include(v => v.Translations).ThenInclude(t => t.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                        .ThenInclude(a => a.NameTranslations).ThenInclude(nt => nt.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                        .ThenInclude(a => a.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                        .FirstOrDefaultAsync(v => v.Chapter.Sequence == cSeq && v.Number == vSeq);

                    if (verse != null)
                    {
                        resultList.Add(
                            new FetchCountStatisticsOf<Quranic.Verse.UpToQuran>(verse.UpToQuranDto(), item.Count));
                    }
                }
            }

            var responseData = new TopFetchedCountStatisticsOf<Quranic.Verse.UpToQuran>(resultList);
            await cacheService.Save(cacheKey, responseData, TimeSpan.FromDays(1));

            return Ok(new { data = responseData });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching top verses statistics.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("verse/{chapterSequence}/{verseSequence}/daily")]
    public async Task<IActionResult> GetVerseDailyStats(
        [FromRoute] int chapterSequence,
        [FromRoute] int verseSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();
        const int dayHistoryLimit = 365;
        var targetUrlSegment = $"/verse/{chapterSequence}/{verseSequence}";
        var cutOffDate = DateTime.UtcNow.AddDays(-dayHistoryLimit);

        try
        {
            if (await cacheService.Get<DailyStatisticsOf<Quranic.Verse.UpToQuran>>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var verse = await db.QVerses
                .AsNoTracking()
                .Include(v => v.Chapter)
                .Include(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                .Include(v => v.Words)
                .Include(v => v.Words).ThenInclude(w => w.Roots)
                .Include(v => v.Words).ThenInclude(w => w.Meanings).ThenInclude(w => w.Language)
                .Include(v => v.Words).ThenInclude(w => w.Transliterations).ThenInclude(t => t.Language)
                .Include(v => v.Transliterations).ThenInclude(t => t.Language)
                .Include(v => v.Translations).ThenInclude(t => t.Footnotes)
                .Include(v => v.Translations).ThenInclude(t => t.Language)
                .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                .ThenInclude(t => t.Language)
                .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                .ThenInclude(t => t.NameTranslations)
                .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(v => v.Chapter.Sequence == chapterSequence && v.Number == verseSequence);

            if (verse is null) return NotFound("Verse not found.");

            var dailyStats = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => r.Cache.Url.Contains(targetUrlSegment) && r.FetchedAt >= cutOffDate)
                .GroupBy(r => r.FetchedAt.Date)
                .Select(g => new DayRecord
                {
                    Date = DateOnly.FromDateTime(g.Key),
                    Count = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var responseData = new DailyStatisticsOf<Quranic.Verse.UpToQuran>(verse.UpToQuranDto(), dailyStats);
            await cacheService.Save(cacheKey, responseData, TimeSpan.FromDays(1));

            return Ok(new { data = responseData });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching daily stats for verse {C}:{V}", chapterSequence, verseSequence);
            return StatusCode(500, "Internal server error.");
        }
    }


    [HttpGet("chapters/top")]
    public async Task<IActionResult> GetTopChapters()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();
        const int defaultLimit = 50;

        try
        {
            if (await cacheService.Get<TopFetchedCountStatisticsOf<Quranic.Chapter.Complete>>(cacheKey) is
                { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var topUrls = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => r.Cache.Url.Contains("/chapter/") && !r.Cache.Url.Contains("/list"))
                .GroupBy(r => r.Cache.Url)
                .Select(g => new { Url = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(defaultLimit)
                .ToListAsync();

            var resultList = new List<FetchCountStatisticsOf<Quranic.Chapter.Complete>>();

            foreach (var item in topUrls)
            {
                var segments = item.Url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var chapterIndex = Array.IndexOf(segments, "chapter");

                if (chapterIndex != -1 && chapterIndex + 1 < segments.Length &&
                    int.TryParse(segments[chapterIndex + 1], out int seq))
                {
                    var chapter = await db.QChapters
                        .AsNoTracking()
                        .Include(c => c.Meanings).ThenInclude(m => m.Language)
                        .FirstOrDefaultAsync(c => c.Sequence == seq);

                    if (chapter == null) continue;
                    resultList.Add(
                        new FetchCountStatisticsOf<Quranic.Chapter.Complete>(chapter.ToCompleteDto(), item.Count));
                }
            }

            var responseData = new TopFetchedCountStatisticsOf<Quranic.Chapter.Complete>(resultList);
            await cacheService.Save(cacheKey, responseData, TimeSpan.FromDays(1));

            return Ok(new { data = responseData });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching top chapters statistics.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("chapter/{chapterSequence}/daily")]
    public async Task<IActionResult> GetChapterDailyStats([FromRoute] int chapterSequence)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();
        const int dayHistoryLimit = 365;
        var targetUrlSegment = $"/chapter/{chapterSequence}";
        var cutOffDate = DateTime.UtcNow.AddDays(-dayHistoryLimit);

        try
        {
            if (await cacheService.Get<DailyStatisticsOf<Quranic.Chapter.Complete>>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var chapter = await db.QChapters
                .AsNoTracking()
                .Include(c => c.Meanings).ThenInclude(m => m.Language)
                .FirstOrDefaultAsync(c => c.Sequence == chapterSequence);

            if (chapter is null) return NotFound("Chapter not found.");

            var dailyStats = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => r.Cache.Url.Contains(targetUrlSegment) && r.FetchedAt >= cutOffDate)
                .GroupBy(r => r.FetchedAt.Date)
                .Select(g => new DayRecord
                {
                    Date = DateOnly.FromDateTime(g.Key),
                    Count = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var responseData = new DailyStatisticsOf<Quranic.Chapter.Complete>(chapter.ToCompleteDto(), dailyStats);
            await cacheService.Save(cacheKey, responseData, TimeSpan.FromDays(1));

            return Ok(new { data = responseData });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching daily stats for chapter {C}", chapterSequence);
            return StatusCode(500, "Internal server error.");
        }
    }

    /*

        [HttpGet("verses/top/daily")]
    public async Task<IActionResult> GetTopVersesWithHistory()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();
        const int defaultLimit = 20;
        const int dayHistoryLimit = 365;
        var cutOffDate = DateTime.UtcNow.AddDays(-dayHistoryLimit);

        try
        {
            if (await cacheService.Get<TopDetailedStatisticsOf<Quranic.Verse.UpToQuran>>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var topUrlsData = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => r.Cache.Url.Contains("/verse/"))
                .GroupBy(r => r.Cache.Url)
                .Select(g => new { Url = g.Key, TotalCount = g.Count() })
                .OrderByDescending(x => x.TotalCount)
                .Take(defaultLimit)
                .ToListAsync();

            if (topUrlsData.Count == 0)
            {
                var emptyResult = new TopDetailedStatisticsOf<Quranic.Verse.UpToQuran>([]);
                await cacheService.Save(cacheKey, emptyResult, TimeSpan.FromDays(30));
                return Ok(new { data = emptyResult });
            }

            var targetUrls = topUrlsData.Select(x => x.Url).ToList();

            var historyData = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => targetUrls.Contains(r.Cache.Url) && r.FetchedAt >= cutOffDate)
                .GroupBy(r => new { r.Cache.Url, r.FetchedAt.Date })
                .Select(g => new
                {
                    g.Key.Url,
                    g.Key.Date,
                    DailyCount = g.Count()
                })
                .ToListAsync();

            var resultList = new List<DetailedFetchStatisticsOf<Quranic.Verse.UpToQuran>>();

            foreach (var item in topUrlsData)
            {
                var segments = item.Url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var verseIndex = Array.IndexOf(segments, "verse");

                if (verseIndex != -1 && verseIndex + 2 < segments.Length &&
                    int.TryParse(segments[verseIndex + 1], out int cSeq) &&
                    int.TryParse(segments[verseIndex + 2], out int vSeq))
                {
                    var verse = await db.QVerses
                        .AsNoTracking()
                        .Include(v => v.Chapter)
                        .Include(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                        .Include(v => v.Words)
                        .Include(v => v.Words).ThenInclude(w => w.Roots)
                        .Include(v => v.Words).ThenInclude(w => w.Meanings).ThenInclude(w => w.Language)
                        .Include(v => v.Words).ThenInclude(w => w.Transliterations).ThenInclude(t => t.Language)
                        .Include(v => v.Transliterations).ThenInclude(t => t.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Footnotes)
                        .Include(v => v.Translations).ThenInclude(t => t.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                        .ThenInclude(a => a.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors)
                        .ThenInclude(a => a.NameTranslations).ThenInclude(nt => nt.Language)
                        .Include(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                        .FirstOrDefaultAsync(v => v.Chapter.Sequence == cSeq && v.Number == vSeq);

                    if (verse == null) continue;

                    var dailyRecords = historyData
                        .Where(h => h.Url == item.Url)
                        .OrderBy(h => h.Date)
                        .Select(h => new DayRecord
                        {
                            Date = DateOnly.FromDateTime(h.Date),
                            Count = h.DailyCount
                        })
                        .ToList();

                    resultList.Add(new DetailedFetchStatisticsOf<Quranic.Verse.UpToQuran>(
                        verse.UpToQuranDto(),
                        dailyRecords
                    ));
                }
            }

            var responseData = new TopDetailedStatisticsOf<Quranic.Verse.UpToQuran>(resultList);
            //await cacheService.Save(cacheKey, responseData, TimeSpan.FromDays(30));

            return Ok(new { data = responseData });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching top verses with history.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("chapters/top/daily")]
    public async Task<IActionResult> GetTopChaptersWithHistory()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();
        const int defaultLimit = 20;
        const int dayHistoryLimit = 365;
        var cutOffDate = DateTime.UtcNow.AddDays(-dayHistoryLimit);

        try
        {
            if (await cacheService.Get<TopDetailedStatisticsOf<Quranic.Chapter.Complete>>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var topUrlsData = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => r.Cache.Url.Contains("/chapter/") && !r.Cache.Url.Contains("/list"))
                .GroupBy(r => r.Cache.Url)
                .Select(g => new { Url = g.Key, TotalCount = g.Count() })
                .OrderByDescending(x => x.TotalCount)
                .Take(defaultLimit)
                .ToListAsync();

            if (topUrlsData.Count == 0)
            {
                var emptyResult = new TopDetailedStatisticsOf<Quranic.Chapter.Complete>([]);
                await cacheService.Save(cacheKey, emptyResult, TimeSpan.FromDays(30));
                return Ok(new { data = emptyResult });
            }

            var targetUrls = topUrlsData.Select(x => x.Url).ToList();

            var historyData = await db.CacheRecords
                .Include(r => r.Cache)
                .Where(r => targetUrls.Contains(r.Cache.Url) && r.FetchedAt >= cutOffDate)
                .GroupBy(r => new { r.Cache.Url, r.FetchedAt.Date })
                .Select(g => new
                {
                    g.Key.Url,
                    g.Key.Date,
                    DailyCount = g.Count()
                })
                .ToListAsync();

            var resultList = new List<DetailedFetchStatisticsOf<Quranic.Chapter.Complete>>();

            foreach (var item in topUrlsData)
            {
                var segments = item.Url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var chapterIndex = Array.IndexOf(segments, "chapter");

                if (chapterIndex != -1 && chapterIndex + 1 < segments.Length &&
                    int.TryParse(segments[chapterIndex + 1], out int seq))
                {
                    var chapter = await db.QChapters
                        .AsNoTracking()
                        .Include(c => c.Meanings).ThenInclude(m => m.Language)
                        .FirstOrDefaultAsync(c => c.Sequence == seq);

                    if (chapter != null)
                    {
                        var dailyRecords = historyData
                            .Where(h => h.Url == item.Url)
                            .OrderBy(h => h.Date)
                            .Select(h => new DayRecord
                            {
                                Date = DateOnly.FromDateTime(h.Date),
                                Count = h.DailyCount
                            })
                            .ToList();

                        resultList.Add(new DetailedFetchStatisticsOf<Quranic.Chapter.Complete>(
                            chapter.ToCompleteDto(),
                            dailyRecords
                        ));
                    }
                }
            }

            var responseData = new TopDetailedStatisticsOf<Quranic.Chapter.Complete>(resultList);
            //await cacheService.Save(cacheKey, responseData, TimeSpan.FromDays(30));

            return Ok(new { data = responseData });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching top chapters with history.");
            return StatusCode(500, "Internal server error.");
        }
    }

    */
}