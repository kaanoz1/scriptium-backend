using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>
/// Merges chains of narration into one teacher → student graph. Each narrator
/// is placed at the median distance from the top of the chains it occurs in
/// (0 = companion), which is robust against the cycles and skipped links that
/// real-world isnad data contains.
/// </summary>
public static class IsnadGraphBuilder
{
    public const int MaxNodes = 120;

    private sealed class EdgeRow
    {
        public int TeacherId { get; init; }
        public int StudentId { get; init; }
        public int Count { get; init; }
    }

    private sealed class NodeRow
    {
        public int NarratorId { get; init; }
        public int ChainCount { get; init; }
        public bool IsAuthor { get; init; }
        public int Depth { get; init; }
    }

    public static async Task<IsnadGraph> ForGroup(ScriptiumDbContext db, int groupId)
    {
        var edges = await db.Database.SqlQuery<EdgeRow>($"""
            SELECT t.i_h_ao_narrator_id AS "TeacherId", s.i_h_ao_narrator_id AS "StudentId", count(*)::int AS "Count"
            FROM i_h_ao_sanad sa
            JOIN i_h_ao_sanad_narrator s ON s.i_h_ao_sanad_id = sa.id
            JOIN i_h_ao_sanad_narrator t ON t.i_h_ao_sanad_id = s.i_h_ao_sanad_id AND t.position = s.position + 1
            WHERE sa.i_h_ao_meaning_group_id = {groupId} AND t.i_h_ao_narrator_id <> s.i_h_ao_narrator_id
            GROUP BY 1, 2
            """).ToListAsync();

        var nodes = await db.Database.SqlQuery<NodeRow>($"""
            WITH c AS (
                SELECT sn.i_h_ao_sanad_id AS sid, sn.i_h_ao_narrator_id AS nid, sn.position AS pos,
                       max(sn.position) OVER (PARTITION BY sn.i_h_ao_sanad_id) AS top
                FROM i_h_ao_sanad sa
                JOIN i_h_ao_sanad_narrator sn ON sn.i_h_ao_sanad_id = sa.id
                WHERE sa.i_h_ao_meaning_group_id = {groupId}
            )
            SELECT nid AS "NarratorId", count(DISTINCT sid)::int AS "ChainCount", bool_or(pos = 0) AS "IsAuthor",
                   (percentile_disc(0.5) WITHIN GROUP (ORDER BY top - pos))::int AS "Depth"
            FROM c GROUP BY nid
            """).ToListAsync();

        var chains = await db.HAoSanads.InGroup(groupId).CountAsync();
        return await Build(db, edges, nodes, chains);
    }

    public static async Task<IsnadGraph> ForHadith(ScriptiumDbContext db, int hadithId)
    {
        var edges = await db.Database.SqlQuery<EdgeRow>($"""
            SELECT t.i_h_ao_narrator_id AS "TeacherId", s.i_h_ao_narrator_id AS "StudentId", count(*)::int AS "Count"
            FROM i_h_ao_sanad_narrator s
            JOIN i_h_ao_sanad_narrator t ON t.i_h_ao_sanad_id = s.i_h_ao_sanad_id AND t.position = s.position + 1
            WHERE s.i_h_ao_hadith_id = {hadithId} AND t.i_h_ao_narrator_id <> s.i_h_ao_narrator_id
            GROUP BY 1, 2
            """).ToListAsync();

        var nodes = await db.Database.SqlQuery<NodeRow>($"""
            WITH c AS (
                SELECT sn.i_h_ao_sanad_id AS sid, sn.i_h_ao_narrator_id AS nid, sn.position AS pos,
                       max(sn.position) OVER (PARTITION BY sn.i_h_ao_sanad_id) AS top
                FROM i_h_ao_sanad_narrator sn
                WHERE sn.i_h_ao_hadith_id = {hadithId}
            )
            SELECT nid AS "NarratorId", count(DISTINCT sid)::int AS "ChainCount", bool_or(pos = 0) AS "IsAuthor",
                   (percentile_disc(0.5) WITHIN GROUP (ORDER BY top - pos))::int AS "Depth"
            FROM c GROUP BY nid
            """).ToListAsync();

        var chains = await db.HAoSanads.OfHadith(hadithId).CountAsync();
        return await Build(db, edges, nodes, chains);
    }

    private static async Task<IsnadGraph> Build(ScriptiumDbContext db, List<EdgeRow> edges, List<NodeRow> nodes,
        int chains)
    {
        // Drop the rarest branches until the graph is small enough to draw.
        var minCount = 1;
        var kept = edges;
        HashSet<int> keptNodes = NodesOf(kept);
        while (keptNodes.Count > MaxNodes)
        {
            minCount++;
            kept = edges.Where(e => e.Count >= minCount).ToList();
            keptNodes = NodesOf(kept);
        }

        // Single-narrator chains have no edges; still show their narrator.
        if (kept.Count == 0)
            keptNodes = nodes.OrderByDescending(n => n.ChainCount).Take(MaxNodes).Select(n => n.NarratorId)
                .ToHashSet();

        var ids = keptNodes.ToList();
        var narrators = await db.HAoNarrators.AsNoTracking().Where(n => ids.Contains(n.Id)).ToPlain()
            .ToDictionaryAsync(n => n.Id);
        var stats = nodes.ToDictionary(n => n.NarratorId);

        return new IsnadGraph
        {
            Nodes = ids.Where(id => narrators.ContainsKey(id) && stats.ContainsKey(id))
                .Select(id => new IsnadNode
                {
                    Narrator = narrators[id],
                    Depth = stats[id].Depth,
                    ChainCount = stats.TryGetValue(id, out var s) ? s.ChainCount : 0,
                    IsAuthor = stats.TryGetValue(id, out var a) && a.IsAuthor,
                })
                .OrderBy(n => n.Depth).ThenByDescending(n => n.ChainCount)
                .ToList(),
            Edges = kept.Where(e => narrators.ContainsKey(e.TeacherId) && narrators.ContainsKey(e.StudentId))
                .Select(e => new IsnadEdge { TeacherId = e.TeacherId, StudentId = e.StudentId, Count = e.Count })
                .ToList(),
            ChainCount = chains,
            Truncated = kept.Count < edges.Count,
            MinEdgeCount = minCount,
        };
    }

    private static HashSet<int> NodesOf(List<EdgeRow> edges) =>
        edges.SelectMany(e => new[] { e.TeacherId, e.StudentId }).ToHashSet();
}
