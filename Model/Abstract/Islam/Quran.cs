using ScriptiumBackend.Model.Common;
using ScriptiumBackend.Model.Shared;

namespace ScriptiumBackend.Model.Abstract.Islam;

public class Quran : Scripture
{
    public Quran(List<Model.Islam.Quranic.Chapter> chapters) => Chapters.AddRange(chapters);

    public override char Code { get; init; } = 'Q';

    public override required string Name { get; init; } = "القرآن الكريم";

    public List<Model.Islam.Quranic.Chapter> Chapters { get; init; } = [];
    
}