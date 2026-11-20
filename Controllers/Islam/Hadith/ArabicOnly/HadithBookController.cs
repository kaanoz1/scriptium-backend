using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

[ApiController, Route(HadithQueries.Route)]
public class HadithBookController(
    ScriptiumDbContext db,
    ILogger<HadithBookController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    [HttpGet("book/{bookId:int}")]
    public Task<IActionResult> Get([FromRoute, Range(1, int.MaxValue)] int bookId) => Cached<BookComplete>(async () =>
    {
        var book = await db.HAoBooks.AsNoTracking()
            .Where(b => b.Id == bookId)
            .Select(b => new
            {
                b.Id, b.Name, b.AuthorName, b.AuthorDeathYear, b.City, b.Publisher, b.Edition, b.HadithCount,
                Category = new BookCategoryPlain { Id = b.Category.Id, Name = b.Category.Name },
            })
            .FirstOrDefaultAsync();
        if (book is null) return null;

        var hadiths = db.HAoHadiths.InBook(bookId);
        var numbers = await hadiths.GroupBy(_ => 1)
            .Select(g => new { First = g.Min(h => h.Number), Last = g.Max(h => h.Number) })
            .FirstOrDefaultAsync();

        return new BookComplete
        {
            Id = book.Id,
            Name = book.Name,
            AuthorName = book.AuthorName,
            AuthorDeathYear = book.AuthorDeathYear,
            City = book.City,
            Publisher = book.Publisher,
            Edition = book.Edition,
            Category = book.Category,
            HadithCount = await hadiths.CountAsync(),
            FirstNumber = numbers?.First ?? 0,
            LastNumber = numbers?.Last ?? 0,
            GradeDistribution = await hadiths.GradeDistribution(),
            TypeDistribution = await hadiths.TypeDistribution(),
            ChainCount = await db.HAoSanads.InBook(bookId).CountAsync(),
            QuranQuoteCount = await db.HAoHadithQuranQuotes.InBook(bookId).CountAsync(),
        };
    });

    /// <summary>Hadiths of a book in book order, optionally filtered.</summary>
    [HttpGet("book/{bookId:int}/hadiths")]
    public Task<IActionResult> Hadiths(
        [FromRoute, Range(1, int.MaxValue)] int bookId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery, EnumDataType(typeof(HadithGrade))] HadithGrade? grade = null,
        [FromQuery, EnumDataType(typeof(HadithType))] HadithType? type = null) => Uncached(async () =>
    {
        if (!await db.HAoBooks.AnyAsync(b => b.Id == bookId)) return null;

        var q = db.HAoHadiths.AsNoTracking().InBook(bookId);
        if (grade is not null) q = q.Where(h => h.Grade == grade);
        if (type is not null) q = q.Where(h => h.Type == type);

        return await q.OrderBy(h => h.Number).ToListItems().ToPagedAsync(page, pageSize);
    });

    /// <summary>Page of the book list containing hadith number <paramref name="number"/> (for "jump to").</summary>
    [HttpGet("book/{bookId:int}/locate/{number:int}")]
    public Task<IActionResult> Locate([FromRoute] int bookId, [FromRoute] int number, [FromQuery] int pageSize = 25) =>
        Uncached(async () =>
        {
            var exists = await db.HAoHadiths.InBook(bookId).AnyAsync(h => h.Number == number);
            if (!exists) return null;
            var before = await db.HAoHadiths.InBook(bookId).CountAsync(h => h.Number < number);
            var (_, take) = HadithQueries.Paging(1, pageSize);
            return new LocateResult { Page = before / take + 1 };
        });
}

public class LocateResult
{
    public required int Page { get; init; }
}
