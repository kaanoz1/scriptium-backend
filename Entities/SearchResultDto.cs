using DTO;
using ScriptiumBackend.Models;

namespace ScriptiumBackend.Entities;

public class SearchResultDto
{
    public List<TranslationTextWithVerseUpperMeanDto> TranslationTexts { get; set; } = [];
    public List<SectionUpperMeanDto> Sections { get; set; } = [];
}