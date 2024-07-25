using Ara3D.Utils;

namespace Ara3D.IfcParser.Test;

public class IfcNode
{
    public readonly IfcGraph Graph;
    public readonly StepEntityWithId Entity;

    public IfcNode(IfcGraph g, StepEntityWithId e)
    {
        Graph = g;
        Entity = e;
    }

    public List<IfcNode> Nodes { get; } = new List<IfcNode>();

    private void AddNodes(StepValue value)
    {
        if (value is StepId id)
        {
            var n = Graph.GetNode(id.Id);
            Nodes.Add(n);
        }
        else if (value is StepAggregate agg)
        {
            foreach (var v in agg.Values)
                AddNodes(v);
        }
    }

    public void Init()
    {
        foreach (var a in Entity.AttributeValues)
            AddNodes(a);
    }

    public override string ToString()
        => Entity.ToString();

    public string ToGraph(HashSet<IfcNode> prev = null)
    {
        prev ??= new HashSet<IfcNode>();
        if (prev.Contains(this))
            return "_";
        var nodeStr = Nodes.Select(n => n.ToGraph(prev)).JoinStringsWithComma();
        return $"{Entity.EntityType}({nodeStr})";
    }
}

public class IfcGraph
{
    public readonly Dictionary<int, IfcNode> Lookup
        = new Dictionary<int, IfcNode>();

    public IfcNode GetNode(int id)
        => Lookup[id];

    public IEnumerable<IfcNode> Nodes
        => Lookup.Values;

    public IfcGraph(StepDocument doc)
    {
        foreach (var e in doc.GetEntities())
        {
            var node = new IfcNode(this, e);
            Lookup.Add(node.Entity.Id, node);
        }

        foreach (var n in Nodes)
            n.Init();
    }
}