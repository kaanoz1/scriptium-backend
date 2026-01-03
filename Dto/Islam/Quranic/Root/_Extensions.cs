using System.Security.AccessControl;
using ScriptiumBackend.Dto.Islam.Quranic.Word;

namespace ScriptiumBackend.Dto.Islam.Quranic.Root;

using Common = Model.Common;

public static class RootExtensions
{
    extension(Model.Islam.Quranic.Root root)
    {
        public Plain ToPlainDto()
        {
            return new()
            {
                Latin = root.Latin,
                Text = root.Content,
            };
        }

        public Complete ToCompleteDto()
        {
            return new()
            {
                Latin = root.Latin,
                Text = root.Content,
            };
        }

        public Down ToDownDto()
        {
            return new()
            {
                Latin = root.Latin,
                Text = root.Content,
            };
        }

        public UpToVerse ToUpToVerseDto()
        {
            return new()
            {
                Latin = root.Latin,
                Text = root.Content,
                Words = root.Words.Select(w => w.ToUpToVerseDto()).ToList(),
            };
        }
    }
}