using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.StepParser
{

    public class StepGraphComparator
    {
        public readonly List<IGrouping<string, StepNode>> Groupings;
        public readonly Dictionary<StepNode, int> GroupIds = new();

        public StepGraphComparator(IEnumerable<StepNode> nodes)
        {
            Groupings = nodes.GroupBy(n => n.QuickHash()).ToList();
            for (var i = 0; i < Groupings.Count; ++i)
            {
                foreach (var n in Groupings[i])
                {
                    GroupIds[n] = i;
                }
            }
        }

        public static StepGraphComparator Create(params StepGraph[] graphs)
        {
            return new StepGraphComparator(graphs.SelectMany(g => g.Nodes));
        }
    }

    public class StepGraphDiffer
    {
        public readonly StepGraph A;
        public readonly StepGraph B;

        public StepGraphDiffer(StepGraph a, StepGraph b)
        {
            A = a;
            B = b;
        }

        // TODO: 
        // Are two sub-graphs, definitely different? 
        // Does one sub-graph, have more children? 
        // Does another sub-graph, have fewer children

        // What was added ... and where.
        // What was subtracted ... and where
        // What was changed. 
    }

    public enum DiffType
    {
        Added,
        Deleted,
        Different,
    }

    public class NodeDifference
    {
        public StepNode A;
        public StepNode B;
        public DiffType DiffType;

        public NodeDifference(StepNode a, StepNode b)
        {
            if (a == null && b == null)
                throw new Exception("Both nodes can't be null");
            A = a;
            B = b;
            if (a == null)
                DiffType = DiffType.Added;
            else if (b == null)
                DiffType |= DiffType.Deleted;
            else
            {
                if (a.EntityType != b.EntityType)
                    DiffType = DiffType.Different;
                if (b.Nodes.Count > a.Nodes.Count)
                    DiffType = DiffType.Different;
                if (a.Nodes.Count > b.Nodes.Count)
                    DiffType = DiffType.Different;
                // TODO: look at
            }
        }

        public readonly List<NodeDifference> Children = new();
    }
}