namespace ScriptiumBackend.Dto.Shared.Language;

public static class Extensions
{
    extension(Model.Shared.Language language)
    {
        public Plain ToPlainDto()
        {
            return new()
            {
                Name = language.Name,
                NameEnglish = language.EnglishName,
                Code = language.Code,
            };
        }
        
    }
}