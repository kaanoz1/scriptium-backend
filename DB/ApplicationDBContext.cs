using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Models;


namespace ScriptiumBackend.DB
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) : DbContext(options) //IdentityDbContext<User, Role, Guid>(options)
    {

        //Static 
        public DbSet<Language> Languages { get; set; }

        public DbSet<Scripture> Scriptures { get; set; }

        public DbSet<ScriptureMeaning> ScriptureMeanings { get; set; }

        public DbSet<Section> Sections { get; set; }


        public DbSet<SectionMeaning> SectionMeanings { get; set; }

        public DbSet<Chapter> Chapters { get; set; }


        public DbSet<ChapterMeaning> ChapterMeanings { get; set; }

        public DbSet<Root> Roots { get; set; }

        public DbSet<Verse> Verses { get; set; }


        public DbSet<Word> Words { get; set; }


        public DbSet<WordMeaning> WordMeanings { get; set; }


        public DbSet<Transliteration> Transliterations { get; set; }


        public DbSet<Translator> Translators { get; set; }

        public DbSet<Translation> Translations { get; set; }


        public DbSet<TranslatorTranslation> TranslatorTranslations { get; set; }

        public DbSet<TranslationText> TranslationTexts { get; set; }


        public DbSet<FootNoteText> FootNoteTexts { get; set; }


        public DbSet<FootNote> FootNotes { get; set; }

        public DbSet<Cache> Caches { get; set; }

        public DbSet<CacheRecord> CacheRecords { get; set; }

        public DbSet<RequestLog> RequestLogs { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookMeaning> BookMeanings { get; set; }

        public DbSet<BookNode> BookNodes { get; set; }

        public DbSet<BookNodeMeaning> BookNodeMeanings { get; set; }

        public DbSet<BookText> BookText { get; set; }

        public DbSet<BookTranslation> BookTranslations { get; set; }

        public DbSet<BookTranslationText> BookTranslationTexts { get; set; }
        
        public DbSet<BookTranslationTextFootNote> BookTranslationTextFootNotes { get; set; }

        

        // Dynamic, user data.
/*
        public DbSet<User> User { get; set; }

        public DbSet<Session> Session { get; set; }

        public DbSet<Collection> Collection { get; set; }

        public DbSet<CollectionVerse> CollectionVerse { get; set; }

        public DbSet<Note> Note { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<CommentVerse> CommentVerse { get; set; }

        public DbSet<CommentNote> CommentNote { get; set; }

        public DbSet<Follow> Follow { get; set; }

        public DbSet<Block> Block { get; set; }

        public DbSet<FreezeR> FreezeR { get; set; }

        public DbSet<Like> Like { get; set; }

        public DbSet<LikeComment> LikeComment { get; set; }

        public DbSet<LikeNote> LikeNote { get; set; }

        public DbSet<Notification> Notification { get; set; }

        public DbSet<Suggestion> Suggestion { get; set; }

*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Language>(language =>
            {
                language.ToTable("language");

                language.HasKey(e => e.Id);

                language.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired();

                language.HasIndex(e => e.LangCode)
                    .IsUnique();

                language.Property(l => l.LangCode).HasColumnName("lang_code").HasColumnType(Utility.DBTypeVARCHAR2)
                    .IsRequired();

                language.Property(l => l.LangEnglish).HasColumnName("lang_english")
                    .HasColumnType(Utility.DBTypeVARCHAR16).IsRequired();

                language.Property(l => l.LangOwn).HasColumnName("lang_own").HasColumnType(Utility.DBTypeVARCHAR16)
                    .IsRequired();


                language.HasMany(l => l.ScriptureMeanings).WithOne(sm => sm.Language).OnDelete(DeleteBehavior.NoAction);
                language.HasMany(l => l.SectionMeanings).WithOne(sm => sm.Language).OnDelete(DeleteBehavior.NoAction);
                language.HasMany(l => l.ChapterMeanings).WithOne(cm => cm.Language).OnDelete(DeleteBehavior.NoAction);
                language.HasMany(l => l.WordMeanings).WithOne(wm => wm.Language).OnDelete(DeleteBehavior.NoAction);
                language.HasMany(l => l.Transliterations).WithOne(t => t.Language).OnDelete(DeleteBehavior.NoAction);
                language.HasMany(l => l.Translators).WithOne(t => t.Language).OnDelete(DeleteBehavior.NoAction);
                language.HasMany(l => l.Translations).WithOne(t => t.Language).OnDelete(DeleteBehavior.NoAction);

                language.HasData(
                    new Language { Id = 1, LangCode = "en", LangOwn = "English", LangEnglish = "English" },
                    new Language { Id = 2, LangCode = "de", LangOwn = "Deutsch", LangEnglish = "German" }
                );
            });

            modelBuilder.Entity<Scripture>(scripture =>

            {
                scripture.ToTable("scripture");

                scripture.HasKey(s => s.Id);

                scripture.Property(s => s.Id).HasColumnName("id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired().ValueGeneratedOnAdd();

                scripture.Property(s => s.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeNVARCHAR255).IsRequired();

                scripture.Property(s => s.Code)
                    .HasColumnName("code")
                    .HasColumnType(Utility.DBTypeCHAR1).HasMaxLength(1).IsRequired();

                scripture.Property(s => s.Number)
                    .HasColumnName("number")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired();

                scripture.HasIndex(e => e.Name)
                    .IsUnique();

                scripture.HasIndex(e => e.Code)
                    .IsUnique();

                scripture.HasMany(s => s.Meanings).WithOne(m => m.Scripture).OnDelete(DeleteBehavior.NoAction);
                scripture.HasMany(s => s.Sections).WithOne(s => s.Scripture).OnDelete(DeleteBehavior.Restrict);
                scripture.HasMany(s => s.Roots).WithOne(r => r.Scripture).OnDelete(DeleteBehavior.Restrict);
                scripture.HasMany(s => s.Translations).WithOne(r => r.Scripture).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ScriptureMeaning>(scriptureMeaning =>
            {
                scriptureMeaning.ToTable("scripture_meaning");

                scriptureMeaning.HasKey(s => s.Id);

                scriptureMeaning.Property(sm => sm.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                scriptureMeaning.Property(sm => sm.Meaning).HasColumnName("meaning")
                    .HasColumnType(Utility.DBTypeVARCHAR50).IsRequired();

                scriptureMeaning.Property(sm => sm.ScriptureId).HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired();

                scriptureMeaning.HasOne(c => c.Scripture)
                    .WithMany(p => p.Meanings)
                    .HasForeignKey(c => c.ScriptureId)
                    .OnDelete(DeleteBehavior.Cascade);

                scriptureMeaning.Property(sm => sm.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired();

                scriptureMeaning.HasOne(c => c.Language)
                    .WithMany(p => p.ScriptureMeanings)
                    .HasForeignKey(c => c.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                scriptureMeaning.HasIndex(e => new { e.LanguageId, e.ScriptureId })
                    .IsUnique();
            });

            modelBuilder.Entity<Section>(section =>
            {
                section.ToTable("section");

                section.HasKey(s => s.Id);

                section.Property(s => s.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                section.Property(s => s.Name).HasColumnName("name").HasColumnType(Utility.DBTypeNVARCHAR255)
                    .IsRequired();

                section.Property(s => s.ScriptureId).HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired();

                section.HasOne(s => s.Scripture)
                    .WithMany(scr => scr.Sections)
                    .HasForeignKey(s => s.ScriptureId)
                    .OnDelete(DeleteBehavior.Restrict);

                section.HasIndex(e => e.Name)
                    .IsUnique();

                section.HasIndex(e => new { e.ScriptureId, e.Number })
                    .IsUnique();

                section.HasMany(s => s.Chapters).WithOne(c => c.Section).OnDelete(DeleteBehavior.Restrict);

                section.HasMany(s => s.Meanings).WithOne(sm => sm.Section).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<SectionMeaning>(sectionMeaning =>
            {
                sectionMeaning.ToTable("section_meaning");

                sectionMeaning.HasKey(s => s.Id);

                sectionMeaning.Property(s => s.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                sectionMeaning.Property(s => s.Meaning).HasColumnName("meaning").HasColumnType(Utility.DBTypeVARCHAR100)
                    .IsRequired();

                sectionMeaning.Property(s => s.SectionId).HasColumnName("section_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired();

                sectionMeaning.HasOne(c => c.Section)
                    .WithMany(e => e.Meanings)
                    .HasForeignKey(e => e.SectionId)
                    .OnDelete(DeleteBehavior.Restrict);

                sectionMeaning.Property(s => s.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired();

                sectionMeaning.HasOne(c => c.Language)
                    .WithMany(e => e.SectionMeanings)
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                sectionMeaning.HasIndex(e => new { e.LanguageId, e.SectionId })
                    .IsUnique();
            });

            modelBuilder.Entity<Chapter>(chapter =>
            {
                chapter.ToTable("chapter");

                chapter.HasKey(c => c.Id);

                chapter.Property(c => c.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                chapter.Property(c => c.Name).HasColumnName("name").HasColumnType(Utility.DBTypeNVARCHAR255)
                    .IsRequired();

                chapter.Property(c => c.Number).HasColumnName("number").HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired();

                chapter.Property(c => c.SectionId).HasColumnName("section_id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                chapter.HasOne(c => c.Section)
                    .WithMany(s => s.Chapters)
                    .HasForeignKey(c => c.SectionId)
                    .OnDelete(DeleteBehavior.Restrict);

                chapter.HasMany(c => c.Verses).WithOne(v => v.Chapter).OnDelete(DeleteBehavior.Restrict);

                chapter.HasMany(c => c.Meanings).WithOne(m => m.Chapter).OnDelete(DeleteBehavior.NoAction);

                chapter.HasIndex(c => c.Name)
                    .IsUnique();

                chapter.HasIndex(c => new { c.SectionId, c.Number })
                    .IsUnique();
            });

            modelBuilder.Entity<ChapterMeaning>(chapterMeaning =>
            {
                chapterMeaning.ToTable("chapter_meaning");

                chapterMeaning.HasKey(cm => cm.Id);

                chapterMeaning.Property(cm => cm.Id).HasColumnName("id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                chapterMeaning.Property(s => s.Meaning).HasColumnName("meaning").HasColumnType(Utility.DBTypeVARCHAR100)
                    .IsRequired();

                chapterMeaning.Property(s => s.ChapterId).HasColumnName("chapter_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired();

                chapterMeaning.HasOne(c => c.Chapter)
                    .WithMany(p => p.Meanings)
                    .HasForeignKey(c => c.ChapterId)
                    .OnDelete(DeleteBehavior.Restrict);

                chapterMeaning.Property(s => s.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired();

                chapterMeaning.HasOne(cm => cm.Language)
                    .WithMany(l => l.ChapterMeanings)
                    .HasForeignKey(cm => cm.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                chapterMeaning.HasIndex(cm => new { cm.ChapterId, cm.LanguageId })
                    .IsUnique();
            });

            modelBuilder.Entity<Root>(root =>
            {
                root.ToTable("root");

                root.HasKey(r => r.Id);

                root.Property(r => r.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                root.Property(r => r.Latin)
                    .HasColumnName("latin")
                    .HasColumnType(Utility.DBTypeNVARCHAR30)
                    .IsRequired()
                    .UseCollation("SQL_Latin1_General_CP1_CS_AS"); // Case-sensitive

                root.Property(r => r.Own)
                    .HasColumnName("own")
                    .HasColumnType(Utility.DBTypeNVARCHAR30)
                    .IsRequired();

                root.Property(r => r.ScriptureId)
                    .HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                root.HasOne(r => r.Scripture)
                    .WithMany(sc => sc.Roots)
                    .HasForeignKey(r => r.ScriptureId)
                    .OnDelete(DeleteBehavior.Restrict);

                root.HasMany(r => r.Words).WithMany(w => w.Roots);

                root.HasIndex(r => new { r.Latin, r.ScriptureId })
                    .IsUnique();
            });


            modelBuilder.Entity<Verse>(verse =>
            {
                verse.ToTable("verse");

                verse.HasKey(v => v.Id);

                verse.Property(v => v.Id).HasColumnName("id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                verse.Property(v => v.Number).HasColumnName("number").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                verse.Property(v => v.Text).HasColumnName("text").HasColumnType(Utility.DBTypeNVARCHAR1000)
                    .IsRequired();

                verse.Property(v => v.TextWithoutVowel).HasColumnName("text_without_vowel")
                    .HasColumnType(Utility.DBTypeNVARCHAR1000).IsRequired(false);

                verse.Property(v => v.TextSimplified).HasColumnName("text_simplified")
                    .HasColumnType(Utility.DBTypeNVARCHAR1000).IsRequired(false);

                verse.Property(v => v.ChapterId).HasColumnName("chapter_id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                verse.HasOne(v => v.Chapter)
                    .WithMany(c => c.Verses)
                    .HasForeignKey(v => v.ChapterId)
                    .OnDelete(DeleteBehavior.Restrict);

                verse.HasMany(v => v.Words).WithOne(w => w.Verse).OnDelete(DeleteBehavior.Restrict);
                verse.HasMany(v => v.Transliterations).WithOne(t => t.Verse).OnDelete(DeleteBehavior.NoAction);
                verse.HasMany(v => v.TranslationTexts).WithOne(tt => tt.Verse).OnDelete(DeleteBehavior.NoAction);
                // Unused properties, but kept for future use or reference.
                // verse.HasMany(v => v.CollectionVerses).WithOne(w => w.Verse).OnDelete(DeleteBehavior.NoAction);
                // verse.HasMany(v => v.Notes).WithOne(w => w.Verse).OnDelete(DeleteBehavior.NoAction);
                // verse.HasMany(v => v.Comments).WithOne(w => w.Verse).OnDelete(DeleteBehavior.NoAction);

                verse.HasIndex(v => new { v.ChapterId, v.Number })
                    .IsUnique();
            });

            modelBuilder.Entity<Word>(word =>
            {
                word.ToTable("word");

                word.HasKey(w => w.Id);

                word.Property(v => v.Id).HasColumnName("id").HasColumnType(Utility.DBType64bitInteger).IsRequired();

                word.Property(v => v.SequenceNumber).HasColumnName("sequence_number")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired();

                word.Property(v => v.Text).HasColumnName("text").HasColumnType(Utility.DBTypeNVARCHAR50)
                    .IsRequired();

                word.Property(v => v.TextWithoutVowel).HasColumnName("text_without_vowel")
                    .HasColumnType(Utility.DBTypeNVARCHAR50);

                word.Property(v => v.TextSimplified).HasColumnName("text_simplified")
                    .HasColumnType(Utility.DBTypeNVARCHAR50);

                word.Property(v => v.VerseId).HasColumnName("verse_id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                word.HasOne(w => w.Verse)
                    .WithMany(v => v.Words)
                    .HasForeignKey(w => w.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);


                word.HasMany(w => w.Roots)
                    .WithMany(r => r.Words);


                word.HasMany(w => w.WordMeanings)
                    .WithOne(r => r.Word)
                    .OnDelete(DeleteBehavior.NoAction);

                word.HasIndex(w => new { w.SequenceNumber, w.VerseId })
                    .IsUnique();
            });

            modelBuilder.Entity<WordMeaning>(wordMeaning =>
            {
                wordMeaning.ToTable("word_meaning");

                wordMeaning.HasKey(wm => wm.Id);

                wordMeaning.Property(wm => wm.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                wordMeaning.Property(wm => wm.Meaning)
                    .HasColumnName("meaning")
                    .HasColumnType(Utility.DBTypeVARCHAR100)
                    .IsRequired();

                wordMeaning.Property(wm => wm.WordId).HasColumnName("word_id").HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                wordMeaning.Property(wm => wm.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired();

                wordMeaning.HasOne(wm => wm.Word)
                    .WithMany(w => w.WordMeanings)
                    .HasForeignKey(wm => wm.WordId)
                    .OnDelete(DeleteBehavior.Restrict);

                wordMeaning.HasOne(wm => wm.Language)
                    .WithMany(l => l.WordMeanings)
                    .HasForeignKey(wm => wm.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transliteration>(transliteration =>
            {
                transliteration.ToTable("transliteration");

                transliteration.HasKey(t => t.Id);

                transliteration.Property(t => t.Id)
                    .HasColumnName("id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                transliteration.Property(t => t.Text).HasColumnName("text")
                    .HasColumnType(Utility.DBTypeVARCHARMAX)
                    .IsRequired();

                transliteration.Property(t => t.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired();

                transliteration.HasOne(t => t.Language)
                    .WithMany(l => l.Transliterations)
                    .HasForeignKey(t => t.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                transliteration.HasOne(t => t.Verse)
                    .WithMany(v => v.Transliterations)
                    .HasForeignKey(t => t.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);

                transliteration.HasIndex(e => new { e.VerseId, e.LanguageId }).IsUnique();
            });

            modelBuilder.Entity<Translator>(translator =>
            {
                translator.ToTable("translator");

                translator.HasKey(e => e.Id);

                translator.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                translator.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeVARCHAR250)
                    .IsRequired();

                translator.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType(Utility.DBTypeVARCHARMAX);

                translator.Property(e => e.Url)
                    .HasColumnType(Utility.DBTypeVARCHARMAX);

                translator.Property(e => e.LanguageId)
                    .HasColumnName("language_id");

                translator.HasOne(e => e.Language)
                    .WithMany(l => l.Translators)
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                translator.HasMany(e => e.TranslatorTranslations)
                    .WithOne(tt => tt.Translator)
                    .HasForeignKey(tt => tt.TranslatorId)
                    .OnDelete(DeleteBehavior.NoAction);

                translator.HasMany(e => e.TranslationBookTranslators)
                    .WithOne(tt => tt.Translator)
                    .HasForeignKey(tt => tt.TranslatorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BookTranslation>(bookTranslation =>
            {
                bookTranslation.HasMany(e => e.TranslationBookTranslators)
                    .WithOne(tt => tt.Translation)
                    .HasForeignKey(tt => tt.TranslatorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Translation>(translation =>
            {
                translation.ToTable("translation");

                translation.HasKey(t => t.Id);

                translation.Property(t => t.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();


                translation.Property(t => t.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeVARCHAR250)
                    .IsRequired();

                translation.Property(t => t.ProductionTime)
                    .HasColumnName("production_year")
                    .HasColumnType(Utility.DBTypeDateTime);

                translation.Property(t => t.AddedAt)
                    .HasColumnName("added_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();

                translation.Property(t => t.EagerFrom)
                    .HasColumnName("eager_from")
                    .HasColumnType(Utility.DBTypeDateTime);

                translation.Property(t => t.LanguageId)
                    .HasColumnName("language_id").HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired()
                    .HasDefaultValue(1);

                translation.Property(t => t.ScriptureId)
                    .HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();


                translation.HasOne(t => t.Language)
                    .WithMany(l => l.Translations)
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                translation.HasMany(t => t.TranslatorTranslations)
                    .WithOne(tt => tt.Translation)
                    .HasForeignKey(t => t.TranslationId)
                    .OnDelete(DeleteBehavior.NoAction);


                translation.HasMany(e => e.TranslationTexts)
                    .WithOne(ttx => ttx.Translation)
                    .HasForeignKey(ttx => ttx.TranslationId)
                    .OnDelete(DeleteBehavior.NoAction);

                translation.HasOne(t => t.Scripture)
                    .WithMany(s => s.Translations)
                    .HasForeignKey(t => t.ScriptureId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TranslatorTranslation>(translatorTranslation =>
            {
                translatorTranslation.ToTable("translator_translation");

                translatorTranslation.HasKey(tt => new { tt.TranslatorId, tt.TranslationId });

                translatorTranslation.Property(tt => tt.TranslatorId)
                    .HasColumnName("translator_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                translatorTranslation.Property(tt => tt.TranslationId)
                    .HasColumnName("translation_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                translatorTranslation.Property(tt => tt.AssignedOn)
                    .HasColumnName("assigned_on")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();


                translatorTranslation.HasOne(tt => tt.Translator)
                    .WithMany(tr => tr.TranslatorTranslations)
                    .HasForeignKey(tt => tt.TranslatorId)
                    .OnDelete(DeleteBehavior.Cascade);

                translatorTranslation.HasOne(tt => tt.Translation)
                    .WithMany(tr => tr.TranslatorTranslations)
                    .HasForeignKey(tt => tt.TranslationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TranslationText>(translationText =>
            {
                translationText.ToTable("translation_text");

                translationText.HasKey(tt => tt.Id);

                translationText.Property(tt => tt.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired()
                    ;

                translationText.Property(tt => tt.Text).HasColumnName("text").HasColumnType(Utility.DBTypeVARCHARMAX)
                    .IsRequired();

                translationText.Property(tt => tt.TranslationId)
                    .HasColumnName("translation_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                translationText.Property(tt => tt.VerseId)
                    .HasColumnName("verse_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();


                translationText.HasOne(tt => tt.Translation)
                    .WithMany(tr => tr.TranslationTexts)
                    .HasForeignKey(tt => tt.TranslationId)
                    .OnDelete(DeleteBehavior.Cascade);

                translationText.HasOne(tt => tt.Verse)
                    .WithMany(v => v.TranslationTexts)
                    .HasForeignKey(tt => tt.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);

                translationText.HasMany(tt => tt.FootNotes)
                    .WithOne(fn => fn.TranslationText)
                    .HasForeignKey(fn => fn.TranslationTextId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Unused properties, but kept for future use or reference.
                // translationText.HasMany(tt => tt.Suggestions)
                //     .WithOne(s => s.TranslationText)
                //     .OnDelete(DeleteBehavior.NoAction);

                translationText.HasIndex(tt => new { tt.VerseId, tt.TranslationId });
            });

            modelBuilder.Entity<FootNoteText>(footNoteText =>
            {
                footNoteText.ToTable("footnote_text");

                footNoteText.HasKey(ft => ft.Id);

                footNoteText.Property(ft => ft.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired()
                    ;

                footNoteText.Property(ft => ft.Text).HasColumnName("text").HasColumnType(Utility.DBTypeNVARCHARMAX)
                    .IsRequired();

                footNoteText.HasMany(footnoteText => footnoteText.FootNotes).WithOne(ft => ft.FootNoteText)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<FootNote>(footNote =>
            {
                footNote.ToTable("footnote");

                footNote.HasKey(fn => fn.Id);

                footNote.Property(fn => fn.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired()
                    ;

                footNote.Property(fn => fn.Index)
                    .HasColumnName("index")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired();

                footNote.HasOne(fn => fn.TranslationText)
                    .WithMany(tt => tt.FootNotes)
                    .HasForeignKey(fn => fn.TranslationTextId)
                    .OnDelete(DeleteBehavior.Cascade);

                footNote.HasOne(fn => fn.FootNoteText)
                    .WithMany(fnt => fnt.FootNotes)
                    .HasForeignKey(fn => fn.FootNoteTextId)
                    .OnDelete(DeleteBehavior.Cascade);

                footNote.HasIndex(fn => new { fn.Index, fn.TranslationTextId });
            });

            modelBuilder.Entity<Cache>(cache =>
            {
                cache.ToTable("cache");

                cache.HasKey(e => e.Id);

                cache.Property(e => e.Id).HasColumnName("id").HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                cache.Property(e => e.Key).HasColumnName("key").HasColumnType(Utility.DBTypeVARCHAR126)
                    .IsRequired();

                cache.Property(e => e.Data)
                    .HasColumnType("NVARCHAR(MAX)").IsRequired();
            });

            modelBuilder.Entity<CacheRecord>(cacheRecord =>
            {
                cacheRecord.ToTable("cache_r");

                cacheRecord.HasKey(e => e.Id);

                cacheRecord.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                cacheRecord.Property(e => e.CacheId)
                    .HasColumnName("cache_id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                cacheRecord.Property(e => e.FetchedAt)
                    .HasColumnName("fetched_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();

                cacheRecord.HasOne(e => e.Cache)
                    .WithMany(c => c.Records)
                    .HasForeignKey(e => e.CacheId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
/*  User properties. Disabled since Scriptium does not process user data for a while.
            modelBuilder.Entity<Role>(Role => { Role.ToTable("role"); });

            modelBuilder.Entity<User>(User =>
            {
                User.HasQueryFilter(u => u.DeletedAt == null);

                User.ToTable("user");

                User.HasKey(e => e.Id);

                User.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBTypeUUID)
                    .HasDefaultValueSql(Utility.DBDefaultUUIDFunction)
                    .IsRequired();

                User.Property(e => e.UserName)
                    .HasColumnName("username")
                    .HasColumnType(Utility.DBTypeVARCHAR16)
                    .HasMaxLength(16)
                    .IsRequired()
                    .HasColumnName("username");

                User.Property(e => e.Name)
                    .HasMaxLength(16).HasColumnType(Utility.DBTypeVARCHAR16)
                    .IsRequired();

                User.Property(e => e.Image).HasColumnName("image").HasColumnType(Utility.DBTypeVARBINARYMAX)
                    .IsRequired(false);

                User.Property(e => e.Surname).HasColumnName("surname").HasColumnType(Utility.DBTypeVARCHAR16)
                    .HasMaxLength(16)
                    .IsRequired(false);

                User.Property(e => e.Gender).HasColumnName("gender").HasColumnType(Utility.DBTypeCHAR1)
                    .HasMaxLength(1)
                    .IsRequired(false);

                User.Property(e => e.Biography)
                    .HasMaxLength(200)
                    .IsRequired(false);

                User.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsRequired();

                User.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();

                User.Property(e => e.LastActive)
                    .HasColumnName("last_active")
                    .HasColumnType(Utility.DBTypeDateTime);

                User.Property(e => e.IsFrozen)
                    .HasColumnName("is_frozen")
                    .HasColumnType(Utility.DBTypeDateTime);

                User.Property(e => e.IsPrivate)
                    .HasColumnName("is_private")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired(false);

                User.Property(e => e.PreferredLanguageId)
                    .HasColumnName("preferred_languageId")
                    .HasColumnType(Utility.DBType8bitInteger)
                    .HasDefaultValue(1)
                    .IsRequired();

                User.HasIndex(e => e.NormalizedEmail)
                    .IsUnique();

                User.HasIndex(e => e.NormalizedUserName)
                    .IsUnique();

                User.HasIndex(e => e.Email)
                    .IsUnique();

                User.HasIndex(e => e.UserName)
                    .IsUnique();

                User.HasOne(e => e.PreferredLanguage)
                    .WithMany(l => l.PreferredUsers)
                    .HasForeignKey(e => e.PreferredLanguageId)
                    .OnDelete(DeleteBehavior.Restrict); // TODO: ON DELETE CASCADE
            });


            modelBuilder.Entity<Collection>(Collection =>
            {
                Collection.ToTable("collection");

                Collection.HasKey(e => e.Id);

                Collection.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                Collection.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeVARCHAR24)
                    .HasMaxLength(24)
                    .IsRequired()
                    .HasDefaultValue(string.Empty);

                Collection.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType(Utility.DBTypeVARCHAR72)
                    .HasMaxLength(72)
                    .IsRequired(false);

                Collection.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType(Utility.DBTypeUUID)
                    .IsRequired();

                Collection.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();

                Collection.HasIndex(e => new { e.UserId, e.Name })
                    .IsUnique();

                Collection.HasOne(e => e.User)
                    .WithMany(u => u.Collections)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                Collection.HasMany(c => c.Verses).WithOne(cv => cv.Collection).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CollectionVerse>(CollectionVerse =>
            {
                CollectionVerse.ToTable("collection_verse");

                CollectionVerse.HasKey(e => e.Id);

                CollectionVerse.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger).IsRequired();

                CollectionVerse.Property(e => e.CollectionId)
                    .HasColumnName("collection_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                CollectionVerse.Property(e => e.VerseId)
                    .HasColumnName("verse_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                CollectionVerse.Property(e => e.SavedAt)
                    .HasColumnName("saved_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();

                CollectionVerse.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasColumnType(Utility.DBTypeVARCHAR250)
                    .HasMaxLength(250)
                    .IsRequired(false);

                CollectionVerse.HasIndex(e => new { e.CollectionId, e.VerseId })
                    .IsUnique();

                CollectionVerse.HasOne(e => e.Collection)
                    .WithMany(c => c.Verses)
                    .HasForeignKey(e => e.CollectionId)
                    .OnDelete(DeleteBehavior.Cascade);

                CollectionVerse.HasOne(e => e.Verse)
                    .WithMany(v => v.CollectionVerses)
                    .HasForeignKey(e => e.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Note>(Note =>
            {
                Note.ToTable("note");

                Note.HasKey(e => e.Id);

                Note.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger).IsRequired();

                Note.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType(Utility.DBTypeUUID)
                    .IsRequired();

                Note.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType(Utility.DBTypeVARCHARMAX)
                    .IsRequired();

                Note.Property(e => e.VerseId)
                    .HasColumnName("verse_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                Note.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();

                Note.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType(Utility.DBTypeDateTime);

                Note.HasOne(e => e.User)
                    .WithMany(u => u.Notes)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                Note.HasOne(e => e.Verse)
                    .WithMany(v => v.Notes)
                    .HasForeignKey(e => e.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);

                Note.HasMany(n => n.Comments).WithOne(c => c.Note).OnDelete(DeleteBehavior.NoAction);
                Note.HasMany(n => n.Likes).WithOne(l => l.Note).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Comment>(Comment =>
            {
                Comment.ToTable("comment");

                Comment.HasKey(e => e.Id);

                Comment.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                Comment.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType(Utility.DBTypeUUID)
                    .IsRequired();

                Comment.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType(Utility.DBTypeVARCHAR500)
                    .IsRequired();

                Comment.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired();

                Comment.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType(Utility.DBTypeDateTime);

                Comment.Property(e => e.ParentCommentId)
                    .HasColumnName("parent_comment_id")
                    .HasColumnType(Utility.DBType64bitInteger);

                Comment.HasOne(e => e.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                Comment.HasOne(e => e.ParentComment)
                    .WithMany(c => c.Replies)
                    .HasForeignKey(e => e.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict);

                Comment.HasOne(c => c.CommentVerse)
                    .WithOne(cv => cv.Comment)
                    .HasForeignKey<CommentVerse>(cv => cv.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                Comment.HasOne(c => c.CommentNote)
                    .WithOne(cn => cn.Comment)
                    .HasForeignKey<CommentNote>(cn => cn.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CommentVerse>(CommentVerse =>
            {
                CommentVerse.ToTable("comment_verse");

                CommentVerse.HasKey(e => e.CommentId);

                CommentVerse.Property(e => e.CommentId)
                    .HasColumnName("comment_id")
                    .HasColumnType(Utility.DBType64bitInteger);

                CommentVerse.Property(e => e.VerseId)
                    .HasColumnName("verse_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired();

                CommentVerse.HasOne(e => e.Verse)
                    .WithMany(v => v.Comments)
                    .HasForeignKey(e => e.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);

                CommentVerse.HasOne(c => c.Comment)
                    .WithOne(cn => cn.CommentVerse)
                    .HasForeignKey<CommentVerse>(cn => cn.CommentId)
                    .OnDelete(DeleteBehavior.Cascade); //TODO: ON DELETE CASCADE
            });

            modelBuilder.Entity<CommentNote>(CommentNote =>
            {
                CommentNote.ToTable("comment_note");

                CommentNote.HasKey(e => e.CommentId);

                CommentNote.Property(e => e.CommentId)
                    .HasColumnName("comment_id")
                    .HasColumnType(Utility.DBType64bitInteger);

                CommentNote.Property(e => e.NoteId)
                    .HasColumnName("note_id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                CommentNote.HasOne(e => e.Note)
                    .WithMany(v => v.Comments)
                    .HasForeignKey(e => e.NoteId)
                    .OnDelete(DeleteBehavior.NoAction); //TODO: ON DELETE CASCADE

                CommentNote.HasOne(c => c.Comment)
                    .WithOne(cn => cn.CommentNote)
                    .HasForeignKey<CommentNote>(cn => cn.CommentId)
                    .OnDelete(DeleteBehavior.Cascade); //TODO: ON DELETE CASCADE
            });

            modelBuilder.Entity<Follow>(Follow =>
            {
                Follow.ToTable("follow");

                Follow.HasKey(e => e.Id);

                Follow.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    ;

                Follow.Property(e => e.FollowerId)
                    .HasColumnName("follower_id")
                    .IsRequired();

                Follow.Property(e => e.FollowedId)
                    .HasColumnName("followed_id")
                    .IsRequired();

                Follow.Property(e => e.Status)
                    .HasColumnName("status").HasConversion<int>()
                    .IsRequired();

                Follow.Property(e => e.OccurredAt)
                    .HasColumnName("occurred_at")
                    .IsRequired()
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                Follow.HasIndex(e => new { e.FollowerId, e.FollowedId })
                    .IsUnique();

                Follow.HasOne(e => e.Follower)
                    .WithMany(u => u.Followings)
                    .HasForeignKey(e => e.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE

                Follow.HasOne(e => e.Followed)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(e => e.FollowedId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE
            });


            modelBuilder.Entity<Block>(Block =>
            {
                Block.ToTable("block");

                Block.HasKey(e => e.Id);

                Block.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    ;

                Block.Property(e => e.BlockerId)
                    .HasColumnName("blocker_id")
                    .IsRequired();

                Block.Property(e => e.BlockedId)
                    .HasColumnName("blocked_id")
                    .IsRequired();

                Block.Property(e => e.BlockedAt)
                    .HasColumnName("blocked_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                Block.Property(e => e.Reason)
                    .HasColumnName("reason")
                    .HasColumnType(Utility.DBTypeVARCHAR100)
                    .HasMaxLength(100);

                Block.HasIndex(e => new { e.BlockerId, e.BlockedId })
                    .IsUnique();

                Block.HasOne(e => e.Blocker)
                    .WithMany(u => u.BlockedUsers)
                    .HasForeignKey(e => e.BlockerId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE

                Block.HasOne(e => e.Blocked)
                    .WithMany(u => u.BlockedByUsers)
                    .HasForeignKey(e => e.BlockedId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE
            });

            modelBuilder.Entity<FreezeR>(FreezeR =>
            {
                FreezeR.ToTable("freeze_r");

                FreezeR.HasKey(e => e.Id);

                FreezeR.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    ;

                FreezeR.Property(e => e.Status)
                    .HasColumnName("status").HasConversion<int>()
                    .IsRequired();

                FreezeR.Property(e => e.UserId)
                    .HasColumnName("user_id").HasColumnType(Utility.DBTypeUUID)
                    .IsRequired();

                FreezeR.Property(e => e.ProceedAt)
                    .HasColumnName("proceed_at")
                    .IsRequired()
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                FreezeR.HasOne(e => e.User)
                    .WithMany(u => u.FreezeRecords)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Like>(Like =>
            {
                Like.ToTable("lke");

                Like.HasKey(e => e.Id);

                Like.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    ;

                Like.Property(e => e.UserId)
                    .HasColumnName("user_id").HasColumnType(Utility.DBTypeUUID)
                    .IsRequired();

                Like.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                Like.HasIndex(e => e.UserId);

                Like.HasOne(e => e.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                Like.HasOne(e => e.LikeComment)
                    .WithOne(lc => lc.Like)
                    .HasForeignKey<LikeComment>(lc => lc.LikeId)
                    .OnDelete(DeleteBehavior.Restrict);

                Like.HasOne(e => e.LikeNote)
                    .WithOne(ln => ln.Like)
                    .HasForeignKey<LikeNote>(ln => ln.LikeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LikeComment>(LikeComment =>
            {
                LikeComment.ToTable("like_comment");

                LikeComment.HasKey(e => e.LikeId);

                LikeComment.Property(e => e.LikeId)
                    .HasColumnName("likeId")
                    .IsRequired();

                LikeComment.Property(e => e.CommentId)
                    .HasColumnName("comment_id")
                    .IsRequired();

                LikeComment.HasIndex(e => e.CommentId);

                LikeComment.HasOne(e => e.Comment)
                    .WithMany(c => c.LikeComments)
                    .HasForeignKey(e => e.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                LikeComment.HasOne(l => l.Like)
                    .WithOne(c => c.LikeComment)
                    .HasForeignKey<LikeComment>(e => e.LikeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LikeNote>(LikeNote =>
            {
                LikeNote.ToTable("like_note");

                LikeNote.HasKey(e => e.LikeId);

                LikeNote.Property(e => e.LikeId)
                    .HasColumnName("like_id").HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                LikeNote.Property(e => e.NoteId)
                    .HasColumnName("note_id").HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired();

                LikeNote.HasIndex(e => e.NoteId);

                LikeNote.HasOne(e => e.Note)
                    .WithMany(n => n.Likes)
                    .HasForeignKey(e => e.NoteId)
                    .OnDelete(DeleteBehavior.Cascade);

                LikeNote.HasOne(e => e.Like)
                    .WithOne(n => n.LikeNote)
                    .HasForeignKey<LikeNote>(e => e.LikeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Notification>(Notification =>
            {
                Notification.ToTable("notification");

                Notification.HasKey(e => e.Id);

                Notification.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired();

                Notification.Property(e => e.RecipientId)
                    .HasColumnName("recipient_id")
                    .IsRequired();

                Notification.Property(e => e.ActorId)
                    .HasColumnName("actor_id")
                    .IsRequired();

                Notification.Property(e => e.NotificationType)
                    .HasColumnName("notification_type").HasConversion<int>()
                    .IsRequired();

                Notification.Property(e => e.EntityType).HasConversion<int>()
                    .HasColumnName("entity_type");

                Notification.Property(e => e.EntityId)
                    .HasColumnName("entity_id");

                Notification.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                Notification.Property(e => e.IsRead)
                    .HasColumnName("is_read")
                    .HasDefaultValue(false);

                Notification.HasOne(e => e.Recipient)
                    .WithMany(u => u.NotificationsReceived)
                    .HasForeignKey(e => e.RecipientId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE

                Notification.HasOne(e => e.Actor)
                    .WithMany(u => u.NotificationsSent)
                    .HasForeignKey(e => e.ActorId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE

                //TODO: Add check "recipient_id <> actor_id AND ((entity_type IS NOT NULL AND entity_id IS NOT NULL) OR (entity_type IS NULL AND entity_id IS NULL))");
            });



            modelBuilder.Entity<Suggestion>(Suggestion =>
            {
                Suggestion.ToTable("suggestion");

                Suggestion.HasKey(e => e.Id);

                Suggestion.Property(e => e.UserId).HasColumnName("user_id").HasColumnType(Utility.DBTypeUUID)
                    .IsRequired();

                Suggestion.Property(e => e.TranslationTextId)
                    .IsRequired();

                Suggestion.Property(e => e.SuggestionText)
                    .HasColumnName("suggestion_text").HasColumnType(Utility.DBTypeVARCHAR500)
                    .HasMaxLength(500).IsRequired();

                Suggestion.Property(e => e.CreatedAt)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                Suggestion.HasIndex(e => new { e.UserId, e.TranslationTextId })
                    .IsUnique();

                Suggestion.HasOne(e => e.User)
                    .WithMany(u => u.Suggestions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                Suggestion.HasOne(e => e.TranslationText)
                    .WithMany(tt => tt.Suggestions)
                    .HasForeignKey(e => e.TranslationTextId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }

        //Custom functions:

        public async Task<List<CommentOwnerDto>> GetVerseCommentsAsync(User userRequested, long verseId,
            bool OrderByDecrement = false)
        {
            SqlParameter userIdParam = new("@UserId", userRequested.Id);
            SqlParameter verseIdParam = new("@VerseId", verseId);

            try
            {
                IQueryable<Comment>? result = Comment
                    .FromSqlRaw("SELECT * FROM dbo.GetVerseCommentHierarchy(@UserId, @VerseId)", userIdParam,
                        verseIdParam)
                    .Include(c => c.User)
                    .Include(c => c.LikeComments)
                    .ThenInclude(lc => lc.Like);

                if (result == null)
                    return [];

                if (OrderByDecrement)
                    result = result.OrderByDescending(c => c.CreatedAt);

                List<Comment> Comments = await result.ToListAsync();

                List<CommentOwnerDto> data = [];

                foreach (Comment comment in Comments)
                    data.Add(comment.ToCommentOwnerDto(true, userRequested));

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred in GetVerseCommentsAsync | UserId: {UserId} | VerseId: {VerseId} | Message: {Message}",
                    userRequested.Id, verseId, ex.Message);

                return [];
            }
        }

        public HashSet<long> GetAvailableVerseCommentIds(User userRequested, long verseId)
        {
            SqlParameter userIdParam = new("@UserId", userRequested.Id);
            SqlParameter verseIdParam = new("@VerseId", verseId);


            return Comment
                .FromSqlRaw("SELECT * FROM dbo.GetVerseCommentHierarchy(@UserId, @VerseId)", userIdParam, verseIdParam)
                .Select(c => c.Id).ToHashSet();
        }

        public async Task<List<CommentOwnerDto>> GetNoteCommentsAsync(User userRequested, long NoteId,
            bool OrderByDecrement = false)
        {
            SqlParameter UserIdParam = new("@UserId", userRequested.Id);
            SqlParameter NoteIdParam = new("@NoteId", NoteId);

            var Result = Comment
                .FromSqlRaw("SELECT * FROM dbo.GetVerseCommentHierarchy(@UserId, @NoteId)", UserIdParam, NoteIdParam)
                .Include(c => c.User).Select(c => c.ToCommentOwnerDto(true, userRequested));

            if (OrderByDecrement)
                Result = Result.OrderByDescending(c => c.CreatedAt);

            return await Result.ToListAsync();
        }

        public HashSet<long> GetAvailableNoteCommentIds(Guid userId, long noteId)
        {
            SqlParameter UserIdParam = new("@UserId", userId);
            SqlParameter NoteIdParam = new("@NoteId", noteId);


            return Comment
                .FromSqlRaw("SELECT * FROM dbo.GetNoteCommentHierarchy(@UserId, @NoteId)", UserIdParam, NoteIdParam)
                .Select(c => c.Id).ToHashSet();
        }
    }

    */
        }
    }
}