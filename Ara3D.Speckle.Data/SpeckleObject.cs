using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Objects.Geometry;
using Objects.Other;
using Point = Objects.Geometry.Point;

namespace Ara3D.Speckle.Data
{
    /// <summary>
    /// This is generated from a 'Base' object, which is a base class for all objects in the Speckle API.
    /// It is easier to navigate and convert to/from other formats. 
    /// </summary>
    public class SpeckleObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public List<object> Elements { get; set; } = new List<object>();
        public IEnumerable<SpeckleObject> Children => Elements.Concat(Properties.Values).OfType<SpeckleObject>();
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        public bool IsSimpleDictionary => Properties.Count > 0 && Elements.Count == 0 && DotNetType == "Dictionary`2";
        public bool IsSimpleList => Properties.Count == 0 && Elements.Count > 0 && DotNetType == "List`1";

        public Mesh Mesh { get; set; }
        public RenderMaterial Material { get; set; }
        
        public Transform Transform { get; set; }
        public SpeckleObject InstanceDefinition { get; set; }
        public string InstanceDefinitionId => InstanceDefinition?.Id ?? "";
        public List<SpeckleObject> Instances { get; set; } = new List<SpeckleObject>();
        public bool IsInstanced { get; set; }
        public Point BasePoint { get; set; }
    }
}