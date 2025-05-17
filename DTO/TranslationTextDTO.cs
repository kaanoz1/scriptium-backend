using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class TranslationTextDTO
    {
        public required string Text { get; set; }
        public required TranslationDTO Translation { get; set; }
        public required List<FootNoteDTO> FootNotes { get; set; }

    }

    public class TranslationTextWithVerseUpperConfinedDTO
    {
        public required TranslationTextDTO TranslationText { get; set; }

        public required VerseUpperConfinedDTO Verse { get; set; }
    }

    public class TranslationTextWithVerseUpperMeanDTO
    {
        public required TranslationTextDTO TranslationText { get; set; }

        public required VerseUpperMeanDTO Verse { get; set; }
    }

    public static class TranslationTextExtensions
    {
        public static TranslationTextDTO ToTranslationTextDTO(this TranslationText translationText)
        {
            return new TranslationTextDTO
            {
                Text = translationText.Text,
                Translation = translationText.Translation.ToTranslationDTO(),
                FootNotes = translationText.FootNotes.Select(e => e.ToFootNoteDTO()).ToList(),
            };
        }

        public static TranslationTextWithVerseUpperConfinedDTO ToTranslationTextWithVerseUpperConfinedDTO(this TranslationText translationText)
        {
            return new TranslationTextWithVerseUpperConfinedDTO
            {
                TranslationText = translationText.ToTranslationTextDTO(),
                Verse = translationText.Verse.ToVerseUpperConfinedDTO()
            };
        }

        public static TranslationTextWithVerseUpperMeanDTO ToTranslationTextWithVerseUpperMeanDTO(this TranslationText translationText)
        {
            return new TranslationTextWithVerseUpperMeanDTO
            {
                TranslationText = translationText.ToTranslationTextDTO(),
                Verse = translationText.Verse.ToVerseUpperMeanDTO()
            };
        }
    }
}