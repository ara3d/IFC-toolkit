using System.Collections.Generic;

namespace Ara3D.Speckle.Data
{
    /// <summary>
    /// This is generated from a 'Base' object, which is a base class for all objects in the Speckle API.
    /// It is easier to navigate and convert to/from other formats. 
    /// </summary>
    public class SpeckleObject
    {
        public string Id { get; set; }
        public string SpeckleType { get; set; }
        public string DotNetType { get; set; }

        public List<object> Elements { get; set; } = new List<object>();
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        public bool IsSimpleDictionary => Properties.Count > 0 && Elements.Count == 0 && DotNetType == "Dictionary`2";
        public bool IsSimpleList => Properties.Count == 0 && Elements.Count > 0 && DotNetType == "List`1";

        /* TODO: all of this would be nice to know in the future. 
        public Transform Transform { get; set; }
        public SpeckleObject InstanceDefinition { get; set; }
        public string InstanceDefinitionId => InstanceDefinition?.Id ?? "";
        public List<SpeckleObject> Instances { get; set; } = new List<SpeckleObject>();
        public Point BasePoint { get; set; }
        public bool IsInstanced { get; set; }
        */
    }
}