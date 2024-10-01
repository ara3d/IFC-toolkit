using Ara3D.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Ara3D.StepParser;
using Ara3D.Utils;

namespace Ara3D.IfcParser
{
    /// <summary>
    /// This is a high-level representation of an IFC model as a graph of nodes and relations.
    /// It also contains the  properties, and property sets. 
    /// </summary>
    public class IfcGraph
    {
        public static IfcGraph Load(FilePath fp, ILogger logger = null)
            => new IfcGraph(new StepDocument(fp, logger), logger);

        public StepDocument Document { get; }

        public Dictionary<uint, IfcNode> Nodes { get; } = new Dictionary<uint, IfcNode>();
        public Dictionary<uint, IfcRelation> Relations { get; } = new Dictionary<uint, IfcRelation>();

        public IReadOnlyList<uint> SourceIds { get; }
        public IReadOnlyList<uint> SinkIds { get; }

        public IfcNode AddNode(IfcNode n)
            => Nodes[n.Id] = n;

        public IfcRelation AddRelation(IfcRelation r)
        {
            Relations.Add(r.Id, r);
            return r;
        }

        public IfcGraph(StepDocument d, ILogger logger = null)
        {
            Document = d;

            logger?.Log("Computing entities");
            foreach (var inst in Document.RawInstances)
            {
                if (!inst.IsValid())
                    continue;
                
                // Property Values
                if (inst.Type.Equals("IFCPROPERTYSINGLEVALUE"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddNode(new IfcProp(this, e, (StepString)e[0], e[2]));
                }
                else if (inst.Type.Equals("IFCPROPERTYENUMERATEDVALUE"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddNode(new IfcProp(this, e, (StepString)e[0], e[2]));
                }
                else if (inst.Type.Equals("IFCPROPERTYREFERENCEVALUE"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddNode(new IfcProp(this, e, (StepString)e[0], e[3]));
                }
                else if (inst.Type.Equals("IFCPROPERTYLISTVALUE"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddNode(new IfcProp(this, e, (StepString)e[0], e[2]));
                }
                else if (inst.Type.Equals("IFCCOMPLEXPROPERTY"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddNode(new IfcProp(this, e, (StepString)e[0], e[3]));
                }
                
                // Property Set 
                else if (inst.Type.Equals("IFCPROPERTYSET"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddNode(new IfcPropSet(this, e, (StepString)e[0], (StepString)e[2], (StepList)e[4]));
                }

                // Aggregate relation
                else if (inst.Type.Equals("IFCRELAGGREGATES"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddRelation(new IfcAggregateRelation(this, e, (StepId)e[4], (StepList)e[5]));
                }

                // Spatial relation
                else if (inst.Type.Equals("IFCRELCONTAINEDINSPATIALSTRUCTURE"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddRelation(new IfcSpatialRelation(this, e, (StepId)e[5], (StepList)e[4]));
                }

                // Property set relations
                else if (inst.Type.Equals("IFCRELDEFINESBYPROPERTIES"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddRelation(new IfcPropSetRelation(this, e, (StepId)e[5], (StepList)e[4]));
                }

                // Type relations
                else if (inst.Type.Equals("IFCRELDEFINESBYTYPE"))
                {
                    var e = d.GetInstanceWithData(inst);
                    AddRelation(new IfcTypeRelation(this, e, (StepId)e[5], (StepList)e[4]));
                }

                // Everything else 
                else
                {
                    // Simple IFC node: without step entity data.
                    var e = d.GetInstanceWithData(inst);
                    AddNode(new IfcNode(this, e));
                }
            }

            logger?.Log("Retrieving the root of all of the spatial relationship");
            SourceIds = GetSpatialRelations()
                .Where(r => r.From != null)
                .Select(r => (uint)r.From.Id)
                .Distinct().ToList();
            logger?.Log("Completed creating model graph");
        }

        public IEnumerable<IfcNode> GetNodes()
            => Nodes.Values;

        public IfcNode GetOrCreateNode(StepInstance lineData, int arg)
        {
            if (arg < 0 || arg >= lineData.AttributeValues.Count)
                throw new Exception("Argument index out of range");
            return GetOrCreateNode(lineData.AttributeValues[arg]);
        }

        public IfcNode GetOrCreateNode(StepValue o)
            => GetOrCreateNode(o is StepId id 
                ? (uint)id.Id
                : throw new Exception($"Expected a StepId value, not {o}"));

        public IfcNode GetOrCreateNode(uint id)
        {
            var r = Nodes.TryGetValue(id, out var node)
                ? node
                : AddNode(new IfcNode(this, GetStepEntity(id)));
            Debug.Assert(r.Id == id);
            return r;
        }

        public List<IfcNode> GetOrCreateNodes(List<StepValue> list)
            => list.Select(GetOrCreateNode).ToList();

        public List<IfcNode> GetOrCreateNodes(StepInstance line, int arg)
        {
            if (arg < 0 || arg >= line.AttributeValues.Count)
                throw new Exception("Argument out of range");
            if (!(line.AttributeValues[arg] is StepList agg))
                throw new Exception("Expected a list");
            return GetOrCreateNodes(agg.Values);
        }

        public IfcNode GetNode(uint id)
        {
            var r = Nodes[id];
            Debug.Assert(r.Id == id);
            return r;
        }

        public IEnumerable<IfcRelation> GetRelations()
            => Relations.Values;

        public IEnumerable<IfcNode> GetSources()
            => SourceIds.Select(GetNode);

        public IEnumerable<IfcNode> GetSinks()
            => SinkIds.Select(GetNode);

        public IEnumerable<IfcPropSet> GetPropSets()
            => GetNodes().OfType<IfcPropSet>();

        public IEnumerable<IfcProp> GetProps()
            => GetNodes().OfType<IfcProp>();

        public IEnumerable<IfcSpatialRelation> GetSpatialRelations()
            => GetRelations().OfType<IfcSpatialRelation>();

        public IEnumerable<IfcAggregateRelation> GetAggregateRelations()
            => GetRelations().OfType<IfcAggregateRelation>();
    }

    public class IfcPart
    {
        public StepInstance LineData { get; }
        public IfcGraph Graph { get; }
        public uint Id => (uint)LineData.Id;
        public string Type => LineData?.EntityType ?? "";

        public IfcPart(IfcGraph graph, StepInstance lineData)
        {
            Graph = graph;
            LineData = lineData;
        }

        public override bool Equals(object obj)
        {
            if (obj is IfcPart other)
                return Id == other.Id;
            return false;
        }

        public override int GetHashCode()
            => (int)Id;

        public override string ToString()
            => $"{Type}#{Id}";
    }

    /// <summary>
    /// Always express a 1-to-many relation
    /// </summary>
    public class IfcRelation : IfcPart
    {
        public StepId From { get; }
        public StepList To { get; }

        public IfcRelation(IfcGraph graph, StepInstance lineData, StepId from, StepList to)
            : base(graph, lineData)
        {
            From = from;
            To = to;
        }
    }

    public class IfcPropSetRelation : IfcRelation
    {
        public IfcPropSetRelation(IfcGraph graph, StepInstance lineData, StepId from, StepList to)
            : base(graph, lineData, from, to)
        {
        }
    }

    public class IfcSpatialRelation : IfcRelation
    {
        public IfcSpatialRelation(IfcGraph graph, StepInstance lineData, StepId from, StepList to)
            : base(graph, lineData, from, to)
        {
        }
    }

    public class IfcAggregateRelation : IfcRelation
    {
        public IfcAggregateRelation(IfcGraph graph, StepInstance lineData, StepId from, StepList to)
            : base(graph, lineData, from, to)
        {
        }
    }

    public class IfcTypeRelation : IfcRelation
    {
        public IfcTypeRelation(IfcGraph graph, StepInstance lineData, StepId from, StepList to)
            : base(graph, lineData, from, to)
        {
        }
    }

    public class IfcProp : IfcNode
    {
        public readonly StepValue Name;
        public readonly StepValue Value;

        public IfcProp(IfcGraph graph, StepInstance lineData, StepString name, StepValue value)
            : base(graph, lineData)
        {
            Name = name;
            Value = value;
        }
    }

    public class IfcPropSet : IfcNode
    {
        public readonly StepString Guid;
        public readonly StepString Name;
        public readonly StepList Properties;

        public IfcPropSet(IfcGraph graph, StepInstance lineData, StepString guid, StepString name, StepList properties)
            : base(graph, lineData)
        {
            Guid = guid;
            Name = name;
            Properties = properties;
        }
    }

    public class IfcNode : IfcPart
    {
        public IfcNode(IfcGraph graph, StepInstance lineData)
            : base(graph, lineData)
        { }
    }
}
