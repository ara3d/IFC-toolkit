using Ara3D.IfcParser.Test;
using Ara3D.Logging;
using NUnit.Framework;

namespace Ara3D.IfcParser.Test;

public static class GraphTests
{
    [Test]
    public static void GraphTest()
    {
        var f = StepTests.LargeFiles().First();
        var logger = Logger.Console;
        var d = new StepDocument(f, logger);
        var g = new IfcGraph(d);
        logger.Log("Created graph");
        
        foreach (var n in g.Nodes.Take(20))
        {
            //Console.WriteLine(n);
        }

        foreach (var n in g
                     .Nodes
                     .Where(n => n.Entity.IsEntityType("IFCSHAPEREPRESENTATION"))
                     .Take(5))
        {
            //Console.WriteLine(n.ToGraph());
        }

        var ga = new GraphAnalysis(g);

        Console.WriteLine("# Property analysis");
        foreach (var p in IfcEntityCodes.PropertyEntities)
            Write(ga, p);
    }

    public static void Write(GraphAnalysis ga, string name)
    {
        Console.WriteLine($"## Analysis of {name}");

        {
            if (ga.NodeChildren.TryGetValue(name, out var values))
            {
                var distinct = values.Distinct().ToList();
                Console.WriteLine($"{distinct.Count} distinct children");
                foreach (var v in distinct)
                {
                    Console.WriteLine($"  {v}");
                }
            }
            else
            {
                Console.WriteLine($"No children");
            }
        }
        {
            if (ga.NodeParent.TryGetValue(name, out var values))
            {
                var distinct = values.Distinct().ToList();
                Console.WriteLine($"{distinct.Count} distinct parents");
                foreach (var v in distinct)
                {
                    Console.WriteLine($"  {v}");
                }
            }
            else
            {
                Console.WriteLine($"No parents");
            }
        }
    }
}