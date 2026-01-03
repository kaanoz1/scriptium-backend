using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Model.Abstract.Islam;
using ScriptiumBackend.Model.Common;
using Common = ScriptiumBackend.Model.Common;
using Shared = ScriptiumBackend.Model.Shared;
using Util = ScriptiumBackend.Model.Util;
using Islam = ScriptiumBackend.Model.Islam;


namespace ScriptiumBackend.Db;

public class ScriptiumDbContext(DbContextOptions<ScriptiumDbContext> options) : DbContext(options)
{
    // Common Model Tables
    public DbSet<Shared.Node> Nodes { get; set; } = null!;

    public DbSet<Scripture> ScripturesC { get; set; } = null!;
    public DbSet<Common.Chapter> ChaptersC { get; set; } = null!;
    public DbSet<Common.Verse> VersesC { get; set; } = null!;

    public DbSet<Common.Word> WordsC { get; set; } = null!;
    public DbSet<Common.Root> RootsC { get; set; } = null!;

    // Shared Model Tables

    public DbSet<Shared.Language> Languages { get; set; } = null!;
    public DbSet<Shared.Meaning> Meanings { get; set; } = null!;
    public DbSet<Shared.Transliteration> Transliterations { get; set; } = null!;


    // Utility Model Tables

    public DbSet<Util.Cache> Caches { get; set; } = null!;
    public DbSet<Util.CacheRecord> CacheRecords { get; set; } = null!;


    // Tables Related to Islamic Sources

    public DbSet<Islam.Quranic.Chapter> ChaptersQ { get; set; } = null!;
    public DbSet<Islam.Quranic.Verse> VersesQ { get; set; } = null!;
    public DbSet<Islam.Quranic.Word> WordsQ { get; set; } = null!;
    public DbSet<Islam.Quranic.Root> RootsQ { get; set; } = null!;


    // Custom Models (Not in the DB)

