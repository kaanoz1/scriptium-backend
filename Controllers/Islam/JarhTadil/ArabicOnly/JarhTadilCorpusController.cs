using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Dto.Derived.Islam.JarhTadil.ArabicOnly;
using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.JarhTadil.ArabicOnly;

/// <summary>Corpus-wide endpoints of the Arabic jarh wa ta'dil corpus: overview and sitemap.</summary>
[ApiController, Route(JarhTadilRoutes.ArabicOnly)]
public class JarhTadilCorpusController(
    ScriptiumDbContext db,
    ILogger<JarhTadilCorpusController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    private const int SitemapPageSize = 45_000;

    [HttpGet("overview")]
    public Task<IActionResult> Overview() => Cached<JarhTadilOverview>(async () =>
    {
        var narrators = db.HAoNarrators.AsNoTracking().Where(n => !n.IsStub);

        return new JarhTadilOverview
        {
            Counts = new JarhTadilCounts
            {
                Narrators = await narrators.CountAsync(),
                Companions = await narrators.CountAsync(n => n.RankNo == NarratorRank.Companion),
                Scholars = await db.HAoScholars.CountAsync(),
                Statements = await db.HAoScholarStatements.CountAsync(),
                Links = await db.HAoNarratorLinks.CountAsync(),
            },
            RankDistribution = await narrators
                .GroupBy(n => n.RankNo)
                .Select(g => new CodeCount { Code = (int)g.Key, Count = g.Count() })
                .OrderBy(c => c.Code)
                .ToListAsync(),
            TopNarrators = await narrators
                .OrderByDescending(n => n.HadithCount).ThenBy(n => n.Id)
                .Take(24).ToPlain().ToListAsync(),
            TopScholars = await db.HAoScholars.AsNoTracking()
                .OrderByDescending(s => s.StatementCount).ThenBy(s => s.Id)
                .Take(12).ToPlain().ToListAsync(),
        };
    });

    [HttpGet("sitemap")]
    public Task<IActionResult> Sitemap() => Cached<JarhTadilSitemapSummary>(async () => new JarhTadilSitemapSummary
    {
        Narrators = await db.HAoNarrators.CountAsync(n => !n.IsStub),
        Scholars = await db.HAoScholars.CountAsync(),
        PageSize = SitemapPageSize,
    });

    [HttpGet("sitemap/narrators/{page:int}")]
    public Task<IActionResult> SitemapNarrators([FromRoute, Range(0, 1000)] int page) =>
        Cached<SitemapIdPage>(async () => new SitemapIdPage
        {
            Ids = await db.HAoNarrators.AsNoTracking()
                .Where(n => !n.IsStub)
                .OrderBy(n => n.Id)
                .Skip(page * SitemapPageSize).Take(SitemapPageSize)
                .Select(n => n.Id)
                .ToListAsync(),
        });
}
