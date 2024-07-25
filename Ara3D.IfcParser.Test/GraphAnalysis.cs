using Ara3D.Utils;

namespace Ara3D.IfcParser.Test;

public class GraphAnalysis
{
    public MultiDictionary<string, string> NodeChildren = new();
    public MultiDictionary<string, string> NodeParent = new();

    public GraphAnalysis(IfcGraph g)
    {
        foreach (var n in g.Nodes)
        {
            var name = n.Entity.EntityType;
            foreach (var child in n.Nodes)
            {
                var childName = child.Entity.EntityType;
                NodeChildren.Add(name, childName);
                NodeParent.Add(childName, name);
            }
        }
    }
}