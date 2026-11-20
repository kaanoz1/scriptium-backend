using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Model.Abstract;
using ScriptiumBackend.Model.Derived.Islam.Quranic;
using ScriptiumBackend.Model.Sealed;
using ScriptiumBackend.Utils.Predefined.Model.Islam;
using Abstract = ScriptiumBackend.Model.Abstract;
using Util = ScriptiumBackend.Model.Util;
using Quranic = ScriptiumBackend.Model.Derived.Islam.Quranic;
using HadithAo = ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Db.Configurations.Islam;


namespace ScriptiumBackend.Db;

public class ScriptiumDbContext(DbContextOptions<ScriptiumDbContext> options) : DbContext(options)
{
    // Abstract Model Tables

    public DbSet<Abstract.Chapter> AChapters { get; set; } = null!;
    public DbSet<Abstract.Verse> AVerses { get; set; } = null!;
    public DbSet<Abstract.Word> AWords { get; set; } = null!;
    public DbSet<Abstract.Root> ARoots { get; set; } = null!;
    public DbSet<Abstract.Translation> ATranslations { get; set; } = null!;
    public DbSet<TranslationUnit> ATranslationUnits { get; set; } = null!;


    // Sealed Models


    public DbSet<Scripture> Scriptures { get; set; } = null!;
    public DbSet<Footnote> Footnotes { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<Meaning> Meanings { get; set; } = null!;
    public DbSet<Transliteration> Transliterations { get; set; } = null!;


    // Utility Model Tables

    public DbSet<Util.Cache> Caches { get; set; } = null!;
    public DbSet<Util.CacheRecord> CacheRecords { get; set; } = null!;
    public DbSet<Util.SearchableItem> SearchableItems { get; set; } = null!;


    // Derived Models

    // Derived Models Related To Islam

    // Derived Models Related To Quran

    public DbSet<Quranic.Chapter> QChapters { get; set; } = null!;
    public DbSet<Quranic.Root> QRoots { get; set; } = null!;

    public DbSet<Quranic.Translation> QTranslations { get; set; } = null!;
    public DbSet<Quranic.Verse> QVerses { get; set; } = null!;
    public DbSet<Quranic.Word> QWords { get; set; } = null!;

    public DbSet<VerseTranslation> QVerseTranslations { get; set; } = null!;


    // Derived Models Related To Hadith (Arabic-only corpus, "ao")

    public DbSet<HadithAo.BookCategory> HAoBookCategories { get; set; } = null!;
    public DbSet<HadithAo.Book> HAoBooks { get; set; } = null!;
    public DbSet<HadithAo.Hadith> HAoHadiths { get; set; } = null!;
    public DbSet<HadithAo.MeaningGroup> HAoMeaningGroups { get; set; } = null!;
    public DbSet<HadithAo.Taraf> HAoTarafs { get; set; } = null!;
    public DbSet<HadithAo.Sanad> HAoSanads { get; set; } = null!;
    public DbSet<HadithAo.SanadNarrator> HAoSanadNarrators { get; set; } = null!;
    public DbSet<HadithAo.Narrator> HAoNarrators { get; set; } = null!;
    public DbSet<HadithAo.NarratorLink> HAoNarratorLinks { get; set; } = null!;
    public DbSet<HadithAo.Scholar> HAoScholars { get; set; } = null!;
    public DbSet<HadithAo.ScholarStatement> HAoScholarStatements { get; set; } = null!;
    public DbSet<HadithAo.HadithNarratorMention> HAoHadithNarratorMentions { get; set; } = null!;
    public DbSet<HadithAo.HadithQuranQuote> HAoHadithQuranQuotes { get; set; } = null!;
    public DbSet<HadithAo.Topic> HAoTopics { get; set; } = null!;


    // Custom Models (Not in the DB)

    public Quran Quran
    {
        get
        {
            var chapters = QChapters
                .OrderBy(qc => qc.Sequence)
                .ToList();

            var translations = QTranslations
                .Include(t => t.Authors).ThenInclude(a => a.Language).ToList();

            return new Quran(chapters, translations);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.HasPostgresExtension("pg_trgm");

        // Religion-specific schema: Qur'an and hadith tables.
        IslamConfiguration.Configure(modelBuilder);
        
        modelBuilder.Entity<Util.SearchableItem>(entity =>
        {
        
            entity.ToTable("u_searchable_item");
        
            entity.Property(e => e.Embedding)
                .HasColumnType("vector(768)");
        });
        
        modelBuilder.Entity<Util.Cache>(entity =>
        {
            entity.HasIndex(e => e.Url)
                .IsUnique();

            entity.ToTable("u_cache");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Url).IsUnique();
        });

        modelBuilder.Entity<TranslationUnit>(entity =>
        {
            entity.HasIndex(e => e.Text)
                .HasMethod("gin") 
                .HasOperators("gin_trgm_ops"); 
        });
        

        modelBuilder.Entity<Util.CacheRecord>(entity =>
        {
            entity.ToTable("u_cache_record");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property<long>("cache_id");

            entity.HasOne(cr => cr.Cache)
                .WithMany(c => c.Records)
                .HasForeignKey("cache_id")
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(e => e.Language)
                .WithMany()
                .HasForeignKey("s_language_id")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.NameTranslations)
                .WithMany()
                .UsingEntity(j =>
                {
                    j.ToTable("mm_s_authorName__a_translations");

                    j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                    j.HasKey("Id");

                    j.Property<long>("AuthorId").HasColumnName("s_author_id");
                    j.Property<long>("MeaningId").HasColumnName("s_meaning_id");

                    j.HasOne(typeof(Author))
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    j.HasOne(typeof(Meaning))
                        .WithMany()
                        .HasForeignKey("MeaningId")
                        .OnDelete(DeleteBehavior.NoAction);

                    j.HasIndex("AuthorId", "MeaningId").IsUnique();
                });
        });


        modelBuilder.Entity<Scripture>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasKey(e => e.Id);


            entity.HasMany(e => e.Meanings).WithMany().UsingEntity(j =>
            {
                j.ToTable("mm_s_scripture__s_meanings");

                j.Property<long>("Id").HasColumnName("id");
                j.HasKey("Id");

                j.Property<short>("ScriptureId").HasColumnName("s_scripture_id");
                j.Property<long>("MeaningId").HasColumnName("s_meaning_id");

                j.HasOne(typeof(Scripture)).WithMany().HasForeignKey("ScriptureId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasOne(typeof(Meaning)).WithMany().HasForeignKey("MeaningId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasIndex("ScriptureId", "MeaningId").IsUnique();
            });
        });


        modelBuilder.Entity<Abstract.Translation>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description).IsRequired(false);

            entity.HasMany(t => t.Authors)
                .WithMany(a => a.Translations)
                .UsingEntity(j =>
                {
                    j.ToTable("mm_s_translation__s_author");

                    j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                    j.HasKey("Id");

                    j.Property<short>("TranslationId").HasColumnName("s_translation_id");
                    j.Property<long>("AuthorId").HasColumnName("s_author_id");

                    j.HasOne(typeof(Abstract.Translation)).WithMany()
                        .HasForeignKey("TranslationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    j.HasOne(typeof(Author)).WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    j.HasIndex("TranslationId", "AuthorId").IsUnique();
                });
        });



        modelBuilder.Entity<Language>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();


            entity.HasData(Utils.Constants.Default.Language.InitialLanguages);
        });
        modelBuilder.Entity<Meaning>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Content).HasColumnName("content");
        });

        modelBuilder.Entity<Abstract.Root>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasIndex(e => e.Text).IsUnique();
        });

}
}