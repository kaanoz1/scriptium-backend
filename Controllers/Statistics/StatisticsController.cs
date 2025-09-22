using System.Text.RegularExpressions;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Models;
using ScriptiumBackend.Services;
using ScriptiumBackend.Interface;
using ScriptiumBackend.DB;

namespace ScriptiumBackend.Controllers.Statistics;

[ApiController, Route("statistics"), EnableRateLimiting(policyName: "StatisticsControllerRateLimit")]
public class StatisticsController(
    ApplicationDbContext db,
    ILogger<StatisticsController> logger,
    ICacheService cacheService) : ControllerBase
{
    private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
    private readonly ILogger<StatisticsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly ICacheService
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));


/* TODO: Will be updated
    [HttpGet]
    public async Task<IActionResult> GetGeneralStatistics()
    {
        string requestPath = Request.Path.ToString();

        try
        {
            GeneralStatisticsDto? cached = await _cacheService.GetCachedDataAsync<GeneralStatisticsDto>(requestPath);
            if (cached != null)
            {
                _logger.LogInformation($"[CACHE HIT] Returning cached general statistics for {requestPath}");
                return Ok(new { data = cached });
            }

            int userCount = await _db.User.CountAsync();
            int requestCount = 0;
            int dailyActiveUserCount =
                await _db.User.CountAsync(u => DateTime.UtcNow - u.LastActive < TimeSpan.FromDays(1));
            int totalSavedVerse = await _db.CollectionVerse.CountAsync();

            GeneralStatisticsDto data = new()
            {
                DailyActiveUserCount = dailyActiveUserCount,
                TotalSavedVerse = totalSavedVerse,
                RequestCount = requestCount,
                UserCount = userCount,
            };

            await _cacheService.SetCacheDataAsync(requestPath, data, TimeSpan.FromDays(10));
            _logger.LogInformation($"Cached general statistics for {requestPath}");

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching general statistics");
            return BadRequest(new { message = "An unexpected error occurred." });
        }
    }
*/

    [HttpGet("verse/popular/fetched")]
    public async Task<IActionResult> GetPopularVerse()
    {
        const int maximumVerseCount = 100;
        const int maximumCacheRecordCount = 10_000_000;
        string requestPath = Request.Path.ToString();

        try
        {
            List<StatisticsOf<VerseUpperMeanDto>>? cached =
                await _cacheService.GetCachedDataAsync<List<StatisticsOf<VerseUpperMeanDto>>>(requestPath);

            if (cached is not null && cached.Count > 1000)
            {
                _logger.LogInformation($"[CACHE HIT] Returning cached popular verses for {requestPath}");
                return Ok(new { data = cached });
            }

            List<CacheRecord> verseCaches = await _db.CacheRecords
                .Include(cr => cr.Cache)
                .Where(cr => cr.Cache.Key.StartsWith("/verse/"))
                .OrderByDescending(cr => cr.Id)
                .Take(maximumCacheRecordCount)
                .ToListAsync();


            Regex verseRegex = new(@"^/verse/(\d+)/(\d+)/(\d+)/(\d+)$");

            Dictionary<string, Dictionary<DateOnly, int>> dayDictionary = new();
            Dictionary<string, VerseIndicatorDto> keyToAddressMap = new();

            foreach (CacheRecord cr in verseCaches)
            {
                string key = cr.Cache.Key;

                Match match = verseRegex.Match(key);
                if (!match.Success)
                    continue;

                int scripture = int.Parse(match.Groups[1].Value);
                int section = int.Parse(match.Groups[2].Value);
                int chapter = int.Parse(match.Groups[3].Value);
                int verse = int.Parse(match.Groups[4].Value);

                DateOnly date = DateOnly.FromDateTime(cr.FetchedAt);

                if (!dayDictionary.ContainsKey(key))
                    dayDictionary[key] = new();

                if (!dayDictionary[key].ContainsKey(date))
                    dayDictionary[key][date] = 0;

                dayDictionary[key][date]++;
                keyToAddressMap[key] = new VerseIndicatorDto
                {
                    Scripture = scripture,
                    Section = section,
                    Chapter = chapter,
                    Verse = verse
                };
            }

            var topKeys = dayDictionary
                .OrderByDescending(kvp => kvp.Value.Values.Sum())
                .Take(maximumVerseCount)
                .Select(kvp => kvp.Key)
                .ToHashSet();

            // Diğer sözlükleri filtrele
            dayDictionary = dayDictionary
                .Where(kvp => topKeys.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            keyToAddressMap = keyToAddressMap
                .Where(kvp => topKeys.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var addresses = topKeys
                .Select(k => keyToAddressMap[k])
                .ToHashSet();

            var allVerses = await _db.Verses
                .Include(v => v.Chapter)
                .ThenInclude(c => c.Section)
                .ThenInclude(s => s.Scripture)
                .Include(v => v.Chapter.Meanings).ThenInclude(m => m.Language)
                .Include(v => v.Chapter.Section.Meanings).ThenInclude(m => m.Language)
                .Include(v => v.Chapter.Section.Scripture.Meanings).ThenInclude(m => m.Language)
                .AsNoTracking()
                .ToListAsync();

            var filteredVerses = allVerses
                .Where(v => addresses.Contains(new VerseIndicatorDto
                {
                    Scripture = v.Chapter.Section.Scripture.Number,
                    Section = v.Chapter.Section.Number,
                    Chapter = v.Chapter.Number,
                    Verse = v.Number
                }))
                .Select(v => v.ToVerseUpperMeanDto())
                .ToList();

            List<StatisticsOf<VerseUpperMeanDto>> data = new();

            foreach (var verse in filteredVerses)
            {
                string key =
                    $"/verse/{verse.Chapter.Section.Scripture.Number}/{verse.Chapter.Section.Number}/{verse.Chapter.Number}/{verse.Number}";

                if (!dayDictionary.TryGetValue(key, out var dailyDict))
                    continue;

                data.Add(new StatisticsOf<VerseUpperMeanDto>
                {
                    Data = verse,
                    dayDictionary = dailyDict
                });
            }

            // 3. Veriyi cache'e yaz
            await _cacheService.SetCacheDataAsync(requestPath, data, TimeSpan.FromDays(10));

            _logger.LogInformation($"Cached popular verses for {requestPath}");

            // 4. Sonuç dön
            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching popular verses.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
/* Disabled since Scriptium does not hold and process user informational data.
    [HttpGet("verse/popular/saved")]
    public async Task<IActionResult> GetMostSavedVerses()
    {
        const int maximumVerseCount = 100;
        string requestPath = Request.Path.ToString();

        try
        {
            List<StatisticsOf<VerseUpperMeanDto>>? cached =
                await _cacheService.GetCachedDataAsync<List<StatisticsOf<VerseUpperMeanDto>>>(requestPath);

            if (cached is not null)
            {
                _logger.LogInformation($"[CACHE HIT] Returning cached saved verses for {requestPath}");
                return Ok(new { data = cached });
            }

            var collectionVerses = await _db.CollectionVerse
                .AsNoTracking()
                .Include(cv => cv.Verse)
                .ThenInclude(v => v.Chapter)
                .ThenInclude(c => c.Section)
                .ThenInclude(s => s.Scripture)
                .Include(cv => cv.Verse.Chapter.Meanings).ThenInclude(m => m.Language)
                .Include(cv => cv.Verse.Chapter.Section.Meanings).ThenInclude(m => m.Language)
                .Include(cv => cv.Verse.Chapter.Section.Scripture.Meanings).ThenInclude(m => m.Language)
                .ToListAsync();

            var grouped = collectionVerses
                .GroupBy(cv => new VerseIndicatorDto
                {
                    Scripture = cv.Verse.Chapter.Section.Scripture.Number,
                    Section = cv.Verse.Chapter.Section.Number,
                    Chapter = cv.Verse.Chapter.Number,
                    Verse = cv.Verse.Number
                })
                .Select(g => new
                {
                    Key = g.Key,
                    Daily = g.GroupBy(x => DateOnly.FromDateTime(x.SavedAt))
                        .ToDictionary(dg => dg.Key, dg => dg.Count()),
                    Total = g.Count(),
                    Verse = g.First().Verse
                })
                .OrderByDescending(g => g.Total)
                .Take(maximumVerseCount)
                .ToList();

            List<StatisticsOf<VerseUpperMeanDto>> data = grouped.Select(g => new StatisticsOf<VerseUpperMeanDto>
            {
                Data = g.Verse.ToVerseUpperMeanDto(),
                dayDictionary = g.Daily
            }).ToList();

            await _cacheService.SetCacheDataAsync(requestPath, data, TimeSpan.FromDays(10));

            _logger.LogInformation($"Cached most saved verses for {requestPath}");

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching most saved verses.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
*/

    [HttpGet("chapter/popular/fetched")]
    public async Task<IActionResult> GetPopularChapters()
    {
        const int maximumChapterCount = 100;
        const int maximumCacheRecordCount = 10_000_000;
        string requestPath = Request.Path.ToString();

        try
        {
            List<StatisticsOf<ChapterUpperMeanDto>>? cached =
                await _cacheService.GetCachedDataAsync<List<StatisticsOf<ChapterUpperMeanDto>>>(requestPath);

            if (cached is not null && cached.Count > 1000)
            {
                _logger.LogInformation($"[CACHE HIT] Returning cached popular chapters for {requestPath}");
                return Ok(new { data = cached });
            }

            List<CacheRecord> chapterCaches = await _db.CacheRecords
                .Include(cr => cr.Cache)
                .Where(cr => cr.Cache.Key.StartsWith("/verse/"))
                .OrderByDescending(cr => cr.Id)
                .Take(maximumCacheRecordCount)
                .ToListAsync();

            Regex chapterRegex = new(@"^/verse/(\d+)/(\d+)/(\d+)/\d+$");

            var chapterKeyDict = new Dictionary<string, Dictionary<DateOnly, int>>();
            Dictionary<string, ChapterIndicatorDto> chapterAddressMap = new();
            foreach (var cr in chapterCaches)
            {
                string key = cr.Cache.Key;
                Match match = chapterRegex.Match(key);
                if (!match.Success) continue;

                int scripture = int.Parse(match.Groups[1].Value);
                int section = int.Parse(match.Groups[2].Value);
                int chapter = int.Parse(match.Groups[3].Value);

                string chapterKey = $"/chapter/{scripture}/{section}/{chapter}";
                DateOnly date = DateOnly.FromDateTime(cr.FetchedAt);

                if (!chapterKeyDict.ContainsKey(chapterKey))
                    chapterKeyDict[chapterKey] = new();

                if (!chapterKeyDict[chapterKey].ContainsKey(date))
                    chapterKeyDict[chapterKey][date] = 0;

                chapterKeyDict[chapterKey][date]++;
                chapterAddressMap[chapterKey] = new ChapterIndicatorDto
                {
                    Scripture = scripture,
                    Chapter = chapter,
                    Section = section
                };
            }

            var topKeys = chapterKeyDict
                .OrderByDescending(kvp => kvp.Value.Values.Sum())
                .Take(maximumChapterCount)
                .Select(kvp => kvp.Key)
                .ToHashSet();

            chapterKeyDict = chapterKeyDict
                .Where(kvp => topKeys.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            chapterAddressMap = chapterAddressMap
                .Where(kvp => topKeys.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var addresses = topKeys
                .Select(k => chapterAddressMap[k])
                .ToHashSet();

            var chapters = await _db.Chapters
                .Include(c => c.Section).ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings)
                .ThenInclude(m => m.Language)
                .Include(c => c.Section.Meanings).ThenInclude(m => m.Language)
                .Include(c => c.Meanings).ThenInclude(m => m.Language)
                .AsNoTracking()
                .ToListAsync();

            var filtered = chapters
                .Where(c => addresses.Contains(new ChapterIndicatorDto
                {
                    Scripture = c.Section.Scripture.Number,
                    Section = c.Section.Number,
                    Chapter = c.Number
                }))
                .Select(c => c.ToChapterUpperMeanDto())
                .ToList();

            var data = new List<StatisticsOf<ChapterUpperMeanDto>>();

            foreach (var chapter in filtered)
            {
                string key = $"/chapter/{chapter.Section.Scripture.Number}/{chapter.Section.Number}/{chapter.Number}";

                if (!chapterKeyDict.TryGetValue(key, out var daily))
                    continue;

                data.Add(new StatisticsOf<ChapterUpperMeanDto>
                {
                    Data = chapter,
                    dayDictionary = daily
                });
            }

            await _cacheService.SetCacheDataAsync(requestPath, data, TimeSpan.FromDays(10));

            _logger.LogInformation($"Cached popular chapters for {requestPath}");

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching popular chapters.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }


    [HttpGet("section/popular/fetched")]
    public async Task<IActionResult> GetPopularSections()
    {
        const int maximumSectionCount = 100;
        const int maximumCacheRecordCount = 10_000_000;
        string requestPath = Request.Path.ToString();

        try
        {
            List<StatisticsOf<SectionUpperMeanDto>>? cached =
                await _cacheService.GetCachedDataAsync<List<StatisticsOf<SectionUpperMeanDto>>>(requestPath);

            if (cached is not null && cached.Count > 1000)
            {
                _logger.LogInformation($"[CACHE HIT] Returning cached popular sections for {requestPath}");
                return Ok(new { data = cached });
            }

            List<CacheRecord> sectionCaches = await _db.CacheRecords
                .Include(cr => cr.Cache)
                .Where(cr => cr.Cache.Key.StartsWith("/verse/"))
                .OrderByDescending(cr => cr.Id)
                .Take(maximumCacheRecordCount)
                .ToListAsync();

            Regex sectionRegex = new(@"^/verse/(\d+)/(\d+)/\d+/\d+$");

            var sectionKeyDict = new Dictionary<string, Dictionary<DateOnly, int>>();
            Dictionary<string, SectionIndicatorDto> sectionAddressMap = new();
            foreach (var cr in sectionCaches)
            {
                string key = cr.Cache.Key;
                Match match = sectionRegex.Match(key);
                if (!match.Success) continue;

                int scripture = int.Parse(match.Groups[1].Value);
                int section = int.Parse(match.Groups[2].Value);

                string sectionKey = $"/section/{scripture}/{section}";
                DateOnly date = DateOnly.FromDateTime(cr.FetchedAt);

                if (!sectionKeyDict.ContainsKey(sectionKey))
                    sectionKeyDict[sectionKey] = new();

                if (!sectionKeyDict[sectionKey].ContainsKey(date))
                    sectionKeyDict[sectionKey][date] = 0;

                sectionKeyDict[sectionKey][date]++;
                sectionAddressMap[sectionKey] = new SectionIndicatorDto
                {
                    Scripture = scripture,
                    Section = section
                };
            }

            var topKeys = sectionKeyDict
                .OrderByDescending(kvp => kvp.Value.Values.Sum())
                .Take(maximumSectionCount)
                .Select(kvp => kvp.Key)
                .ToHashSet();

            sectionKeyDict = sectionKeyDict
                .Where(kvp => topKeys.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            sectionAddressMap = sectionAddressMap
                .Where(kvp => topKeys.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var addresses = topKeys
                .Select(k => sectionAddressMap[k])
                .ToHashSet();

            var sections = await _db.Sections
                .Include(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Meanings).ThenInclude(m => m.Language)
                .AsNoTracking()
                .ToListAsync();

            var filtered = sections
                .Where(s => addresses.Contains(new SectionIndicatorDto
                {
                    Scripture = s.Scripture.Number,
                    Section = s.Number
                }))
                .Select(s => s.ToSectionUpperMeanDto())
                .ToList();

            var data = new List<StatisticsOf<SectionUpperMeanDto>>();

            foreach (var section in filtered)
            {
                string key = $"/section/{section.Scripture.Number}/{section.Number}";

                if (!sectionKeyDict.TryGetValue(key, out var daily))
                    continue;

                data.Add(new StatisticsOf<SectionUpperMeanDto>
                {
                    Data = section,
                    dayDictionary = daily
                });
            }

            await _cacheService.SetCacheDataAsync(requestPath, data, TimeSpan.FromDays(10));
            _logger.LogInformation($"Cached popular sections for {requestPath}");

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching popular sections.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }


    [HttpGet("root/popular/fetched")]
    public async Task<IActionResult> GetPopularRoots()
    {
        const int maximumRootCount = 100;
        const int maximumCacheRecordCount = 10_000_000;
        string requestPath = Request.Path.ToString();

        try
        {
            List<StatisticsOf<RootScriptureConfinedDto>>? cached =
                await _cacheService.GetCachedDataAsync<List<StatisticsOf<RootScriptureConfinedDto>>>(requestPath);

            if (cached is not null && cached.Count > 1000)
            {
                _logger.LogInformation($"[CACHE HIT] Returning cached popular roots for {requestPath}");
                return Ok(new { data = cached });
            }

            List<CacheRecord> rootCaches = await _db.CacheRecords
                .Include(cr => cr.Cache)
                .Where(cr => cr.Cache.Key.StartsWith("/root/"))
                .OrderByDescending(cr => cr.Id)
                .Take(maximumCacheRecordCount)
                .ToListAsync();

            Regex rootRegex = new(@"^/root/(\d+)/(\w+)$");

            var rootKeyDict = new Dictionary<string, Dictionary<DateOnly, int>>();
            var rootAddressMap = new Dictionary<string, RootIdentifierDto>();

            foreach (var cr in rootCaches)
            {
                string key = cr.Cache.Key;
                Match match = rootRegex.Match(key);
                if (!match.Success) continue;

                int scripture = int.Parse(match.Groups[1].Value);
                string latin = match.Groups[2].Value;

                string rootKey = $"/root/{scripture}/{latin}";
                DateOnly date = DateOnly.FromDateTime(cr.FetchedAt);

                if (!rootKeyDict.ContainsKey(rootKey))
                    rootKeyDict[rootKey] = new();

                if (!rootKeyDict[rootKey].ContainsKey(date))
                    rootKeyDict[rootKey][date] = 0;

                rootKeyDict[rootKey][date]++;
                rootAddressMap[rootKey] = new RootIdentifierDto
                {
                    Latin = latin,
                    ScriptureNumber = scripture,
                    Own = string.Empty // will be filled after fetch
                };
            }

            var topKeys = rootKeyDict
                .OrderByDescending(kvp => kvp.Value.Values.Sum())
                .Take(maximumRootCount)
                .Select(kvp => kvp.Key)
                .ToHashSet();

            var identifiers = topKeys.Select(k => rootAddressMap[k]).ToList();

            var roots = await _db.Roots
                .Where(r => identifiers.Any(i => i.Latin == r.Latin && i.ScriptureNumber == r.Scripture.Number))
                .Include(r => r.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .AsNoTracking()
                .ToListAsync();

            var data = new List<StatisticsOf<RootScriptureConfinedDto>>();

            foreach (var root in roots)
            {
                string key = $"/root/{root.Scripture.Number}/{root.Latin}";

                if (!rootKeyDict.TryGetValue(key, out var daily))
                    continue;

                var dto = new RootScriptureConfinedDto
                {
                    Latin = root.Latin,
                    Own = root.Own,
                    Scripture = root.Scripture.ToScriptureUpperMeanDto()
                };

                data.Add(new StatisticsOf<RootScriptureConfinedDto>
                {
                    Data = dto,
                    dayDictionary = daily
                });
            }

            await _cacheService.SetCacheDataAsync(requestPath, data, TimeSpan.FromDays(10));
            _logger.LogInformation($"Cached popular roots for {requestPath}");

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching popular roots.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}