using ScriptiumBackend.Models;

namespace DTO
{
    public class FootNoteDto
    {
        public required short Index { get; set; }
        public required string Text { get; set; }
    }

    public static class FootNoteSimpleExtensions
    {
        public static FootNoteDto ToFootNoteDto(this FootNote footNote)
        {
            return new FootNoteDto
            {
                Index = footNote.Index,
                Text = footNote.FootNoteText.Text
            };
        }
    }
}