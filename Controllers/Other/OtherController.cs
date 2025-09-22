using AngleSharp.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.DB;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Controllers.Other;

[ApiController, Route("other"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
public class OtherController(ApplicationDbContext db) : ControllerBase
{
    private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    private const int PER_SITEMAP = 50_000;
    private readonly string _baseUrl = "https://scriptium.net";

    [HttpGet("/sitemap-index.xml")]
    public async Task<IActionResult> GetSitemapIndex(CancellationToken ct)
    {
        // Her kümenin sayısını topla (hız için COUNT(*) sorguları):
        var scriptureCount = await _db.Scriptures.AsNoTracking().CountAsync(ct);
        var sectionCount = await _db.Sections.AsNoTracking().CountAsync(ct);
        var chapterCount = await _db.Chapters.AsNoTracking().CountAsync(ct);
        var verseCount = await _db.Verses.AsNoTracking().CountAsync(ct);
        var rootCount = await _db.Roots.AsNoTracking().CountAsync(ct);

        // Kitap/BookNode varsa:
        var bookCount = await _db.Books.AsNoTracking().CountAsync(ct);

        // Statikler:
        var staticCount = 5; // /, /books, /about, /discord, /statistics

        var total = scriptureCount + sectionCount + chapterCount + verseCount + rootCount + bookCount + staticCount;
        var partCount = Math.Max(1, (int)Math.Ceiling(total / (double)PER_SITEMAP));

        var indexXml = SitemapIndexGenerator.GenerateIndexXml(_baseUrl, partCount);
        return new ContentResult { Content = indexXml, ContentType = "application/xml", StatusCode = 200 };
    }

    // PARÇA: /sitemap/{part}.xml
    [HttpGet("/sitemap/{part:int}.xml")]
    public async Task<IActionResult> GetSitemapPart([FromRoute] int part, CancellationToken ct)
    {
        if (part < 0) return NotFound();

        var skip = part * PER_SITEMAP;
        var take = PER_SITEMAP;

        var staticUrls = new[]
        {
            new SitemapUrl { OrderKey = 0, Id = 1, Loc = $"{_baseUrl}/", },
            new SitemapUrl { OrderKey = 0, Id = 2, Loc = $"{_baseUrl}/books", },
            new SitemapUrl { OrderKey = 0, Id = 3, Loc = $"{_baseUrl}/about", },
            new SitemapUrl { OrderKey = 0, Id = 4, Loc = $"{_baseUrl}/discord", },
            new SitemapUrl { OrderKey = 0, Id = 5, Loc = $"{_baseUrl}/statistics", },
        }.AsQueryable();

        // 2) SCRIPTURE (OrderKey = 1)
        var qScripture = _db.Scriptures.AsNoTracking()
            .Select(s => new SitemapUrl
            {
                OrderKey = 1,
                Id = s.Id,
                Loc = $"{_baseUrl}/{s.Code}",
            });

        // 3) SECTION (OrderKey = 2)
        var qSection = _db.Sections.AsNoTracking()
            .Select(s => new SitemapUrl
            {
                OrderKey = 2,
                Id = s.Id,
                Loc = $"{_baseUrl}/{s.Scripture.Code}/{s.Number}",
            });

        // 4) CHAPTER (OrderKey = 3)
        var qChapter = _db.Chapters.AsNoTracking()
            .Select(c => new SitemapUrl
            {
                OrderKey = 3,
                Id = c.Id,
                Loc = $"{_baseUrl}/{c.Section.Scripture.Code}/{c.Section.Number}/{c.Number}",
            });

        // 5) VERSE (OrderKey = 4)
        var qVerse = _db.Verses.AsNoTracking()
            .Select(v => new SitemapUrl
            {
                OrderKey = 4,
                Id = v.Id,
                Loc =
                    $"{_baseUrl}/{v.Chapter.Section.Scripture.Code}/{v.Chapter.Section.Number}/{v.Chapter.Number}/{v.Number}",
            });

        // 6) ROOT (OrderKey = 5)
        var qRoot = _db.Roots.AsNoTracking()
            .Select(r => new SitemapUrl
            {
                OrderKey = 5,
                Id = r.Id,
                Loc = $"{_baseUrl}/{r.Scripture.Code}/{r.Latin}",
            });

        // 7) BOOK (OrderKey = 6)  // BURAYI modeline göre düzelt:
        // Örn: en anlamı slug/text olacak şekilde explicit seç
        var qBook = _db.Books.AsNoTracking()
            .Select(b => new SitemapUrl
            {
                OrderKey = 6,
                Id = b.Id,
                Loc = $"{_baseUrl}/books/{b.Meanings.FirstOrDefault(m => m.Language.LangCode == "en")}"
            });

        var unified = staticUrls
            .Concat(qScripture)
            .Concat(qSection)
            .Concat(qChapter)
            .Concat(qVerse)
            .Concat(qRoot)
            .Concat(qBook)
            .OrderBy(x => x.OrderKey).ThenBy(x => x.Id);

        var page = await unified.Skip(skip).Take(take).ToListAsync(ct);
        if (page.Count == 0) return NotFound();

        // SitemapXml’e dönüştür
        var urls = page.Select(x => new SitemapUrl { Loc = x.Loc, LastMod = x.LastMod }).ToList();
        var xml = SitemapGenerator.GenerateSitemapXml(urls);

        return new ContentResult { Content = xml, ContentType = "application/xml", StatusCode = 200 };
    }
}