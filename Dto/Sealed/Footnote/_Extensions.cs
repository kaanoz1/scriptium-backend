namespace ScriptiumBackend.Dto.Sealed.Footnote;

public static class Extensions
{
    extension(Model.Sealed.Footnote footnote)
    {
        public Plain ToPlainDto()
        {
            return new()
            {
                Index = footnote.Index,
                Indicator = footnote.Indicator,
                Text = footnote.Text,
            };
        }
    }
}