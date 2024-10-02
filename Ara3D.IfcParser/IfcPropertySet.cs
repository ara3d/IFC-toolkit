using Ara3D.StepParser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Ara3D.IfcParser
{
    public class IfcPropertySet
    {
        public readonly uint EntityId;
        public readonly string GlobalId;
        public readonly uint OwnerHistoryId;
        public readonly string Name;
        public readonly string Description;
        public readonly List<uint> Properties;

        public IfcPropertySet(uint entityId, IReadOnlyList<StepValue> vals)
        {
            if (vals.Count != 5)
                throw new Exception("Expected 5 values");
            EntityId = entityId;
            GlobalId = vals[0].AsString();
            OwnerHistoryId = vals[1].AsId();
            Name = vals[2].AsString();
            Description = vals[3].AsString();
            Properties = vals[4].AsIdList();
        }

        public override string ToString()
        {
            var propIds = string.Join(", ", Properties.Select(p => $"#{p}"));
            return $"#{EntityId}=IFCPROPERTYSET({GlobalId}, #{OwnerHistoryId}, {Name}, {Description}, ({propIds}))";
        }
    }
}