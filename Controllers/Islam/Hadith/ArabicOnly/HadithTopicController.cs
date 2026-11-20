using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>The subject tree (موضوعات الحديث).</summary>
[ApiController, Route(HadithQueries.Route)]
public class HadithTopicController(
    ScriptiumDbContext db,
    ILogger<HadithTopicController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    /// <summary>Top-level subjects (children of the tree's single root).</summary>
    [HttpGet("topics")]
    public Task<IActionResult> Root() => Cached<TopicComplete>(async () =>
    {
        var roots = await db.HAoTopics.AsNoTracking().Roots().Select(t => t.Id).ToListAsync();
        return new TopicComplete { Topic = null, Path = [], Children = await Children(db, roots) };
    });

    [HttpGet("topic/{id:int}")]
    public Task<IActionResult> Get([FromRoute, Range(1, int.MaxValue)] int id) => Cached<TopicComplete>(async () =>
    {
        var topic = await db.HAoTopics.AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new { t.Id, t.Name, t.TreeId, t.Left, t.Right, t.Level, MeaningGroupId = EF.Property<int?>(t, ForeignKeys.MeaningGroup) })
            .FirstOrDefaultAsync();
        if (topic is null) return null;

        MeaningGroupPlain? group = null;
        if (topic.MeaningGroupId is { } gid)
            group = await db.HAoMeaningGroups.AsNoTracking().Where(g => g.Id == gid).ToPlain().FirstOrDefaultAsync();

        return new TopicComplete
        {
            Topic = new TopicPlain { Id = topic.Id, Name = topic.Name },
            Path = await db.TopicPath(topic.TreeId, topic.Left, topic.Right, inclusive: false),
            Children = await Children(db, [topic.Id]),
            Group = group,
        };
    });

    internal static Task<List<TopicChild>> Children(ScriptiumDbContext db, List<int> parentIds) =>
        db.HAoTopics.AsNoTracking()
            .ChildrenOf(parentIds)
            .OrderBy(t => t.Left)
            .Select(t => new TopicChild
            {
                Id = t.Id,
                Name = t.Name,
                ChildCount = t.Children.Count,
                DescendantCount = (t.Right - t.Left - 1) / 2,
                Group = t.MeaningGroup == null
                    ? null
                    : new MeaningGroupPlain
                    {
                        Id = t.MeaningGroup.Id,
                        Grade = t.MeaningGroup.Grade,
                        IsQudsi = t.MeaningGroup.IsQudsi,
                        Text = t.MeaningGroup.Text,
                        HadithCount = t.MeaningGroup.HadithCount,
                        CompanionCount = t.MeaningGroup.CompanionCount,
                    },
            })
            .ToListAsync();
}
