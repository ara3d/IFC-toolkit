using Ara3D.StepParser;
using System;
using System.Collections.Generic;

namespace Ara3D.IfcParser
{
    public class IfcPropertyValue
    {
        public readonly int EntityId;
        public readonly string Name;
        public readonly string Description;
        public readonly string Value;
        public readonly string Unit;
        public readonly string Entity;

        public IfcPropertyValue(int entityId, IReadOnlyList<StepValue> vals)
        {
            if (vals.Count != 4)
                throw new Exception("Expected 4 values");
            EntityId = entityId;

            Name = vals[0].AsString();
            Description = vals[1].AsString();
            var inst = vals[2];
            if (inst is StepEntity se)
            {
                Entity = se.EntityType.ToString();
                Value = se.Attributes.ToString();
            }
            else
            {
                Entity = "UNKNOWN";
            }

            Unit = vals[3].ToString();
        }

        public override string ToString()
            => $"#{EntityId}=IFCPROPERTYSINGLEVALUE('{Name}', {Description}, {Value}, {Unit})";
    }
}