using ScriptiumBackend.Models;

namespace DTO
{
    public class TranslationTextDto
    {
        public required string Text { get; set; }
        public required TranslationDto Translation { get; set; }
        public required List<FootNoteDto> FootNotes { get; set; }

    }

    public class TranslationTextWithVerseUpperConfinedDto
    {
        public required TranslationTextDto TranslationText { get; set; }

        public required VerseUpperConfinedDto Verse { get; set; }
    }

    public class TranslationTextWithVerseUpperMeanDto
    {
        public required TranslationTextDto TranslationText { get; set; }

        public required VerseUpperMeanDto Verse { get; set; }
    }

    public static class TranslationTextExtensions
    {
        public static TranslationTextDto ToTranslationTextDto(this TranslationText translationText)
        {
            return new TranslationTextDto
            {
                Text = translationText.Text,
                Translation = translationText.Translation.ToTranslationDto(),
                FootNotes = translationText.FootNotes.Select(e => e.ToFootNoteDto()).ToList(),
            };
        }

        public static TranslationTextWithVerseUpperConfinedDto ToTranslationTextWithVerseUpperConfinedDto(this TranslationText translationText)
        {
            return new TranslationTextWithVerseUpperConfinedDto
            {
                TranslationText = translationText.ToTranslationTextDto(),
                Verse = translationText.Verse.ToVerseUpperConfinedDto()
            };
        }

        public static TranslationTextWithVerseUpperMeanDto ToTranslationTextWithVerseUpperMeanDto(this TranslationText translationText)
        {
            return new TranslationTextWithVerseUpperMeanDto
            {
                TranslationText = translationText.ToTranslationTextDto(),
                Verse = translationText.Verse.ToVerseUpperMeanDto()
            };
        }
    }
}