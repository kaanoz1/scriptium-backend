using System.Security.AccessControl;

namespace ScriptiumBackend.Dto.Islam.Quranic.Root;

using Common = Model.Common;

public static class RootExtensions
{
    extension(Common.Root root)
    {
        public PlainRootDto ToPlainRootDto()
        {
            return new()
            {
                Latin = root.Latin,
                Text = root.Content,
            };
        }
    }
}