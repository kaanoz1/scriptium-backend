using ScriptiumBackend.Dto.Derived.Islam.Quranic.Word;

namespace ScriptiumBackend.Dto.Derived.Islam.Quranic.Root;

using Quranic = Model.Derived.Islam.Quranic;

public static class RootExtensions
{
    extension(Quranic.Root root)
    {
        public Plain ToPlainDto()
        {
            return new()
            {
                Latin = root.TextInLatin,
                Text = root.Text,
            };
        }

        public Complete ToCompleteDto()
        {
            return new()
            {
                Latin = root.TextInLatin,
                Text = root.Text,
            };
        }

        public Down ToDownDto()
        {
            return new()
            {
                Latin = root.TextInLatin,
                Text = root.Text,
            };
        }

        public UpToVerse ToUpToVerseDto()
        {
            ArgumentNullException.ThrowIfNull(root.Words);

            return new()
            {
                Latin = root.TextInLatin,
                Text = root.Text,
                Words = root.Words.Select(w => w.ToUpToVerseDto()).ToList(),
            };
        }
    }
}