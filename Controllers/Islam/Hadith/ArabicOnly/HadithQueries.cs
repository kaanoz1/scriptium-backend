using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using HModel = ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>Shared EF projections and lookups for the hadith controllers.</summary>
public static class HadithQueries
{
    /// <summary>Route prefix of the Arabic-only hadith corpus.</summary>
    public const string Route = "api/islam/hadith/ao";

    /// <summary>Longest matn returned in list rows.</summary>
    public const int MaxListMatn = 600;

    public const int MaxPageSize = 100;

    public static IQueryable<HadithListItem> ToListItems(this IQueryable<HModel.Hadith> q) =>
        q.Select(h => new HadithListItem
        {
            Id = h.Id,
            Number = h.Number,
            Type = h.Type,
            Grade = h.Grade,
            MeaningGroupId = EF.Property<int?>(h, ForeignKeys.MeaningGroup),
            Book = new BookPlain
            {
                Id = h.Book.Id,
                Name = h.Book.Name,
                AuthorName = h.Book.AuthorName,
                AuthorDeathYear = h.Book.AuthorDeathYear,
                CategoryId = EF.Property<BookCategoryKind>(h.Book, ForeignKeys.Category),
                HadithCount = h.Book.HadithCount,
            },
            Matn = h.MatnStart != null && h.MatnEnd != null
                ? h.Text.Substring(h.MatnStart.Value, Math.Min(h.MatnEnd.Value - h.MatnStart.Value, MaxListMatn))
                : h.Text.Substring(0, MaxListMatn),
            MatnTruncated = h.MatnStart != null && h.MatnEnd != null
                ? h.MatnEnd.Value - h.MatnStart.Value > MaxListMatn
                : h.Text.Length > MaxListMatn,
        });

    public static IQueryable<NarratorPlain> ToPlain(this IQueryable<HModel.Narrator> q) =>
        q.Select(n => new NarratorPlain
        {
            Id = n.Id,
            Name = n.Name,
            Nickname = n.Nickname,
            RankNo = n.RankNo,
            Rank = n.Rank,
            Generation = n.Generation,
            DeathYear = n.DeathYear,
            IsStub = n.IsStub,
            HadithCount = n.HadithCount,
        });

    public static IQueryable<BookPlain> ToPlain(this IQueryable<HModel.Book> q) =>
        q.Select(b => new BookPlain
        {
            Id = b.Id,
            Name = b.Name,
            AuthorName = b.AuthorName,
            AuthorDeathYear = b.AuthorDeathYear,
            CategoryId = EF.Property<BookCategoryKind>(b, ForeignKeys.Category),
            HadithCount = b.HadithCount,
        });

    public static IQueryable<MeaningGroupPlain> ToPlain(this IQueryable<HModel.MeaningGroup> q) =>
        q.Select(g => new MeaningGroupPlain
        {
            Id = g.Id,
            Grade = g.Grade,
            IsQudsi = g.IsQudsi,
            Text = g.Text,
            HadithCount = g.HadithCount,
            CompanionCount = g.CompanionCount,
        });

    public static IQueryable<ScholarPlain> ToPlain(this IQueryable<HModel.Scholar> q) =>
        q.Select(s => new ScholarPlain
        {
            Id = s.Id,
            KnownAs = s.KnownAs,
            Name = s.Name,
            DeathYear = s.DeathYear,
            RankNo = s.RankNo,
            StatementCount = s.StatementCount,
        });

    /// <summary>Teacher (or student) links of a narrator, most frequent first.</summary>
    public static IQueryable<NarratorLinkItem> Teachers(this ScriptiumDbContext db, int narratorId) =>
        db.HAoNarratorLinks.TeachersOf(narratorId)
            .OrderByDescending(l => l.ChainCount).ThenBy(l => EF.Property<int>(l, ForeignKeys.Teacher))
            .Select(l => new NarratorLinkItem
            {
                ChainCount = l.ChainCount,
                Narrator = new NarratorPlain
                {
                    Id = l.Teacher.Id,
                    Name = l.Teacher.Name,
                    Nickname = l.Teacher.Nickname,
                    RankNo = l.Teacher.RankNo,
                    Rank = l.Teacher.Rank,
                    Generation = l.Teacher.Generation,
                    DeathYear = l.Teacher.DeathYear,
                    IsStub = l.Teacher.IsStub,
                    HadithCount = l.Teacher.HadithCount,
                },
            });

