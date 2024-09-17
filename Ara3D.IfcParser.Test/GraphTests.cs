using Ara3D.IfcParser.Test;
using Ara3D.Logging;
using NUnit.Framework;

namespace Ara3D.IfcParser.Test;

public static class GraphTests
{
    [Test]
    public static void GeometryEntityAnalysis()
    {
        var logger = Logger.Console;
        logger.Log("Starting");
        var files = StepTests.LargeFiles().Take(5);
        var docs = files.Select(StepDocument.Create).ToList();
        
        logger.Log("Created documents");
        var graphs = docs.Select(StepGraph.Create).ToList();
        
        logger.Log("Created graphs");
        var ga = new GraphAnalysis();
        foreach (var g in graphs)
            ga.AddGraph(g);

        logger.Log($"Creating graph analysis");
        //var geoEntities = IfcEntityCodes.GeometricEntities.ToHashSet();
        var meshEntities = IfcEntityCodes.MeshEntities.ToHashSet();
        foreach (var line in GraphCodes(ga, meshEntities))
        {
            Console.WriteLine(line);
        }
    }

    [Test]
    public static void Groups()
    {
        var f = StepTests.LargeFiles().First();
        var logger = Logger.Console;
        var d = new StepDocument(f, logger);
        logger.Log("Created document");
        var g = new StepGraph(d);
        logger.Log("Created graph");
        var groups = g.Nodes.GroupBy(n => g.ToValString(n, 2)).ToList();
        logger.Log($"Created groups");
        var dict = groups.ToDictionary(g => g.Key, g => g.ToList());
        logger.Log($"Created dictionary");

        //var keyValues = dict.OrderByDescending(kv => kv.Value.Count).Take(100).First(kv => kv.Key.Contains('#'));
        var keyValues = dict
            .OrderByDescending(kv => kv.Value.Count)
            .Where(kv => !kv.Key.Contains('#') && kv.Value.Count > 1);

        foreach (var kv in keyValues)
            Console.WriteLine($"{kv.Value.Count}\t{kv.Key}");
    }

    public static IEnumerable<string> GraphCodes(GraphAnalysis ga, HashSet<string> entities)
    {
        foreach (var kv in ga.EntityChildren)
            foreach (var c in kv.Value)
                if (entities.Contains(kv.Key) || entities.Contains(c))
                    yield return $"{kv.Key}-->{c}";
    }
}