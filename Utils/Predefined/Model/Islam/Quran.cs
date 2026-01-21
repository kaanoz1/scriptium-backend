using System.Collections.Generic;

namespace ScriptiumBackend.Utils.Predefined.Model.Islam;

using Model = ScriptiumBackend.Model;

public sealed class Quran(
    List<Model.Derived.Islam.Quranic.Chapter> chapters,
    List<Model.Derived.Islam.Quranic.Translation> translations) : QuranPlain
{
    public List<Model.Derived.Islam.Quranic.Chapter> Chapters { get; init; } = chapters;

    public List<Model.Derived.Islam.Quranic.Translation> Translations { get; init; } = translations;
}