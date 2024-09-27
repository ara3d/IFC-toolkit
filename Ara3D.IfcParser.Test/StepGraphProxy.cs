using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.StepParser
{
    public class StepGraphProxy
    {
        public int Count => Children.Count;
        public StepNode Node;
        public readonly IReadOnlyList<StepGraphProxy> Children;
        public string EntityType => Node.Entity.EntityType;

        public StepGraphProxy(StepNode node)
        {
            Node = node;
            Children = Node.Nodes.Select(n => new StepGraphProxy(n)).ToList();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as StepGraphProxy);
        }

        public bool Equals(StepGraphProxy other)
        {
            if (other == null) return false;
            return Count == other.Count
                   && EntityType == other.EntityType
                   && Children.SequenceEqual(other.Children);
        }

        public override int GetHashCode()
        {
            var r = HashCode.Combine(Count, EntityType);
            return Children.Aggregate<StepGraphProxy, int>(r, HashCode.Combine);
        }
    }
}