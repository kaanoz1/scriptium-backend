using ScriptiumBackend.Dto.Islam.Quranic.Verse;

namespace ScriptiumBackend.Dto.Islam.Quranic.Chapter;

using Model.Islam.Quranic;

public static class Extensions
{
    extension(Chapter chapter)
    {
        public WithVerses ToChapterWithVersesDto()
        {
            ArgumentNullException.ThrowIfNull(chapter.ChapterC);
            ArgumentNullException.ThrowIfNull(chapter.Verses);
            ArgumentNullException.ThrowIfNull(chapter.Meanings);

            if (chapter.Verses.First() is { } verse)
            {
                ArgumentNullException.ThrowIfNull(verse);
                ArgumentNullException.ThrowIfNull(verse.VerseC);
            }
            
            return new WithVerses()
            {
                Name = chapter.ChapterC.Name,
                Sequence = chapter.Sequence,
                Meanings = chapter.Meanings,
                Verses = chapter.Verses.Select(v => v.ToPlainVerseDto()).ToList()
            };
        }
    }
}