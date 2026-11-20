using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;
using Hadith = ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

namespace ScriptiumBackend.Db.Configurations.Islam;

/// <summary>
/// Schema of the Arabic-only hadith corpus (<c>i_h_ao_*</c> tables).
/// <para>
/// Primary keys are the ids of the source dataset (never generated), so the
/// data loader can insert idempotently and URLs stay stable across reloads.
/// </para>
/// <para>
/// Models expose navigations only; foreign-key columns are shadow properties
/// named after the column (e.g. <c>ForeignKeys.Book</c>). Queries can still
/// filter by key through the navigation (<c>h.Book.Id == id</c>): EF reads the
/// FK column directly, without a join.
/// </para>
/// </summary>
public static class HadithArabicOnlyConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hadith.BookCategory>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Hadith.Book>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasOne(x => x.Category).WithMany(c => c.Books)
                .HasForeignKey(ForeignKeys.Category).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Hadith.MeaningGroup>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasIndex(x => x.Grade);
        });

        modelBuilder.Entity<Hadith.Taraf>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
            // One canonical taraf per meaning group (unique FK).
            e.HasOne(x => x.MeaningGroup).WithOne(g => g.Taraf)
                .HasForeignKey<Hadith.Taraf>(ForeignKeys.MeaningGroup).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Hadith.Hadith>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();

            e.HasOne(x => x.Book).WithMany(b => b.Hadiths)
                .HasForeignKey(ForeignKeys.Book).OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.MeaningGroup).WithMany(g => g.Hadiths)
                .HasForeignKey(ForeignKeys.MeaningGroup).OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(ForeignKeys.Book, nameof(Hadith.Hadith.Number)).IsUnique();
            e.HasIndex(ForeignKeys.MeaningGroup, nameof(Hadith.Hadith.Takhrij));

            // Arabic full-text search over the normalised matn. Queries must use
            // to_tsvector('simple', search_text) for this index to be picked.
            e.HasIndex(x => x.SearchText)
                .HasMethod("GIN")
                .IsTsVectorExpressionIndex("simple")
                .HasDatabaseName("IX_i_h_ao_hadith_search_text_fts");
        });

        modelBuilder.Entity<Hadith.Sanad>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasOne(x => x.Hadith).WithMany(h => h.Sanads)
                .HasForeignKey(ForeignKeys.Hadith).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.MeaningGroup).WithMany(g => g.Sanads)
                .HasForeignKey(ForeignKeys.MeaningGroup).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Hadith.Narrator>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasIndex(x => x.HadithCount);
            e.HasIndex(x => x.SearchName)
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops")
                .HasDatabaseName("IX_i_h_ao_narrator_search_name_trgm");
        });

        modelBuilder.Entity<Hadith.SanadNarrator>(e =>
        {
            e.HasKey(ForeignKeys.Sanad, nameof(Hadith.SanadNarrator.Position));

            e.HasOne(x => x.Sanad).WithMany(s => s.Narrators)
                .HasForeignKey(ForeignKeys.Sanad).OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Narrator).WithMany()
                .HasForeignKey(ForeignKeys.Narrator).OnDelete(DeleteBehavior.Restrict);

            // Repeats the chain's hadith (see the model).
            e.HasOne(x => x.Hadith).WithMany()
                .HasForeignKey(ForeignKeys.Hadith).OnDelete(DeleteBehavior.Cascade);

            // Also serves as the narrator FK index: "hadiths of narrator X" is
            // an index-only scan on (narrator, hadith).
            e.HasIndex(ForeignKeys.Narrator, ForeignKeys.Hadith);
        });

        modelBuilder.Entity<Hadith.Scholar>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Hadith.ScholarStatement>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasOne(x => x.Scholar).WithMany(s => s.Statements)
                .HasForeignKey(ForeignKeys.Scholar).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Narrator).WithMany(n => n.Statements)
                .HasForeignKey(ForeignKeys.Narrator).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Hadith.HadithNarratorMention>(e =>
        {
            e.HasKey(ForeignKeys.Hadith, nameof(Hadith.HadithNarratorMention.Sequence));
            e.HasOne(x => x.Hadith).WithMany(h => h.NarratorMentions)
                .HasForeignKey(ForeignKeys.Hadith).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Narrator).WithMany()
                .HasForeignKey(ForeignKeys.Narrator).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Hadith.HadithQuranQuote>(e =>
        {
            e.HasKey(ForeignKeys.Hadith, nameof(Hadith.HadithQuranQuote.Sequence));
            e.HasOne(x => x.Hadith).WithMany(h => h.QuranQuotes)
                .HasForeignKey(ForeignKeys.Hadith).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.Chapter, x.VerseStart, x.VerseEnd });
        });

        modelBuilder.Entity<Hadith.Topic>(e =>
        {
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasOne(x => x.Parent).WithMany(p => p.Children)
                .HasForeignKey(ForeignKeys.ParentTopic).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.MeaningGroup).WithMany(g => g.Topics)
                .HasForeignKey(ForeignKeys.MeaningGroup).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.TreeId, x.Left });
        });

        modelBuilder.Entity<Hadith.NarratorLink>(e =>
        {
            e.HasKey(ForeignKeys.Teacher, ForeignKeys.Student);
            e.HasOne(x => x.Teacher).WithMany()
                .HasForeignKey(ForeignKeys.Teacher).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Student).WithMany()
                .HasForeignKey(ForeignKeys.Student).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(ForeignKeys.Student);
        });
    }
}
