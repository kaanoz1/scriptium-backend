using ScriptiumBackend.Dto.Derived.Islam.Quranic.Verse;
using ScriptiumBackend.Dto.Sealed.Meaning;
using ScriptiumBackend.Utils.Predefined.Model.Islam;


namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Chapter;

public static class Extensions
{
    extension(Model.Derived.Islam.Quranic.Chapter chapter)
    {
        public Complete ToCompleteDto()
        {
            ArgumentNullException.ThrowIfNull(chapter.Meanings);

            return new Complete()
            {
                Name = chapter.Name,
                Sequence = chapter.Sequence,
                Meanings = chapter.Meanings.Select(m => m.ToPlainDto()).ToList(),
            };
        }

        public WithVerses ToChapterWithVersesDto()
        {
            ArgumentNullException.ThrowIfNull(chapter.Verses);
            ArgumentNullException.ThrowIfNull(chapter.Meanings);


            return new WithVerses()
            {
                Name = chapter.Name,
                Sequence = chapter.Sequence,
                Meanings = chapter.Meanings.Select(m => m.ToPlainDto()).ToList(),
                Verses = chapter.Verses.Select(v => v.ToPlainVerseDto()).ToList()
            };
        }

        public UpToQuran ToUpToQuranDto()
        {
            ArgumentNullException.ThrowIfNull(chapter.Meanings);


            return new()
            {
                Meanings = chapter.Meanings.Select(m => m.ToPlainDto()).ToList(),
                Name = chapter.Name,
                Sequence = chapter.Sequence,
                Scripture = new QuranPlain(),
            };
        }
    }
}