using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

[ApiController, Route(HadithQueries.Route)]
public class HadithController(
    ScriptiumDbContext db,
    ILogger<HadithController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    private const int OtherNarrationCount = 8;

    /// <summary>A hadith with its chains, narrator mentions, Qur'an quotes and related narrations.</summary>
    [HttpGet("hadith/{bookId:int}/{number:int}")]
    public Task<IActionResult> Get(
        [FromRoute, Range(1, int.MaxValue)] int bookId,
        [FromRoute, Range(1, int.MaxValue)] int number) => Cached<HadithComplete>(async () =>
    {
        var h = await db.HAoHadiths.AsNoTracking()
            .InBook(bookId)
            .Where(x => x.Number == number)
            .Select(x => new
            {
                x.Id, x.Number, x.Page, x.Type, x.Grade, x.TarafText, x.Text, x.MatnStart, x.MatnEnd,
                MeaningGroupId = EF.Property<int?>(x, ForeignKeys.MeaningGroup),
                Book = new BookPlain
                {
                    Id = x.Book.Id,
                    Name = x.Book.Name,
                    AuthorName = x.Book.AuthorName,
                    AuthorDeathYear = x.Book.AuthorDeathYear,
                    CategoryId = EF.Property<BookCategoryKind>(x.Book, ForeignKeys.Category),
                    HadithCount = x.Book.HadithCount,
                },
                CategoryName = x.Book.Category.Name,
            })
            .FirstOrDefaultAsync();
        if (h is null) return null;

        var mentions = await db.HAoHadithNarratorMentions.AsNoTracking()
            .OfHadith(h.Id)
            .OrderBy(m => m.Sequence)
            .Select(m => new NarratorMention { Start = m.Start, End = m.End, NarratorId = EF.Property<int?>(m, ForeignKeys.Narrator) })
            .ToListAsync();

        var quotes = await db.HAoHadithQuranQuotes.AsNoTracking()
            .OfHadith(h.Id)
            .OrderBy(q => q.Sequence)
            .Select(q => new QuranQuote
            {
                Start = q.Start, End = q.End, Chapter = q.Chapter, VerseStart = q.VerseStart, VerseEnd = q.VerseEnd,
            })
            .ToListAsync();

        var sanads = await db.HAoSanads.AsNoTracking()
            .OfHadith(h.Id)
            .OrderBy(s => s.Id)
            .Select(s => new SanadComplete
            {
                Id = s.Id,
                Grade = s.Grade,
                Verdict = s.Verdict,
                MaxRank = s.MaxRank,
                Length = s.Length,
                Narrators = s.Narrators.OrderBy(n => n.Position)
                    .Select(n => new SanadLink { Position = n.Position, NarratorId = EF.Property<int>(n, ForeignKeys.Narrator) })
                    .ToList(),
            })
            .ToListAsync();

        var narratorIds = mentions.Where(m => m.NarratorId != null).Select(m => m.NarratorId!.Value)
            .Concat(sanads.SelectMany(s => s.Narrators.Select(n => n.NarratorId)))
            .Distinct()
            .ToList();
        var narrators = await db.HAoNarrators.AsNoTracking()
            .Where(n => narratorIds.Contains(n.Id))
            .ToPlain()
            .ToListAsync();

        MeaningGroupPlain? group = null;
        List<List<TopicPlain>> topicPaths = [];
        List<HadithListItem> others = [];
        if (h.MeaningGroupId is { } gid)
        {
            group = await db.HAoMeaningGroups.AsNoTracking().Where(g => g.Id == gid).ToPlain().FirstOrDefaultAsync();
            topicPaths = await db.TopicPathsOfGroup(gid);
            others = await db.HAoHadiths.AsNoTracking()
                .InGroup(gid)
                .Where(x => x.Id != h.Id)
                .InBookOrder()
                .Take(OtherNarrationCount)
                .ToListItems()
                .ToListAsync();
        }

        var previous = await db.HAoHadiths
            .InBook(bookId).Where(x => x.Number < number)
            .MaxAsync(x => (int?)x.Number);
        var next = await db.HAoHadiths
            .InBook(bookId).Where(x => x.Number > number)
            .MinAsync(x => (int?)x.Number);

        return new HadithComplete
        {
            Id = h.Id,
            Book = h.Book,
            CategoryName = h.CategoryName,
            Number = h.Number,
            Page = h.Page,
            Type = h.Type,
            Grade = h.Grade,
            TarafText = h.TarafText,
            Text = h.Text,
            MatnStart = h.MatnStart,
            MatnEnd = h.MatnEnd,
            Mentions = mentions,
            Quotes = quotes,
            Sanads = sanads,
            Narrators = narrators,
            MeaningGroup = group,
            TopicPaths = topicPaths,
            OtherNarrations = others,
            PreviousNumber = previous,
            NextNumber = next,
        };
    });

    /// <summary>All chains of this hadith merged into one graph.</summary>
    [HttpGet("hadith/{bookId:int}/{number:int}/isnad")]
    public Task<IActionResult> Isnad([FromRoute] int bookId, [FromRoute] int number) => Cached<IsnadGraph>(async () =>
    {
        var id = await db.HAoHadiths.InBook(bookId).Where(x => x.Number == number)
            .Select(x => (int?)x.Id).FirstOrDefaultAsync();
        return id is null ? null : await IsnadGraphBuilder.ForHadith(db, id.Value);
    });
}