    public Quran Quran
    {
        get
        {
            var chapters = ChaptersQ
                .Include(qc => qc.ChapterC)
                .OrderBy(qc => qc.Sequence)
                .ToList();

            return new Quran(chapters)
            {
                Name = "القرآن الكريم",
                Code = 'Q'
            };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


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

        modelBuilder.Entity<Shared.Node>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();


            entity.HasOne(n => n.Parent)
                .WithMany(n => n.Children)
                .HasForeignKey("parent_id")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Common.Verse>(entity => { });

        modelBuilder.Entity<Scripture>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasKey(e => e.Id);


            entity.HasMany(e => e.Meanings).WithMany().UsingEntity(j =>
            {
                j.ToTable("c_scripture_meanings");

                j.Property<long>("Id").HasColumnName("id");
                j.HasKey("Id");

                j.Property<short>("ScriptureId").HasColumnName("scripture_id");
                j.Property<long>("MeaningId").HasColumnName("meaning_id");

                j.HasOne(typeof(Common.Scripture)).WithMany().HasForeignKey("ScriptureId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasOne(typeof(Shared.Meaning)).WithMany().HasForeignKey("MeaningId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasIndex("ScriptureId", "MeaningId").IsUnique();
            });
        });

        modelBuilder.Entity<Common.Chapter>(entity => { });

        modelBuilder.Entity<Islam.Quranic.Chapter>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasKey(e => e.Id);


            entity.HasOne(qc => qc.ChapterC)
                .WithOne()
                .HasForeignKey<Islam.Quranic.Chapter>("chapter_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(qc => qc.Verses)
                .WithOne(v => v.Chapter)
                .HasForeignKey("i_q_chapter_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Meanings).WithMany().UsingEntity(j =>
            {
                j.ToTable("i_q_chapter_meanings");

                j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                j.HasKey("Id");

                j.Property<short>("ChapterId").HasColumnName("i_q_chapter_id");
                j.Property<long>("MeaningId").HasColumnName("meaning_id");


                j.HasOne(typeof(Islam.Quranic.Chapter)).WithMany()
                    .HasForeignKey("ChapterId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasOne(typeof(Shared.Meaning)).WithMany()
                    .HasForeignKey("MeaningId")
                    .OnDelete(DeleteBehavior.NoAction);

                j.HasIndex("ChapterId", "MeaningId").IsUnique();
            });
        });

        modelBuilder.Entity<Islam.Quranic.Verse>(entity =>
        {
            entity.HasOne(qv => qv.VerseC)
                .WithOne()
                .HasForeignKey<Islam.Quranic.Verse>("verse_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(qv => qv.Chapter)
                .WithMany(c => c.Verses)
                .HasForeignKey("i_q_chapter_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(v => v.Words)
                .WithOne(w => w.Verse)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Islam.Quranic.Word>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasKey(e => e.Id);

            // Common Word ile İlişki
            entity.HasOne(qw => qw.WordC)
                .WithMany() // Common Word'ün Quranic Word'den haberi yok
                .HasForeignKey("word_c_id")
                .OnDelete(DeleteBehavior.Cascade);

            // MEANINGS İLİŞKİSİ (Join Table)
            entity.HasMany(e => e.Meanings)
                .WithMany() // Meaning entity'si Word'ü bilmez
                .UsingEntity(j =>
                {
                    j.ToTable("i_q_word_meanings");

                    j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                    j.HasKey("Id");

                    j.Property<int>("WordId").HasColumnName("i_q_word_id");
                    j.Property<long>("MeaningId").HasColumnName("meaning_id");

                    j.HasOne(typeof(Islam.Quranic.Word)).WithMany()
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade); // Kelime silinirse ilişki silinsin

                    j.HasOne(typeof(Shared.Meaning)).WithMany()
                        .HasForeignKey("MeaningId")
                        .OnDelete(DeleteBehavior.NoAction); // Anlam tablosundaki veri silinmesin

                    j.HasIndex("WordId", "MeaningId").IsUnique();
                });

            // TRANSLITERATIONS İLİŞKİSİ (Join Table)
            entity.HasMany(e => e.Transliterations)
                .WithMany() // Transliteration entity'si Word'ü bilmez
                .UsingEntity(j =>
                {
                    j.ToTable("i_q_word_transliterations");

                    j.Property<long>("Id").ValueGeneratedOnAdd().HasColumnName("id");
                    j.HasKey("Id");

                    j.Property<int>("WordId").HasColumnName("i_q_word_id");
                    j.Property<long>("TransliterationId").HasColumnName("transliteration_id");

                    j.HasOne(typeof(Islam.Quranic.Word)).WithMany()
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    j.HasOne(typeof(Shared.Transliteration)).WithMany()
                        .HasForeignKey("TransliterationId")
                        .OnDelete(DeleteBehavior.NoAction);

                    j.HasIndex("WordId", "TransliterationId").IsUnique();
                });
        });


        modelBuilder.Entity<Shared.Language>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();


            entity.HasData(Utils.Constants.Default.Language.InitialLanguages);
        });
        modelBuilder.Entity<Shared.Meaning>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.LanguageId).HasColumnName("language_id");
        });

        modelBuilder.Entity<Root>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Islam.Quranic.Root>(entity =>
        {
            entity.HasMany(r => r.Words)
                .WithMany(w => w.Roots)
                .UsingEntity<Dictionary<string, object>>(
                    "i_q_mm_root_word", j =>
                    {
                        j.HasOne(typeof(Islam.Quranic.Word))
                            .WithMany()
                            .HasForeignKey("WordsId")
                            .HasPrincipalKey(nameof(Word.Id))
                            .OnDelete(DeleteBehavior.Restrict);

                        j.HasOne(typeof(Islam.Quranic.Root))
                            .WithMany()
                            .HasForeignKey("RootsId")
                            .HasPrincipalKey(nameof(Root.Id))
                            .OnDelete(DeleteBehavior.Restrict);


                        j.Property<long>("Id")
                            .HasColumnName("id")
                            .ValueGeneratedOnAdd();

                        j.HasKey("Id");


                        j.HasIndex("RootsId", "WordsId").IsUnique();


                        j.Property<int>("RootsId").HasColumnName("root_id");
                        j.Property<int>("WordsId").HasColumnName("word_id");
                    });
        });
    }
}