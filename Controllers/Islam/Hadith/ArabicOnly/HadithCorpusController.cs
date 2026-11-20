using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;
using HModel = ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>Corpus-wide endpoints: overview, book index, Qur'an links, sitemap.</summary>
[ApiController, Route(HadithQueries.Route)]
public class HadithCorpusController(
    ScriptiumDbContext db,
    ILogger<HadithCorpusController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    private const int SitemapPageSize = 45_000;

    [HttpGet("overview")]
    public Task<IActionResult> Overview() => Cached<CorpusOverview>(async () =>
    {
        var counts = new CorpusCounts
        {
            Books = await db.HAoBooks.CountAsync(),
            Hadiths = await db.HAoHadiths.CountAsync(),
            Chains = await db.HAoSanads.CountAsync(),
            Narrators = await db.HAoNarrators.CountAsync(n => !n.IsStub),
            Scholars = await db.HAoScholars.CountAsync(),
            Statements = await db.HAoScholarStatements.CountAsync(),
            MeaningGroups = await db.HAoMeaningGroups.CountAsync(),
            Topics = await db.HAoTopics.CountAsync(),
            QuranQuotes = await db.HAoHadithQuranQuotes.CountAsync(q => q.Chapter != null),
        };

        var rootTopics = await RootTopics();

        return new CorpusOverview
        {
            Counts = counts,
            Categories = await Categories(),
            GradeDistribution = await db.HAoHadiths.GradeDistribution(),
            TypeDistribution = await db.HAoHadiths.TypeDistribution(),
            NarratorRankDistribution = await db.HAoNarrators.Where(n => !n.IsStub)
                .GroupBy(n => n.RankNo)
                .Select(g => new CodeCount { Code = (int)g.Key, Count = g.Count() })
                .OrderBy(c => c.Code)
                .ToListAsync(),
            TopNarrators = await db.HAoNarrators.AsNoTracking()
                .OrderByDescending(n => n.HadithCount).ThenBy(n => n.Id)
                .Take(24).ToPlain().ToListAsync(),
            RootTopics = rootTopics,
        };
    });

    [HttpGet("books")]
    public Task<IActionResult> Books() => Cached<CategoryList>(async () => new CategoryList { Categories = await Categories() });

    /// <summary>Hadiths quoting a given Qur'an verse.</summary>
    [HttpGet("quran/{chapter:int}/{verse:int}/hadiths")]
    public Task<IActionResult> QuranVerseHadiths(
        [FromRoute, Range(1, 114)] short chapter,
        [FromRoute, Range(1, 286)] short verse,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Uncached(async () =>
        {
            var hadithIds = db.HAoHadithQuranQuotes
                .Where(q => q.Chapter == chapter && q.VerseStart <= verse && q.VerseEnd >= verse)
                .Select(q => EF.Property<int>(q, ForeignKeys.Hadith))
                .Distinct();

            var query = db.HAoHadiths.AsNoTracking()
                .Where(h => hadithIds.Contains(h.Id))
                .InBookOrder()
                .ToListItems();

            return await query.ToPagedAsync(page, pageSize);
        });
    }

    [HttpGet("sitemap")]
    public Task<IActionResult> Sitemap() => Cached<SitemapSummary>(async () => new SitemapSummary
    {
        Hadiths = await db.HAoHadiths.CountAsync(),
        Books = await db.HAoBooks.CountAsync(),
        MeaningGroups = await db.HAoMeaningGroups.CountAsync(g => g.HadithCount > 0),
        PageSize = SitemapPageSize,
    });

    [HttpGet("sitemap/hadiths/{page:int}")]
    public Task<IActionResult> SitemapHadiths([FromRoute, Range(0, 1000)] int page) =>
        Cached<SitemapHadithPage>(async () =>
        {
            var rows = await db.HAoHadiths.AsNoTracking()
                .OrderBy(h => h.Id)
                .Skip(page * SitemapPageSize).Take(SitemapPageSize)
                .Select(h => new { BookId = EF.Property<int>(h, ForeignKeys.Book), h.Number })
                .ToListAsync();
            return new SitemapHadithPage { Hadiths = rows.Select(r => new[] { r.BookId, r.Number }).ToList() };
        });

    [HttpGet("sitemap/groups/{page:int}")]
    public Task<IActionResult> SitemapGroups([FromRoute, Range(0, 1000)] int page) =>
        Cached<SitemapIdPage>(async () => new SitemapIdPage
        {
            Ids = await db.HAoMeaningGroups.AsNoTracking()
                .Where(g => g.HadithCount > 0)
                .OrderBy(g => g.Id)
                .Skip(page * SitemapPageSize).Take(SitemapPageSize)
                .Select(g => g.Id)
                .ToListAsync(),
        });

    private async Task<List<BookCategoryWithBooks>> Categories()
    {
        var books = await db.HAoBooks.AsNoTracking().OrderBy(b => b.Id).ToPlain().ToListAsync();
        var categories = await db.HAoBookCategories.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
        return categories.Select(c => new BookCategoryWithBooks
        {
            Id = c.Id,
            Name = c.Name,
            Books = books.Where(b => b.CategoryId == c.Id).ToList(),
        }).ToList();
    }

    private async Task<List<TopicChild>> RootTopics()
    {
        var root = await db.HAoTopics.AsNoTracking().Roots().Select(t => t.Id).ToListAsync();
        return await HadithTopicController.Children(db, root);
    }
}

public class CategoryList : Interfaces.ICacheable
{
    public required List<BookCategoryWithBooks> Categories { get; init; }
}
