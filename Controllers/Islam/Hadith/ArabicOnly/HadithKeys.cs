using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using HadithModel = ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Hadith;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>
/// Filters by foreign key that read the FK column itself.
/// <para>
/// The models only have navigations, and EF translates <c>h.Book.Id == id</c>
/// into a JOIN on the book table. These helpers compare the shadow FK column
/// instead (<see cref="ForeignKeys"/>), which keeps the queries on their
/// indexes - this matters on the 700k-row hadith and 4.4M-row chain tables.
/// </para>
/// </summary>
public static class HadithKeys
{
    // ----- Hadith -----

    public static IQueryable<HadithModel> InBook(this IQueryable<HadithModel> q, int bookId) =>
        q.Where(h => EF.Property<int>(h, ForeignKeys.Book) == bookId);

    public static IQueryable<HadithModel> InBooks(this IQueryable<HadithModel> q, int[] bookIds) =>
        q.Where(h => bookIds.Contains(EF.Property<int>(h, ForeignKeys.Book)));

    /// <summary>Needs the book row (the category lives there), so this one joins.</summary>
    public static IQueryable<HadithModel> InCategory(this IQueryable<HadithModel> q, BookCategoryKind category) =>
        q.Where(h => EF.Property<BookCategoryKind>(h.Book, ForeignKeys.Category) == category);

    public static IQueryable<HadithModel> InGroup(this IQueryable<HadithModel> q, int groupId) =>
        q.Where(h => EF.Property<int?>(h, ForeignKeys.MeaningGroup) == groupId);

    /// <summary>Book order: by book, then by number inside the book.</summary>
    public static IOrderedQueryable<HadithModel> InBookOrder(this IQueryable<HadithModel> q) =>
        q.OrderBy(h => EF.Property<int>(h, ForeignKeys.Book)).ThenBy(h => h.Number);

    // ----- Rows belonging to one hadith -----

    public static IQueryable<Sanad> OfHadith(this IQueryable<Sanad> q, int hadithId) =>
        q.Where(s => EF.Property<int>(s, ForeignKeys.Hadith) == hadithId);

    public static IQueryable<HadithNarratorMention> OfHadith(this IQueryable<HadithNarratorMention> q, int hadithId) =>
        q.Where(m => EF.Property<int>(m, ForeignKeys.Hadith) == hadithId);

    public static IQueryable<HadithQuranQuote> OfHadith(this IQueryable<HadithQuranQuote> q, int hadithId) =>
        q.Where(x => EF.Property<int>(x, ForeignKeys.Hadith) == hadithId);

    // ----- Chains -----

    public static IQueryable<Sanad> InGroup(this IQueryable<Sanad> q, int groupId) =>
        q.Where(s => EF.Property<int?>(s, ForeignKeys.MeaningGroup) == groupId);

    public static IQueryable<Sanad> InBook(this IQueryable<Sanad> q, int bookId) =>
        q.Where(s => EF.Property<int>(s.Hadith, ForeignKeys.Book) == bookId);

    public static IQueryable<HadithQuranQuote> InBook(this IQueryable<HadithQuranQuote> q, int bookId) =>
        q.Where(x => EF.Property<int>(x.Hadith, ForeignKeys.Book) == bookId);

    /// <summary>Ids of the hadiths whose chains contain the narrator (index-only scan).</summary>
    public static IQueryable<int> HadithIdsOfNarrator(this IQueryable<SanadNarrator> q, int narratorId) =>
        q.Where(sn => EF.Property<int>(sn, ForeignKeys.Narrator) == narratorId)
            .Select(sn => EF.Property<int>(sn, ForeignKeys.Hadith))
            .Distinct();

    // ----- Narrators, critics, links -----

    public static IQueryable<ScholarStatement> AboutNarrator(this IQueryable<ScholarStatement> q, int narratorId) =>
        q.Where(s => EF.Property<int>(s, ForeignKeys.Narrator) == narratorId);

    public static IQueryable<ScholarStatement> ByScholar(this IQueryable<ScholarStatement> q, int scholarId) =>
        q.Where(s => EF.Property<int>(s, ForeignKeys.Scholar) == scholarId);

    /// <summary>Links in which the narrator is the student, i.e. their teachers.</summary>
    public static IQueryable<NarratorLink> TeachersOf(this IQueryable<NarratorLink> q, int narratorId) =>
        q.Where(l => EF.Property<int>(l, ForeignKeys.Student) == narratorId);

    /// <summary>Links in which the narrator is the teacher, i.e. their students.</summary>
    public static IQueryable<NarratorLink> StudentsOf(this IQueryable<NarratorLink> q, int narratorId) =>
        q.Where(l => EF.Property<int>(l, ForeignKeys.Teacher) == narratorId);

    // ----- Subject tree -----

    /// <summary>The tree's top node(s).</summary>
    public static IQueryable<Topic> Roots(this IQueryable<Topic> q) =>
        q.Where(t => EF.Property<int?>(t, ForeignKeys.ParentTopic) == null);

    public static IQueryable<Topic> ChildrenOf(this IQueryable<Topic> q, List<int> parentIds) =>
        q.Where(t => parentIds.Contains(EF.Property<int>(t, ForeignKeys.ParentTopic)));

    public static IQueryable<Topic> InGroup(this IQueryable<Topic> q, int groupId) =>
        q.Where(t => EF.Property<int?>(t, ForeignKeys.MeaningGroup) == groupId);
}
