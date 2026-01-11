namespace ScriptiumBackend.Dto.Sealed.Language;

public static class Extensions
{
    extension(Model.Sealed.Language language)
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