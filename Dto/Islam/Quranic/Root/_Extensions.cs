using System.Security.AccessControl;

namespace ScriptiumBackend.Dto.Islam.Quranic.Root;

using Common = Model.Common;

public static class RootExtensions
{
    extension(Common.Root root)
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
    }
}