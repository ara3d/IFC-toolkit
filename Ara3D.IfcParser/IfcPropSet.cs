using System.Collections.Generic;
using System.Diagnostics;
using Ara3D.StepParser;

namespace Ara3D.IfcParser
{
    // https://standards.buildingsmart.org/IFC/RELEASE/IFC2x3/TC1/HTML/ifckernel/lexical/ifcpropertyset.htm
    public class IfcPropSet : IfcNode
    {
        public readonly StepList PropertyIdList;

        public IfcPropSet(IfcGraph graph, StepInstance lineData, StepList propertyIdList)
            : base(graph, lineData)
        {
            Debug.Assert(IsIfcRoot);
            PropertyIdList = propertyIdList;
        }

        public IEnumerable<IfcProp> GetProperties()
        {
            for (var i = 0; i < NumProperties; ++i)
            {
                var id = PropertyId(i);
                var node = Graph.GetNode(id);
                if (node is not IfcProp prop)
                    throw new System.Exception($"Expected a property not {node} from id {id}");
                yield return prop;
            }
        }

        public int NumProperties => PropertyIdList.Values.Count;
        public uint PropertyId(int i) => PropertyIdList.Values[i].AsId();
    }
}