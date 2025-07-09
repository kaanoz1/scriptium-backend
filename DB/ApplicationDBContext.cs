using DTO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using scriptium_backend_dotnet.Models;
using scriptium_backend_dotnet.Models.Util;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace scriptium_backend_dotnet.DB
{
    public class ApplicationDBContext(
        DbContextOptions<ApplicationDBContext> options,
        ILogger<ApplicationDBContext> logger) : IdentityDbContext<User, Role, Guid>(options)
    {
        private readonly ILogger<ApplicationDBContext> _logger = logger;

        public DbSet<Language> Language { get; set; }

        public DbSet<Scripture> Scripture { get; set; }

        public DbSet<ScriptureMeaning> ScriptureMeaning { get; set; }

        public DbSet<Section> Section { get; set; }

        public DbSet<SectionMeaning> SectionMeaning { get; set; }

        public DbSet<Chapter> Chapter { get; set; }

        public DbSet<ChapterMeaning> ChapterMeaning { get; set; }

        public DbSet<Root> Root { get; set; }

        public DbSet<Verse> Verse { get; set; }

        public DbSet<Word> Word { get; set; }

        public DbSet<WordMeaning> WordMeaning { get; set; }

        public DbSet<Transliteration> Transliteration { get; set; }

        public DbSet<Translator> Translator { get; set; }

        public DbSet<Translation> Translation { get; set; }

        public DbSet<TranslatorTranslation> TranslatorTranslation { get; set; }

        public DbSet<TranslationText> TranslationText { get; set; }

        public DbSet<FootNoteText> FootNoteText { get; set; }

        public DbSet<FootNote> FootNote { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Session> Session { get; set; }

        public DbSet<Collection> Collection { get; set; }

        public DbSet<CollectionVerse> CollectionVerse { get; set; }

        public DbSet<Note> Note { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<CommentVerse> CommentVerse { get; set; }

        public DbSet<CommentNote> CommentNote { get; set; }

        public DbSet<Follow> Follow { get; set; }

        public DbSet<FollowR> FollowR { get; set; }

        public DbSet<Block> Block { get; set; }

        public DbSet<FreezeR> FreezeR { get; set; }

        public DbSet<Like> Like { get; set; }

        public DbSet<LikeComment> LikeComment { get; set; }

        public DbSet<LikeNote> LikeNote { get; set; }

        public DbSet<Notification> Notification { get; set; }

        public DbSet<Cache> Cache { get; set; }

        public DbSet<CacheR> CacheR { get; set; }

        public DbSet<Suggestion> Suggestion { get; set; }

        public DbSet<UserUpdateR> UserUpdateRs { get; set; }

        //public DbSet<RequestLog> RequestLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Language>(Language =>
            {
                Language.ToTable("language");

                Language.HasKey(e => e.Id);

                Language.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired(true);

                Language.HasIndex(e => e.LangCode)
                    .IsUnique();

                Language.Property(l => l.LangCode).HasColumnName("lang_code").HasColumnType(Utility.DBTypeVARCHAR2)
                    .IsRequired(true);

                Language.Property(l => l.LangEnglish).HasColumnName("lang_english")
                    .HasColumnType(Utility.DBTypeVARCHAR16).IsRequired(true);

                Language.Property(l => l.LangOwn).HasColumnName("lang_own").HasColumnType(Utility.DBTypeVARCHAR16)
                    .IsRequired(true);


                Language.HasMany(l => l.ScriptureMeanings).WithOne(sm => sm.Language).OnDelete(DeleteBehavior.NoAction);
                Language.HasMany(l => l.SectionMeanings).WithOne(sm => sm.Language).OnDelete(DeleteBehavior.NoAction);
                Language.HasMany(l => l.ChapterMeanings).WithOne(cm => cm.Language).OnDelete(DeleteBehavior.NoAction);
                Language.HasMany(l => l.WordMeanings).WithOne(wm => wm.Language).OnDelete(DeleteBehavior.NoAction);
                Language.HasMany(l => l.Transliterations).WithOne(t => t.Language).OnDelete(DeleteBehavior.NoAction);
                Language.HasMany(l => l.Translators).WithOne(t => t.Language).OnDelete(DeleteBehavior.NoAction);
                Language.HasMany(l => l.Translations).WithOne(t => t.Language).OnDelete(DeleteBehavior.NoAction);
                Language.HasMany(l => l.PreferredUsers).WithOne(u => u.PreferredLanguage)
                    .OnDelete(DeleteBehavior.NoAction);

                Language.HasData(
                    new Language { Id = 1, LangCode = "en", LangOwn = "English", LangEnglish = "English" },
                    new Language { Id = 2, LangCode = "de", LangOwn = "Deutsch", LangEnglish = "German" }
                );
            });

            modelBuilder.Entity<Scripture>(Scripture =>

            {
                Scripture.ToTable("scripture");

                Scripture.HasKey(s => s.Id);

                Scripture.Property(s => s.Id).HasColumnName("id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired(true).ValueGeneratedOnAdd();

                Scripture.Property(s => s.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeNVARCHAR255).IsRequired(true);

                Scripture.Property(s => s.Code)
                    .HasColumnName("code")
                    .HasColumnType(Utility.DBTypeCHAR1).HasMaxLength(1).IsRequired(true);

                Scripture.Property(s => s.Number)
                    .HasColumnName("number")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired(true);

                Scripture.HasIndex(e => e.Name)
                    .IsUnique();

                Scripture.HasIndex(e => e.Code)
                    .IsUnique();

                Scripture.HasMany(s => s.Meanings).WithOne(m => m.Scripture).OnDelete(DeleteBehavior.NoAction);
                Scripture.HasMany(s => s.Sections).WithOne(s => s.Scripture).OnDelete(DeleteBehavior.Restrict);
                Scripture.HasMany(s => s.Roots).WithOne(r => r.Scripture).OnDelete(DeleteBehavior.Restrict);
                Scripture.HasMany(s => s.Translations).WithOne(r => r.Scripture).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ScriptureMeaning>(ScriptureMeaning =>
            {
                ScriptureMeaning.ToTable("scripture_meaning");

                ScriptureMeaning.HasKey(s => s.Id);

                ScriptureMeaning.Property(sm => sm.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                ScriptureMeaning.Property(sm => sm.Meaning).HasColumnName("meaning")
                    .HasColumnType(Utility.DBTypeVARCHAR50).IsRequired(true);

                ScriptureMeaning.Property(sm => sm.ScriptureId).HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired(true);

                ScriptureMeaning.HasOne(c => c.Scripture)
                    .WithMany(p => p.Meanings)
                    .HasForeignKey(c => c.ScriptureId)
                    .OnDelete(DeleteBehavior.Cascade);

                ScriptureMeaning.Property(sm => sm.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired(true);

                ScriptureMeaning.HasOne(c => c.Language)
                    .WithMany(p => p.ScriptureMeanings)
                    .HasForeignKey(c => c.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                ScriptureMeaning.HasIndex(e => new { e.LanguageId, e.ScriptureId })
                    .IsUnique();
            });

            modelBuilder.Entity<Section>(Section =>
            {
                Section.ToTable("section");

                Section.HasKey(s => s.Id);

                Section.Property(s => s.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                Section.Property(s => s.Name).HasColumnName("name").HasColumnType(Utility.DBTypeNVARCHAR255)
                    .IsRequired(true);

                Section.Property(s => s.ScriptureId).HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired(true);

                Section.HasOne(s => s.Scripture)
                    .WithMany(scr => scr.Sections)
                    .HasForeignKey(s => s.ScriptureId)
                    .OnDelete(DeleteBehavior.Restrict);

                Section.HasIndex(e => e.Name)
                    .IsUnique();

                Section.HasIndex(e => new { e.ScriptureId, e.Number })
                    .IsUnique();

                Section.HasMany(s => s.Chapters).WithOne(c => c.Section).OnDelete(DeleteBehavior.Restrict);

                Section.HasMany(s => s.Meanings).WithOne(sm => sm.Section).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<SectionMeaning>(SectionMeaning =>
            {
                SectionMeaning.ToTable("section_meaning");

                SectionMeaning.HasKey(s => s.Id);

                SectionMeaning.Property(s => s.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                SectionMeaning.Property(s => s.Meaning).HasColumnName("meaning").HasColumnType(Utility.DBTypeVARCHAR100)
                    .IsRequired(true);

                SectionMeaning.Property(s => s.SectionId).HasColumnName("section_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired(true);

                SectionMeaning.HasOne(c => c.Section)
                    .WithMany(e => e.Meanings)
                    .HasForeignKey(e => e.SectionId)
                    .OnDelete(DeleteBehavior.Restrict);

                SectionMeaning.Property(s => s.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired(true);

                SectionMeaning.HasOne(c => c.Language)
                    .WithMany(e => e.SectionMeanings)
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                SectionMeaning.HasIndex(e => new { e.LanguageId, e.SectionId })
                    .IsUnique();
            });

            modelBuilder.Entity<Chapter>(Chapter =>
            {
                Chapter.ToTable("chapter");

                Chapter.HasKey(c => c.Id);

                Chapter.Property(c => c.Id).HasColumnName("id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                Chapter.Property(c => c.Name).HasColumnName("name").HasColumnType(Utility.DBTypeNVARCHAR255)
                    .IsRequired(true);

                Chapter.Property(c => c.Number).HasColumnName("number").HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired(true);

                Chapter.Property(c => c.SectionId).HasColumnName("section_id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                Chapter.HasOne(c => c.Section)
                    .WithMany(s => s.Chapters)
                    .HasForeignKey(c => c.SectionId)
                    .OnDelete(DeleteBehavior.Restrict);

                Chapter.HasMany(c => c.Verses).WithOne(v => v.Chapter).OnDelete(DeleteBehavior.Restrict);

                Chapter.HasMany(c => c.Meanings).WithOne(m => m.Chapter).OnDelete(DeleteBehavior.NoAction);

                Chapter.HasIndex(c => c.Name)
                    .IsUnique();

                Chapter.HasIndex(c => new { c.SectionId, c.Number })
                    .IsUnique();
            });

            modelBuilder.Entity<ChapterMeaning>(ChapterMeaning =>
            {
                ChapterMeaning.ToTable("chapter_meaning");

                ChapterMeaning.HasKey(cm => cm.Id);

                ChapterMeaning.Property(cm => cm.Id).HasColumnName("id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);

                ChapterMeaning.Property(s => s.Meaning).HasColumnName("meaning").HasColumnType(Utility.DBTypeVARCHAR100)
                    .IsRequired(true);

                ChapterMeaning.Property(s => s.ChapterId).HasColumnName("chapter_id")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired(true);

                ChapterMeaning.HasOne(c => c.Chapter)
                    .WithMany(p => p.Meanings)
                    .HasForeignKey(c => c.ChapterId)
                    .OnDelete(DeleteBehavior.Restrict);

                ChapterMeaning.Property(s => s.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger).IsRequired(true);

                ChapterMeaning.HasOne(cm => cm.Language)
                    .WithMany(l => l.ChapterMeanings)
                    .HasForeignKey(cm => cm.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                ChapterMeaning.HasIndex(cm => new { cm.ChapterId, cm.LanguageId })
                    .IsUnique();
            });

            modelBuilder.Entity<Root>(Root =>
            {
                Root.ToTable("root");

                Root.HasKey(r => r.Id);

                Root.Property(r => r.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true);

                Root.Property(r => r.Latin)
                    .HasColumnName("latin")
                    .HasColumnType(Utility.DBTypeNVARCHAR30)
                    .IsRequired(true)
                    .UseCollation("SQL_Latin1_General_CP1_CS_AS"); // Case-sensitive

                Root.Property(r => r.Own)
                    .HasColumnName("own")
                    .HasColumnType(Utility.DBTypeNVARCHAR30)
                    .IsRequired(true);

                Root.Property(r => r.ScriptureId)
                    .HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                Root.HasOne(r => r.Scripture)
                    .WithMany(sc => sc.Roots)
                    .HasForeignKey(r => r.ScriptureId)
                    .OnDelete(DeleteBehavior.Restrict);

                Root.HasMany(r => r.Words).WithMany(w => w.Roots);

                Root.HasIndex(r => new { r.Latin, r.ScriptureId })
                    .IsUnique(true);
            });


            modelBuilder.Entity<Verse>(Verse =>
            {
                Verse.ToTable("verse");

                Verse.HasKey(v => v.Id);

                Verse.Property(v => v.Id).HasColumnName("id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);

                Verse.Property(v => v.Number).HasColumnName("number").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                Verse.Property(v => v.Text).HasColumnName("text").HasColumnType(Utility.DBTypeNVARCHAR1000)
                    .IsRequired(true);

                Verse.Property(v => v.TextWithoutVowel).HasColumnName("text_without_vowel")
                    .HasColumnType(Utility.DBTypeNVARCHAR1000).IsRequired(false);

                Verse.Property(v => v.TextSimplified).HasColumnName("text_simplified")
                    .HasColumnType(Utility.DBTypeNVARCHAR1000).IsRequired(false);

                Verse.Property(v => v.ChapterId).HasColumnName("chapter_id").HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                Verse.HasOne(v => v.Chapter)
                    .WithMany(c => c.Verses)
                    .HasForeignKey(v => v.ChapterId)
                    .OnDelete(DeleteBehavior.Restrict);

                Verse.HasMany(v => v.Words).WithOne(w => w.Verse).OnDelete(DeleteBehavior.Restrict);
                Verse.HasMany(v => v.Transliterations).WithOne(t => t.Verse).OnDelete(DeleteBehavior.NoAction);
                Verse.HasMany(v => v.TranslationTexts).WithOne(tt => tt.Verse).OnDelete(DeleteBehavior.NoAction);
                Verse.HasMany(v => v.CollectionVerses).WithOne(w => w.Verse).OnDelete(DeleteBehavior.NoAction);
                Verse.HasMany(v => v.Notes).WithOne(w => w.Verse).OnDelete(DeleteBehavior.NoAction);
                Verse.HasMany(v => v.Comments).WithOne(w => w.Verse).OnDelete(DeleteBehavior.NoAction);

                Verse.HasIndex(v => new { v.ChapterId, v.Number })
                    .IsUnique();
            });

            modelBuilder.Entity<Word>(Word =>
            {
                Word.ToTable("word");

                Word.HasKey(w => w.Id);

                Word.Property(v => v.Id).HasColumnName("id").HasColumnType(Utility.DBType64bitInteger).IsRequired(true);

                Word.Property(v => v.SequenceNumber).HasColumnName("sequence_number")
                    .HasColumnType(Utility.DBType16bitInteger).IsRequired(true);

                Word.Property(v => v.Text).HasColumnName("text").HasColumnType(Utility.DBTypeNVARCHAR50)
                    .IsRequired(true);

                Word.Property(v => v.TextWithoutVowel).HasColumnName("text_without_vowel")
                    .HasColumnType(Utility.DBTypeNVARCHAR50);

                Word.Property(v => v.TextSimplified).HasColumnName("text_simplified")
                    .HasColumnType(Utility.DBTypeNVARCHAR50);

                Word.Property(v => v.VerseId).HasColumnName("verse_id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);

                Word.HasOne(w => w.Verse)
                    .WithMany(v => v.Words)
                    .HasForeignKey(w => w.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);


                Word.HasMany(w => w.Roots)
                    .WithMany(r => r.Words);


                Word.HasMany(w => w.WordMeanings)
                    .WithOne(r => r.Word)
                    .OnDelete(DeleteBehavior.NoAction);

                Word.HasIndex(w => new { w.SequenceNumber, w.VerseId })
                    .IsUnique();
            });

            modelBuilder.Entity<WordMeaning>(WordMeaning =>
            {
                WordMeaning.ToTable("word_meaning");

                WordMeaning.HasKey(wm => wm.Id);

                WordMeaning.Property(wm => wm.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true);

                WordMeaning.Property(wm => wm.Meaning)
                    .HasColumnName("meaning")
                    .HasColumnType(Utility.DBTypeVARCHAR100)
                    .IsRequired(true);

                WordMeaning.Property(wm => wm.WordId).HasColumnName("word_id").HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true);

                WordMeaning.Property(wm => wm.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired(true);

                WordMeaning.HasOne(wm => wm.Word)
                    .WithMany(w => w.WordMeanings)
                    .HasForeignKey(wm => wm.WordId)
                    .OnDelete(DeleteBehavior.Restrict);

                WordMeaning.HasOne(wm => wm.Language)
                    .WithMany(l => l.WordMeanings)
                    .HasForeignKey(wm => wm.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transliteration>(Transliteration =>
            {
                Transliteration.ToTable("transliteration");

                Transliteration.HasKey(t => t.Id);

                Transliteration.Property(t => t.Id)
                    .HasColumnName("id").HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);

                Transliteration.Property(t => t.Text).HasColumnName("text")
                    .HasColumnType(Utility.DBTypeVARCHARMAX)
                    .IsRequired(true);

                Transliteration.Property(t => t.LanguageId).HasColumnName("language_id")
                    .HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired(true);

                Transliteration.HasOne(t => t.Language)
                    .WithMany(l => l.Transliterations)
                    .HasForeignKey(t => t.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                Transliteration.HasOne(t => t.Verse)
                    .WithMany(v => v.Transliterations)
                    .HasForeignKey(t => t.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);

                Transliteration.HasIndex(e => new { e.VerseId, e.LanguageId }).IsUnique(true);
            });

            modelBuilder.Entity<Translator>(Translator =>
            {
                Translator.ToTable("translator");

                Translator.HasKey(e => e.Id);

                Translator.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                Translator.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeVARCHAR250)
                    .IsRequired();

                Translator.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType(Utility.DBTypeVARCHARMAX);

                Translator.Property(e => e.Url)
                    .HasColumnType(Utility.DBTypeVARCHARMAX);

                Translator.Property(e => e.LanguageId)
                    .HasColumnName("language_id");

                Translator.HasOne(e => e.Language)
                    .WithMany(l => l.Translators)
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                Translator.HasMany(e => e.TranslatorTranslations)
                    .WithOne(tt => tt.Translator)
                    .HasForeignKey(tt => tt.TranslatorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Translation>(Translation =>
            {
                Translation.ToTable("translation");

                Translation.HasKey(t => t.Id);

                Translation.Property(t => t.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);


                Translation.Property(t => t.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeVARCHAR250)
                    .IsRequired(true);

                Translation.Property(t => t.ProductionTime)
                    .HasColumnName("production_year")
                    .HasColumnType(Utility.DBTypeDateTime);

                Translation.Property(t => t.AddedAt)
                    .HasColumnName("added_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired(true);

                Translation.Property(t => t.EagerFrom)
                    .HasColumnName("eager_from")
                    .HasColumnType(Utility.DBTypeDateTime);

                Translation.Property(t => t.LanguageId)
                    .HasColumnName("language_id").HasColumnType(Utility.DBType8bitInteger)
                    .IsRequired(true)
                    .HasDefaultValue(1);

                Translation.Property(t => t.ScriptureId)
                    .HasColumnName("scripture_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);


                Translation.HasOne(t => t.Language)
                    .WithMany(l => l.Translations)
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                Translation.HasMany(t => t.TranslatorTranslations)
                    .WithOne(tt => tt.Translation)
                    .HasForeignKey(t => t.TranslationId)
                    .OnDelete(DeleteBehavior.NoAction);

                Translation.HasMany(e => e.TranslationTexts)
                    .WithOne(ttx => ttx.Translation)
                    .HasForeignKey(ttx => ttx.TranslationId)
                    .OnDelete(DeleteBehavior.NoAction);

                Translation.HasOne(t => t.Scripture)
                    .WithMany(s => s.Translations)
                    .HasForeignKey(t => t.ScriptureId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TranslatorTranslation>(TranslatorTranslation =>
            {
                TranslatorTranslation.ToTable("translator_translation");

                TranslatorTranslation.HasKey(tt => new { tt.TranslatorId, tt.TranslationId });

                TranslatorTranslation.Property(tt => tt.TranslatorId)
                    .HasColumnName("translator_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                TranslatorTranslation.Property(tt => tt.TranslationId)
                    .HasColumnName("translation_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                TranslatorTranslation.Property(tt => tt.AssignedOn)
                    .HasColumnName("assigned_on")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired(true);


                TranslatorTranslation.HasOne(tt => tt.Translator)
                    .WithMany(tr => tr.TranslatorTranslations)
                    .HasForeignKey(tt => tt.TranslatorId)
                    .OnDelete(DeleteBehavior.Cascade);

                TranslatorTranslation.HasOne(tt => tt.Translation)
                    .WithMany(tr => tr.TranslatorTranslations)
                    .HasForeignKey(tt => tt.TranslationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TranslationText>(TranslationText =>
            {
                TranslationText.ToTable("translation_text");

                TranslationText.HasKey(tt => tt.Id);

                TranslationText.Property(tt => tt.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true)
                    ;

                TranslationText.Property(tt => tt.Text).HasColumnName("text").HasColumnType(Utility.DBTypeVARCHARMAX)
                    .IsRequired(true);

                TranslationText.Property(tt => tt.TranslationId)
                    .HasColumnName("translation_id")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                TranslationText.Property(tt => tt.VerseId)
                    .HasColumnName("verse_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);


                TranslationText.HasOne(tt => tt.Translation)
                    .WithMany(tr => tr.TranslationTexts)
                    .HasForeignKey(tt => tt.TranslationId)
                    .OnDelete(DeleteBehavior.Cascade);

                TranslationText.HasOne(tt => tt.Verse)
                    .WithMany(v => v.TranslationTexts)
                    .HasForeignKey(tt => tt.VerseId)
                    .OnDelete(DeleteBehavior.Restrict);

                TranslationText.HasMany(tt => tt.FootNotes)
                    .WithOne(fn => fn.TranslationText)
                    .HasForeignKey(fn => fn.TranslationTextId)
                    .OnDelete(DeleteBehavior.NoAction);

                TranslationText.HasMany(tt => tt.Suggestions)
                    .WithOne(s => s.TranslationText)
                    .OnDelete(DeleteBehavior.NoAction);

                TranslationText.HasIndex(tt => new { tt.VerseId, tt.TranslationId });
            });

            modelBuilder.Entity<FootNoteText>(FootNoteText =>
            {
                FootNoteText.ToTable("footnote_text");

                FootNoteText.HasKey(ft => ft.Id);

                FootNoteText.Property(ft => ft.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true)
                    ;

                FootNoteText.Property(ft => ft.Text).HasColumnName("text").HasColumnType(Utility.DBTypeNVARCHARMAX)
                    .IsRequired(true);

                FootNoteText.HasMany(ftnt => ftnt.FootNotes).WithOne(ft => ft.FootNoteText)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<FootNote>(FootNote =>
            {
                FootNote.ToTable("footnote");

                FootNote.HasKey(fn => fn.Id);

                FootNote.Property(fn => fn.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true)
                    ;

                FootNote.Property(fn => fn.Index)
                    .HasColumnName("index")
                    .HasColumnType(Utility.DBType16bitInteger)
                    .IsRequired(true);

                FootNote.HasOne(fn => fn.TranslationText)
                    .WithMany(tt => tt.FootNotes)
                    .HasForeignKey(fn => fn.TranslationTextId)
                    .OnDelete(DeleteBehavior.Cascade);

                FootNote.HasOne(fn => fn.FootNoteText)
                    .WithMany(fnt => fnt.FootNotes)
                    .HasForeignKey(fn => fn.FootNoteTextId)
                    .OnDelete(DeleteBehavior.Cascade);

                FootNote.HasIndex(fn => new { fn.Index, fn.TranslationTextId });
            });

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
                    .IsRequired(true);

                User.Property(e => e.UserName)
                    .HasColumnName("username")
                    .HasColumnType(Utility.DBTypeVARCHAR16)
                    .HasMaxLength(16)
                    .IsRequired(true)
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
                    .IsRequired(true);

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
                    .IsRequired(true);

                User.HasIndex(e => e.NormalizedEmail)
                    .IsUnique(true);

                User.HasIndex(e => e.NormalizedUserName)
                    .IsUnique(true);

                User.HasIndex(e => e.Email)
                    .IsUnique(true);

                User.HasIndex(e => e.UserName)
                    .IsUnique(true);

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
                    .IsRequired(true);

                Collection.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType(Utility.DBTypeVARCHAR24)
                    .HasMaxLength(24)
                    .IsRequired(true)
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
                    .IsRequired(true);

                Collection.HasIndex(e => new { e.UserId, e.Name })
                    .IsUnique(true);

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
                    .HasColumnType(Utility.DBType64bitInteger).IsRequired(true);

                CollectionVerse.Property(e => e.CollectionId)
                    .HasColumnName("collection_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);

                CollectionVerse.Property(e => e.VerseId)
                    .HasColumnName("verse_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);

                CollectionVerse.Property(e => e.SavedAt)
                    .HasColumnName("saved_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired(true);

                CollectionVerse.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasColumnType(Utility.DBTypeVARCHAR250)
                    .HasMaxLength(250)
                    .IsRequired(false);

                CollectionVerse.HasIndex(e => new { e.CollectionId, e.VerseId })
                    .IsUnique(true);

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
                    .HasColumnType(Utility.DBType64bitInteger).IsRequired(true);

                Note.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType(Utility.DBTypeUUID)
                    .IsRequired(true);

                Note.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType(Utility.DBTypeVARCHARMAX)
                    .IsRequired(true);

                Note.Property(e => e.VerseId)
                    .HasColumnName("verse_id")
                    .HasColumnType(Utility.DBType32bitInteger)
                    .IsRequired(true);

                Note.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired(true);

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
                    .IsRequired(true);

                Comment.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType(Utility.DBTypeUUID)
                    .IsRequired();

                Comment.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType(Utility.DBTypeVARCHAR500)
                    .IsRequired(true);

                Comment.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired(true);

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
                    .IsRequired(true);

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
                    .IsRequired(true);

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
                    .IsRequired(true)
                    ;

                Follow.Property(e => e.FollowerId)
                    .HasColumnName("follower_id")
                    .IsRequired(true);

                Follow.Property(e => e.FollowedId)
                    .HasColumnName("followed_id")
                    .IsRequired(true);

                Follow.Property(e => e.Status)
                    .HasColumnName("status").HasConversion<int>()
                    .IsRequired(true);

                Follow.Property(e => e.OccurredAt)
                    .HasColumnName("occurred_at")
                    .IsRequired(true)
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

            modelBuilder.Entity<FollowR>(FollowR =>
            {
                FollowR.ToTable("follow_r");

                FollowR.HasKey(e => e.Id);

                FollowR.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired(true)
                    ;

                FollowR.Property(e => e.FollowerId)
                    .HasColumnName("follower_id")
                    .IsRequired(true);

                FollowR.Property(e => e.FollowedId)
                    .HasColumnName("followed_id")
                    .IsRequired(true);

                FollowR.Property(e => e.Status)
                    .HasColumnName("status").HasConversion<int>()
                    .IsRequired(true);

                FollowR.Property(e => e.OccurredAt)
                    .HasColumnName("occurred_at")
                    .IsRequired(true)
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                FollowR.HasIndex(e => new { e.FollowerId, e.FollowedId })
                    .IsUnique();

                FollowR.HasOne(e => e.Follower)
                    .WithMany(u => u.FollowRing)
                    .HasForeignKey(e => e.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE

                FollowR.HasOne(e => e.Followed)
                    .WithMany(u => u.FollowerRs)
                    .HasForeignKey(e => e.FollowedId)
                    .OnDelete(DeleteBehavior.Restrict); //TODO: ON DELETE CASCADE
            });

            modelBuilder.Entity<Block>(Block =>
            {
                Block.ToTable("block");

                Block.HasKey(e => e.Id);

                Block.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired(true)
                    ;

                Block.Property(e => e.BlockerId)
                    .HasColumnName("blocker_id")
                    .IsRequired(true);

                Block.Property(e => e.BlockedId)
                    .HasColumnName("blocked_id")
                    .IsRequired(true);

                Block.Property(e => e.BlockedAt)
                    .HasColumnName("blocked_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                Block.Property(e => e.Reason)
                    .HasColumnName("reason")
                    .HasColumnType(Utility.DBTypeVARCHAR100)
                    .HasMaxLength(100);

                Block.HasIndex(e => new { e.BlockerId, e.BlockedId })
                    .IsUnique(true);

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
                    .IsRequired(true)
                    ;

                FreezeR.Property(e => e.Status)
                    .HasColumnName("status").HasConversion<int>()
                    .IsRequired(true);

                FreezeR.Property(e => e.UserId)
                    .HasColumnName("user_id").HasColumnType(Utility.DBTypeUUID)
                    .IsRequired(true);

                FreezeR.Property(e => e.ProceedAt)
                    .HasColumnName("proceed_at")
                    .IsRequired(true)
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
                    .IsRequired(true)
                    ;

                Like.Property(e => e.UserId)
                    .HasColumnName("user_id").HasColumnType(Utility.DBTypeUUID)
                    .IsRequired(true);

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
                    .IsRequired(true);

                LikeNote.Property(e => e.NoteId)
                    .HasColumnName("note_id").HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true);

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
                    .IsRequired(true);

                Notification.Property(e => e.RecipientId)
                    .HasColumnName("recipient_id")
                    .IsRequired(true);

                Notification.Property(e => e.ActorId)
                    .HasColumnName("actor_id")
                    .IsRequired(true);

                Notification.Property(e => e.NotificationType)
                    .HasColumnName("notification_type").HasConversion<int>()
                    .IsRequired(true);

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

            modelBuilder.Entity<Cache>(Cache =>
            {
                Cache.ToTable("cache");

                Cache.HasKey(e => e.Id);

                Cache.Property(e => e.Id).HasColumnName("id").HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true);

                Cache.Property(e => e.Key).HasColumnName("key").HasColumnType(Utility.DBTypeVARCHAR126)
                    .IsRequired(true);

                Cache.Property(e => e.Data)
                    .HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            });

            modelBuilder.Entity<CacheR>(CacheR =>
            {
                CacheR.ToTable("cache_r");

                CacheR.HasKey(e => e.Id);

                CacheR.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                CacheR.Property(e => e.CacheId)
                    .HasColumnName("cache_id")
                    .HasColumnType(Utility.DBType64bitInteger)
                    .IsRequired(true);

                CacheR.Property(e => e.FetchedAt)
                    .HasColumnName("fetched_at")
                    .HasColumnType(Utility.DBTypeDateTime)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction)
                    .IsRequired(true);

                CacheR.HasOne(e => e.Cache)
                    .WithMany(c => c.CacheRs)
                    .HasForeignKey(e => e.CacheId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Suggestion>(Suggestion =>
            {
                Suggestion.ToTable("suggestion");

                Suggestion.HasKey(e => e.Id);

                Suggestion.Property(e => e.UserId).HasColumnName("user_id").HasColumnType(Utility.DBTypeUUID)
                    .IsRequired(true);

                Suggestion.Property(e => e.TranslationTextId)
                    .IsRequired(true);

                Suggestion.Property(e => e.SuggestionText)
                    .HasColumnName("suggestion_text").HasColumnType(Utility.DBTypeVARCHAR500)
                    .HasMaxLength(500).IsRequired();

                Suggestion.Property(e => e.CreatedAt)
                    .HasDefaultValueSql(Utility.DBDefaultDateTimeFunction);

                Suggestion.HasIndex(e => new { e.UserId, e.TranslationTextId })
                    .IsUnique(true);

                Suggestion.HasOne(e => e.User)
                    .WithMany(u => u.Suggestions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                Suggestion.HasOne(e => e.TranslationText)
                    .WithMany(tt => tt.Suggestions)
                    .HasForeignKey(e => e.TranslationTextId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserUpdateR>(UpdateR =>
            {
                UpdateR.ToTable("user_update_r");

                UpdateR.HasKey(u => u.Id);

                UpdateR.Property(u => u.UserId).HasColumnName("user_id").IsRequired(true);

                UpdateR.HasOne(u => u.User)
                    .WithMany(u => u.UpdateRecords)
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                UpdateR.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasColumnType(Utility.DBTypeVARCHAR16)
                    .HasMaxLength(16)
                    .IsRequired(false)
                    .HasColumnName("username");

                UpdateR.Property(e => e.Name)
                    .HasMaxLength(16).HasColumnType(Utility.DBTypeVARCHAR16)
                    .IsRequired(false);

                UpdateR.Property(e => e.Image).HasColumnName("image").HasColumnType(Utility.DBTypeVARBINARYMAX)
                    .IsRequired(false);

                UpdateR.Property(e => e.Surname).HasColumnName("surname").HasColumnType(Utility.DBTypeVARCHAR16)
                    .HasMaxLength(16)
                    .IsRequired(false);

                UpdateR.Property(e => e.Gender).HasColumnName("gender").HasColumnType(Utility.DBTypeCHAR1)
                    .HasMaxLength(1)
                    .IsRequired(false);

                UpdateR.Property(e => e.Biography)
                    .HasMaxLength(200)
                    .IsRequired(false);

                UpdateR.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsRequired(false);

                UpdateR.Property(e => e.PreferredLanguageId)
                    .IsRequired(false);
            });

            //TODO: Implement Request Log
        }

        //Custom functions:

        public async Task<List<CommentOwnerDTO>> GetVerseCommentsAsync(User userRequested, long verseId, bool OrderByDecrement = false)
        {
            SqlParameter userIdParam = new("@UserId", userRequested.Id);
            SqlParameter verseIdParam = new("@VerseId", verseId);

            try
            {
                IQueryable<Comment>? result = Comment
                    .FromSqlRaw("SELECT * FROM dbo.GetVerseCommentHierarchy(@UserId, @VerseId)", userIdParam, verseIdParam)
                    .Include(c => c.User)
                    .Include(c => c.LikeComments)
                    .ThenInclude(lc => lc.Like);

                if (result == null)
                    return [];

                if (OrderByDecrement)
                    result = result.OrderByDescending(c => c.CreatedAt);

                List<Comment> Comments = await result.ToListAsync();

                List<CommentOwnerDTO> data = [];

                foreach (Comment comment in Comments)
                    data.Add(comment.ToCommentOwnerDTO(true, userRequested));

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

        public async Task<List<CommentOwnerDTO>> GetNoteCommentsAsync(User userRequested, long NoteId,
            bool OrderByDecrement = false)
        {
            SqlParameter UserIdParam = new("@UserId", userRequested.Id);
            SqlParameter NoteIdParam = new("@NoteId", NoteId);

            var Result = Comment
                .FromSqlRaw("SELECT * FROM dbo.GetVerseCommentHierarchy(@UserId, @NoteId)", UserIdParam, NoteIdParam)
                .Include(c => c.User).Select(c => c.ToCommentOwnerDTO(true, userRequested));

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
}