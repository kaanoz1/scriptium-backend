using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Model.Sealed;
using Quranic = ScriptiumBackend.Model.Derived.Islam.Quranic;

namespace ScriptiumBackend.Db.Configurations.Islam;

public static class QuranConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quranic.VerseTranslation>(entity =>
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
