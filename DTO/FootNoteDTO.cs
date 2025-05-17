using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class FootNoteDTO
    {
        public required short Index { get; set; }
        public required string Text { get; set; }
    }

    public static class FootNoteSimpleExtensions
    {
        public static FootNoteDTO ToFootNoteDTO(this FootNote footNote)
        {
            return new FootNoteDTO
            {
                Index = footNote.Index,
                Text = footNote.FootNoteText.Text
            };
        }
    }
}