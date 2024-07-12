using System.Diagnostics;
using System.Runtime.CompilerServices;
using Debug = System.Diagnostics.Debug;

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
    public long GetFirstIndex(long instanceId)
    {
        Debug.Assert(instanceId > 0);
        Debug.Assert(instanceId * 7 < int.MaxValue);
        return (instanceId * 7) % Capacity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long NextIndex(long index)
        => (index + 1) % Capacity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int entityId, int instanceIndex)
    {
        Debug.Assert(instanceIndex > 0);
        Debug.Assert(instanceIndex < Instances.Length);
        var i = GetFirstIndex(entityId);
        while (IsOccupied(i))
            i = NextIndex(i);
        Lookup[i] = instanceIndex + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOccupied(long index)
        => Lookup[index] > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Find(int key)
    {
        var first = GetFirstIndex(key);
        var i = first;
#if DEBUG
        var scanCnt = 1;
#endif
        while (true)
        {
            var r = Lookup[i];
            if (r == 0)
                return -1;
            if (Instances[r - 1].Id == key)
                return r - 1;
#if DEBUG
            if (scanCnt++ % 10 == 0)
                Debug.WriteLine($"Scanned {scanCnt} times to find an unoccupied place");
#endif 
            i = NextIndex(i);
            Debug.Assert(i != first);
        }
    }
}