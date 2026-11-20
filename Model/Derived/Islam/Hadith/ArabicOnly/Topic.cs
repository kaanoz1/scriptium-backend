using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>
/// Node of the subject tree (موضوعات), stored two ways:
/// <list type="bullet">
/// <item><b>Adjacency list</b>: <see cref="Parent"/> / <see cref="Children"/> - one level at a time.</item>
/// <item><b>Nested set</b>: <see cref="Left"/> / <see cref="Right"/>. A depth-first walk numbers every
/// node when entering (Left) and leaving (Right) it, so node B lies under A exactly when
/// <c>A.Left &lt; B.Left</c> and <c>B.Right &lt; A.Right</c>. All ancestors or all descendants of a node
/// come back in one indexed range query, and <c>(Right - Left - 1) / 2</c> is its descendant count.</item>
/// </list>
/// Leaves may point at a meaning group.
/// </summary>
[Table("i_h_ao_topic")]
public class Topic
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id")]
    public required int Id { get; init; }

    [Required, Column("name")] public required string Name { get; init; }

    /// <summary>Null only for the single root node.</summary>
    [ForeignKey(ForeignKeys.ParentTopic)]
    public Topic? Parent { get; init; }

    public List<Topic> Children { get; init; } = [];

    /// <summary>Depth in the tree: 0 = root, 1 = the 13 main subjects, … up to 10.</summary>
    [Column("level")] public required short Level { get; init; }

    /// <summary>Nested-set number given when the depth-first walk enters this node.</summary>
    [Column("lft")] public required int Left { get; init; }

    /// <summary>Nested-set number given when the walk leaves this node (after all its descendants).</summary>
    [Column("rgt")] public required int Right { get; init; }

    /// <summary>Which tree the nested-set numbers belong to (the source has a single tree, 1).</summary>
    [Column("tree_id")] public required int TreeId { get; init; }

    /// <summary>The report a leaf subject points at.</summary>
    [ForeignKey(ForeignKeys.MeaningGroup)]
    public MeaningGroup? MeaningGroup { get; init; }
}