    public static IQueryable<NarratorLinkItem> Students(this ScriptiumDbContext db, int narratorId) =>
        db.HAoNarratorLinks.StudentsOf(narratorId)
            .OrderByDescending(l => l.ChainCount).ThenBy(l => EF.Property<int>(l, ForeignKeys.Student))
            .Select(l => new NarratorLinkItem
            {
                ChainCount = l.ChainCount,
                Narrator = new NarratorPlain
                {
                    Id = l.Student.Id,
                    Name = l.Student.Name,
                    Nickname = l.Student.Nickname,
                    RankNo = l.Student.RankNo,
                    Rank = l.Student.Rank,
                    Generation = l.Student.Generation,
                    DeathYear = l.Student.DeathYear,
                    IsStub = l.Student.IsStub,
                    HadithCount = l.Student.HadithCount,
                },
            });

    /// <summary>
    /// Root-to-leaf paths (excluding the tree's single top node) of up to
    /// <paramref name="limit"/> topics that link to the meaning group.
    /// </summary>
    public static async Task<List<List<TopicPlain>>> TopicPathsOfGroup(this ScriptiumDbContext db, int groupId,
        int limit = 5)
    {
        var leaves = await db.HAoTopics.AsNoTracking()
            .InGroup(groupId)
            .OrderBy(t => t.Left)
            .Take(limit)
            .Select(t => new { t.TreeId, t.Left, t.Right })
            .ToListAsync();

        var paths = new List<List<TopicPlain>>();
        foreach (var leaf in leaves)
            paths.Add(await db.TopicPath(leaf.TreeId, leaf.Left, leaf.Right, inclusive: true));

        return paths;
    }

    /// <summary>Ancestors (by nested set) of a node, top first, skipping level 0.</summary>
    public static Task<List<TopicPlain>> TopicPath(this ScriptiumDbContext db, int treeId, int left, int right,
        bool inclusive) =>
        db.HAoTopics.AsNoTracking()
            .Where(a => a.TreeId == treeId && a.Level > 0 &&
                        (inclusive ? a.Left <= left && a.Right >= right : a.Left < left && a.Right > right))
            .OrderBy(a => a.Left)
            .Select(a => new TopicPlain { Id = a.Id, Name = a.Name })
            .ToListAsync();

    public static async Task<List<CodeCount>> GradeDistribution(this IQueryable<HModel.Hadith> q) =>
        await q.GroupBy(h => h.Grade)
            .Select(g => new CodeCount { Code = (int)g.Key, Count = g.Count() })
            .OrderBy(c => c.Code)
            .ToListAsync();

    public static async Task<List<CodeCount>> TypeDistribution(this IQueryable<HModel.Hadith> q) =>
        await q.GroupBy(h => h.Type)
            .Select(g => new CodeCount { Code = (int)g.Key, Count = g.Count() })
            .OrderBy(c => c.Code)
            .ToListAsync();

    /// <summary>Hadith counts per book, largest first.</summary>
    public static async Task<List<BookCount>> BookDistribution(this ScriptiumDbContext db,
        IQueryable<HModel.Hadith> q, int limit)
    {
        var counts = await q.GroupBy(h => EF.Property<int>(h, ForeignKeys.Book))
            .Select(g => new { BookId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count).ThenBy(x => x.BookId)
            .Take(limit)
            .ToListAsync();

        var ids = counts.Select(c => c.BookId).ToList();
        var books = await db.HAoBooks.AsNoTracking().Where(b => ids.Contains(b.Id)).ToPlain()
            .ToDictionaryAsync(b => b.Id);

        return counts.Select(c => new BookCount { Book = books[c.BookId], Count = c.Count }).ToList();
    }

    /// <summary>Clamps paging input. Returns (skip, take).</summary>
    public static (int Skip, int Take) Paging(int page, int pageSize)
    {
        var take = Math.Clamp(pageSize, 1, MaxPageSize);
        var p = Math.Max(page, 1);
        return ((p - 1) * take, take);
    }

    public static async Task<Paged<T>> ToPagedAsync<T>(this IQueryable<T> q, int page, int pageSize,
        IQueryable<object>? countQuery = null)
    {
        var (skip, take) = Paging(page, pageSize);
        var total = countQuery is null ? await q.CountAsync() : await countQuery.CountAsync();
        var items = await q.Skip(skip).Take(take).ToListAsync();
        return new Paged<T> { Items = items, Page = Math.Max(page, 1), PageSize = take, TotalCount = total };
    }
}
