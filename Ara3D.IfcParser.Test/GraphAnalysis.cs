using Ara3D.Utils;

namespace Ara3D.IfcParser.Test;

/// <summary>
/// Used to create a parent / child list of nodes. 
/// </summary>
public class GraphAnalysis
{
    public Dictionary<string, HashSet<string>> EntityChildren = new();
    public List<StepNode> Nodes = new();

    public void AddGraph(StepGraph g)
    {
        foreach (var n in g.Nodes)
        {
            Nodes.Add(n);
            var name = n.Entity.EntityType;
            if (!EntityChildren.ContainsKey(name))
                EntityChildren.Add(name, new HashSet<string>());
            var children = EntityChildren[name];
            foreach (var child in n.Nodes)
            {
                var childName = child.Entity.EntityType;
                children.Add(childName);
            }
        }
    }
}