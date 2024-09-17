using Ara3D.Utils;

namespace Ara3D.IfcParser.Test;

/// <summary>
/// Evaluates the count, total size, and average size
/// of each entity type.  
/// </summary>
public class EntitySizeAnalysis
{
    public readonly MultiDictionary<string, int> EntitySizes = new();
    
    public EntitySizeAnalysis(StepDocument doc = null)
        => AddDocument(doc);

    public void AddDocument(StepDocument doc)
    {
        if (doc == null)
            return;
        foreach (var e in doc.GetEntities())
        {
            var size = doc.GetLineSpan(e.LineIndex).Length;
            EntitySizes.Add(e.EntityType, size);
        }
    }

    public Dictionary<string, Statistics> GetStats()
        => EntitySizes.ToDictionary(
            kv => kv.Key, 
            kv => new Statistics(kv.Value.Select(n => (double)n), false, false));

    public static EntitySizeAnalysis Create(IEnumerable<StepDocument> docs)
    {
        var r = new EntitySizeAnalysis();
        foreach (var doc in docs) 
            r.AddDocument(doc);
        return r;
    }
}