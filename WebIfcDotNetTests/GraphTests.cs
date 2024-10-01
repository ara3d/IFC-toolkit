using Ara3D.Logging;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Ara3D.Utils;
using Speckle.Core.Models;
using WebIfcClrWrapper;
using WebIfcDotNet;

namespace WebIfcDotNetTests;
    
public static class GraphTests
{
    [Test]
    public static void TestModelGraph()
    {
        var api = new DotNetApi();
        var logger = new Logger(LogWriter.ConsoleWriter, "");
        var f = "C:\\Users\\cdigg\\git\\web-ifc-dotnet\\src\\engine_web-ifc\\tests\\ifcfiles\\public\\AC20-FZK-Haus.ifc";
        var g = ModelGraph.Load(api, logger, f);
        OutputGraphDetails(g, logger);
    }

    public static void OutputGraphDetails(ModelGraph g, ILogger logger)
    {

        var sinks = g.GetSinks().ToList();
        var sources = g.GetSources().ToList();
        logger.Log($"# of sinks = {sinks.Count}, # of sources = {sources.Count}");
        logger.Log($"Found {g.Model.GetLineIds().Count} lines");
        logger.Log($"Found {g.Nodes.Count} nodes");
        logger.Log($"Found {g.TypeDefinitions.Count} type definitions");
        logger.Log($"Found {g.TypeInstanceToDefinition.Count} type instances");
        logger.Log($"Found {g.Relations.Count} relations");
        logger.Log($"Found {g.GetRelations().OfType<ModelAggregateRelation>().Count()} aggregate relations");
        logger.Log($"Found {g.GetRelations().OfType<ModelSpatialRelation>().Count()} spatial relations");
        logger.Log($"Found {g.GetRelations().OfType<ModelTypeRelation>().Count()} type relations");
        logger.Log($"Found {g.GetRelations().OfType<ModelPropSetRelation>().Count()} property set relations");
        logger.Log($"Found {g.GetProps().Count()} properties");
        logger.Log($"Found {g.GetNodes().OfType<ModelPropSet>().Count()} property sets");

        /*
        // Outputting the node tree. 
        var visited = new HashSet<ModelNode>();
        foreach (var source in sources)
            OutputNode(source, visited);
        */

        //OutputPropSets(g);


        Console.WriteLine("Label types");
        foreach (var t in LabelTypes(g))
        {
             Console.WriteLine($"  {t}");
        }

        Console.WriteLine("Nodes");
        foreach (var n in g.GetSpatialNodes())
        {
            OutputNode(n);
        }
    }

    public static IEnumerable<string> LabelTypes(ModelGraph g)
    {
        return g.GetNodes().OfType<ModelProp>().Select(p => p.Value).OfType<LabelValue>().Select(lv => lv.Type).Distinct().OrderBy(i => i);
    }

    public static void OutputPropSet(ModelPropSet ps, string indent = "")
    {
        Console.WriteLine($"{indent}Property set: {ps.Name} has GUID {ps.Guid} and {ps.Properties.Count} props");
        foreach (var p in ps.Properties)
            OutputProp(p, indent + "  ");
    }

    public static void OutputProp(ModelProp p, string indent = "")
    {
        Console.WriteLine($"{indent}Property: {p.Name} = {p.Value.IfcValToString()}");
    }

    public static void OutputNode(this ModelNode n, string indent = "")
    {
        var hasMesh = n.Graph.Geometries.ContainsKey(n.Id);
        var isContainedIn = n.GetSpatialContainers().FirstOrDefault();
        Console.WriteLine($"{indent}{n}, Has geometry: {hasMesh}, Is type: {n.IsType()}, Has type: {n.GetModelType()}, Is contained in: {isContainedIn}");
        //foreach (var r in n.GetRelationsTo())  Console.WriteLine($"{indent}Node is in a 'to' relation {r.Type}#{r.Id}({r.GetType().Name})");
        //foreach (var r in n.GetRelationsFrom()) Console.WriteLine($"{indent}Node is in a 'from' relation {r.Type}#{r.Id}({r.GetType().Name})");
        foreach (var ps in n.GetPropSets())
            OutputPropSet(ps, indent + "  ");
    }
}