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
        
        modelBuilder.Entity<ScriptiumBackend.Model.Derived.Islam.Quranic.VerseTranslation>(entity =>
        {
            entity.ToTable("i_q_verse_translation");

            entity.HasOne(vt => vt.Translation)
                .WithMany(t => t.VerseTranslations)
                .HasForeignKey("i_q_translation_id")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(vt => vt.Verse)
                .WithMany(v => v.Translations)
                .HasForeignKey("i_q_verse_id")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
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


        modelBuilder.Entity<Quranic.Chapter>(entity =>
        {
            entity.HasMany(qc => qc.Verses)
                .WithOne(v => v.Chapter)
                .HasForeignKey("i_q_chapter_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Meanings).WithMany().UsingEntity(j =>
            {
                j.ToTable("mm_i_q_chapter__s_meanings");

                j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                j.HasKey("Id");

                j.Property<short>("ChapterId").HasColumnName("i_q_chapter_id");
                j.Property<long>("MeaningId").HasColumnName("s_meaning_id");


                j.HasOne(typeof(Quranic.Chapter)).WithMany()
                    .HasForeignKey("ChapterId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasOne(typeof(Meaning)).WithMany()
                    .HasForeignKey("MeaningId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasIndex("ChapterId", "MeaningId").IsUnique();
            });
        });

        modelBuilder.Entity<Quranic.Verse>(entity =>
        {
            entity.HasOne(qv => qv.Chapter)
                .WithMany(c => c.Verses)
                .HasForeignKey("i_q_chapter_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(v => v.Words)
                .WithOne(w => w.Verse)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Transliterations)
                .WithMany().UsingEntity(j =>
                {
                    j.ToTable("mm_i_q_verse__s_transliteration");

                    j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                    j.HasKey("Id");

                    j.Property<short>("VerseId").HasColumnName("i_q_verse_id");
                    j.Property<long>("TransliterationId").HasColumnName("s_transliteration_id");


                    j.HasOne(typeof(Quranic.Verse)).WithMany()
                        .HasForeignKey("VerseId")
                        .OnDelete(DeleteBehavior.NoAction);

                    j.HasOne(typeof(Transliteration)).WithMany()
                        .HasForeignKey("TransliterationId")
                        .OnDelete(DeleteBehavior.NoAction);

                    j.HasIndex("TransliterationId", "VerseId").IsUnique();
                });
        });

        modelBuilder.Entity<Quranic.Word>(entity =>
        {
            entity.HasMany(e => e.Meanings)
                .WithMany()
                .UsingEntity(j =>
                {
                    j.ToTable("mm_i_q_word__s_meanings");

                    j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                    j.HasKey("Id");

                    j.Property<int>("WordId").HasColumnName("i_q_word_id");
                    j.Property<long>("MeaningId").HasColumnName("s_meaning_id");

                    j.HasOne(typeof(Quranic.Word)).WithMany()
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    j.HasOne(typeof(Meaning)).WithMany()
                        .HasForeignKey("MeaningId")
                        .OnDelete(DeleteBehavior.NoAction);

                    j.HasIndex("WordId", "MeaningId").IsUnique();
                });


            entity.HasMany(e => e.Transliterations)
                .WithMany()
                .UsingEntity(j =>
                {
                    j.ToTable("mm_i_q_word__s_transliterations");

                    j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                    j.HasKey("Id");

                    j.Property<int>("WordId").HasColumnName("i_q_word_id");
                    j.Property<long>("TransliterationId").HasColumnName("s_transliteration_id");

                    j.HasOne(typeof(Quranic.Word)).WithMany()
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    j.HasOne(typeof(Transliteration)).WithMany()
                        .HasForeignKey("TransliterationId")
                        .OnDelete(DeleteBehavior.NoAction);

                    j.HasIndex("WordId", "TransliterationId").IsUnique();
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

        modelBuilder.Entity<Quranic.Root>(entity =>
        {
            entity.HasMany(r => r.Words)
                .WithMany(w => w.Roots)
                .UsingEntity<Dictionary<string, object>>(
                    "mm_i_q_root__i_q_word", j =>
                    {
                        j.HasOne(typeof(Quranic.Word))
                            .WithMany()
                            .HasForeignKey("WordsId")
                            .HasPrincipalKey(nameof(Quranic.Word.Id))
                            .OnDelete(DeleteBehavior.Restrict);

                        j.HasOne(typeof(Quranic.Root))
                            .WithMany()
                            .HasForeignKey("RootsId")
                            .HasPrincipalKey(nameof(Quranic.Root.Id))
                            .OnDelete(DeleteBehavior.Restrict);


                        j.Property<long>("Id")
                            .HasColumnName("id")
                            .ValueGeneratedOnAdd();

                        j.HasKey("Id");


                        j.HasIndex("RootsId", "WordsId").IsUnique();


                        j.Property<int>("RootsId").HasColumnName("i_q_root_id");
                        j.Property<int>("WordsId").HasColumnName("i_q_word_id");
                    });
        });
    }
}