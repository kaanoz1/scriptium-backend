using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;
using ScriptiumBackend.Utils.Text;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>Arabic full-text search over the matn of every hadith.</summary>
[ApiController, Route(HadithQueries.Route)]
public class HadithSearchController(
    ScriptiumDbContext db,
    ILogger<HadithSearchController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    /// <summary>Counting stops here; the UI shows "10,000+".</summary>
    private const int CountCap = 10_000;

    [HttpGet("search")]
    public Task<IActionResult> Search(
        [FromQuery, Required, StringLength(100, MinimumLength = 2)] string query,
        [FromQuery] ArabicSearchMode mode = ArabicSearchMode.Smart,
        [FromQuery] int? book = null,
        [FromQuery, EnumDataType(typeof(BookCategoryKind))] BookCategoryKind? category = null,
        [FromQuery, EnumDataType(typeof(HadithGrade))] HadithGrade? grade = null,
        [FromQuery, EnumDataType(typeof(HadithType))] HadithType? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20) => Uncached<Paged<HadithListItem>>(async () =>
    {
        var terms = ArabicSearch.Terms(ArabicNormalizer.Normalize(query));
        if (terms.Count == 0 || terms.All(t => t.Length < 2))
            return new Paged<HadithListItem> { Items = [], Page = 1, PageSize = pageSize, TotalCount = 0 };

        var tsQuery = ArabicSearch.BuildTsQuery(terms, mode);

        // to_tsvector('simple', search_text) matches the GIN expression index.
        var q = db.HAoHadiths.AsNoTracking()
            .Where(h => EF.Functions.ToTsVector("simple", h.SearchText)
                .Matches(EF.Functions.ToTsQuery("simple", tsQuery)));
        if (book is not null) q = q.InBook(book.Value);
        if (category is not null) q = q.InCategory(category.Value);
        if (grade is not null) q = q.Where(h => h.Grade == grade);
        if (type is not null) q = q.Where(h => h.Type == type);

        // Collect matching ids first, without ORDER BY: that keeps the planner on a
        // bitmap scan of the GIN index (sub-millisecond). "ORDER BY id LIMIT n"
        // makes it walk the primary key and evaluate to_tsvector per row (seconds).
        var matchIds = await q.Select(h => h.Id).Take(CountCap + 1).ToListAsync();
        var total = matchIds.Count;
        matchIds.Sort();

        var (skip, take) = HadithQueries.Paging(page, pageSize);
        var pageIds = matchIds.Take(CountCap).Skip(skip).Take(take).ToList();
        var items = await db.HAoHadiths.AsNoTracking()
            .Where(h => pageIds.Contains(h.Id))
            .OrderBy(h => h.Id)
            .ToListItems()
            .ToListAsync();

        var withHighlights = items.Select(i => new HadithListItem
        {
            Id = i.Id,
            Book = i.Book,
            Number = i.Number,
            Type = i.Type,
            Grade = i.Grade,
            MeaningGroupId = i.MeaningGroupId,
            Matn = i.Matn,
            MatnTruncated = i.MatnTruncated,
            Highlights = ArabicSearch.FindMatches(i.Matn, terms, mode)
                .Select(r => new TextRange { Start = r.Start, End = r.End })
                .ToList(),
        }).ToList();

        return new Paged<HadithListItem>
        {
            Items = withHighlights,
            Page = Math.Max(page, 1),
            PageSize = take,
            TotalCount = Math.Min(total, CountCap),
            TotalIsLowerBound = total > CountCap,
        };
    });
}
