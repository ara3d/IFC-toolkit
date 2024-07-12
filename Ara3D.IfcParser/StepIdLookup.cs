using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ara3D.IfcParser;

/// <summary>
/// This is a hand-written lookup table for performance
/// </summary>
public class StepInstanceLookup
{
    public readonly StepInstance[] Instances;
    public readonly int Capacity;
    public readonly int[] Lookup;
    
    public StepInstanceLookup(StepInstance[] instances)
    {
        Instances = instances;
        Capacity = instances.Length * 2;
        Lookup = new int[Capacity];
        for (var i = 0; i < Instances.Length; i++)
        {
            var e = Instances[i];
            if (!e.IsValid())
                continue;
            Add(e.Id, i);
            Debug.Assert(Find(e.Id) == i);  
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetFirstIndex(long key)
        => (key * 7) % Capacity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long NextIndex(long index)
        => (index + 1) % Capacity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int entityId, int entityIndex)
    {
        var i = GetFirstIndex(entityId);
        while (IsOccupied(i))
            i = NextIndex(i);
        Lookup[i] = entityIndex + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOccupied(long index)
        => Lookup[index] > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Find(int key)
    {
        var first = GetFirstIndex(key);
        var i = first;
        while (true)
        {
            var r = Lookup[i];
            if (r == 0)
                return -1;
            if (Instances[r - 1].Id == key)
                return r - 1;
            i = NextIndex(i);
            Debug.Assert(i != first);
        }
    }
}