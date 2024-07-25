using System.Diagnostics;
using System.Xml.Linq;
using Ara3D.Logging;
using Ara3D.SimpleDB;
using Ara3D.Utils;

namespace Ara3D.IfcParser;

public class IfcPropertyDatabase 
{
    public IfcPropertyDatabase()
    {
        PropDescTable = Db.AddTable<PropDesc>();
        PropValTable = Db.AddTable<PropVal>();
        PropSetTable = Db.AddTable<PropSet>();
        PropSetToValTable = Db.AddTable<PropSetToVal>();
    }

    public readonly SimpleDatabase Db = new();
    public readonly IndexedSet<PropDesc> DescriptorSet = new();
    public readonly IndexedSet<PropVal> ValueSet = new();
    public readonly Table PropDescTable;
    public readonly Table PropValTable;
    public readonly Table PropSetTable;
    public readonly Table PropSetToValTable;

    public static readonly Type[] TableTypes = new[]
    {
        typeof(PropDesc),
        typeof(PropVal),
        typeof(PropSet),
        typeof(PropSetToVal),
    };

    /// <summary>
    /// Represents the relationships between property sets and property values. 
    /// </summary>
    public class PropSetToVal : ISimpleDatabaseSerializable
    {
        public PropSetToVal() {} 

        public readonly int PropSetIndex;
        public readonly int PropValIndex;

        public PropSetToVal(int psi, int pvi)
            => (PropSetIndex, PropValIndex) = (psi, pvi);

       public int Size()
            => 8;

        public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
            => new PropSetToVal(
                SimpleDatabase.ReadInt(bytes, ref offset),
                SimpleDatabase.ReadInt(bytes, ref offset));

        public void Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
        {
            SimpleDatabase.WriteInt(bytes, ref offset, PropSetIndex);
            SimpleDatabase.WriteInt(bytes, ref offset, PropValIndex);
        }


        public override bool Equals(object? obj)
        {
            var x = obj as PropSetToVal;
            if (x == null) return false;
            return PropSetIndex == x.PropSetIndex
                   && PropValIndex == x.PropValIndex;
        }
    }

    /// <summary>
    /// Represents the reused meta-data found in property values. 
    /// </summary>
    public class PropDesc : ISimpleDatabaseSerializable
    {
        public PropDesc() { }

        public readonly string Name;
        public readonly string Description;
        public readonly string Unit;
        public readonly string File;
        public readonly string Entity;
        
        public PropDesc(string name, string desc, string unit, string file, string entity)
        {
            Name  = name ?? "";
            Description = desc ?? "";
            Unit = unit ?? "";
            File = file ?? "";
            Entity = entity ?? "";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description, Unit, File, Entity);
        }

        public override bool Equals(object? obj)
        {
            var x = obj as PropDesc;
            if (x == null) return false;
            return Name == x.Name
                   && Description == x.Description
                   && Unit == x.Unit
                   && File == x.File
                   && Entity == x.Entity;
        }

        public override string ToString()
            => $"{Name}:{Description}:{Unit}:{Entity}:{File}";

        public int Size()
            => 20;

        public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
        {
            return new PropDesc(
                SimpleDatabase.ReadString(bytes, ref offset, strings),
                SimpleDatabase.ReadString(bytes, ref offset, strings),
                SimpleDatabase.ReadString(bytes, ref offset, strings),
                SimpleDatabase.ReadString(bytes, ref offset, strings),
                SimpleDatabase.ReadString(bytes, ref offset, strings));
        }

        public void Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
        {
            SimpleDatabase.WriteString(bytes, ref offset, Name, strings);
            SimpleDatabase.WriteString(bytes, ref offset, Description, strings);
            SimpleDatabase.WriteString(bytes, ref offset, Unit, strings);
            SimpleDatabase.WriteString(bytes, ref offset, File, strings);
            SimpleDatabase.WriteString(bytes, ref offset, Entity, strings);
        }
    }

    /// <summary>
    /// A named grouping of property values.  
    /// </summary>
    public class PropSet : ISimpleDatabaseSerializable
    {
        public PropSet() { }

        public readonly int EntityId;
        public readonly string Name;
        public readonly string GlobalId;
        public readonly string Description;

        public PropSet(int entityId, string name, string globalId, string desc)
        {
            EntityId = entityId;
            Name = name ?? "";
            GlobalId = globalId; 
            Description = desc ?? "";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EntityId, Name, GlobalId, Description);
        }

        public override bool Equals(object? obj)
        {
            var x = obj as PropSet;
            if (x == null) return false;
            return Name == x.Name
                   && EntityId == x.EntityId
                   && GlobalId == x.GlobalId
                   && Description == x.Description;
        }

        public override string ToString()
        {
            return $"{EntityId}:{Name}:{GlobalId}:{Description}";
        }

        public int Size()
            => 16;

        public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
        {
            return new PropSet(
                SimpleDatabase.ReadInt(bytes, ref offset),
                SimpleDatabase.ReadString(bytes, ref offset, strings),
                SimpleDatabase.ReadString(bytes, ref offset, strings),
                SimpleDatabase.ReadString(bytes, ref offset, strings));
        }

        public void Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
        {
            SimpleDatabase.WriteInt(bytes, ref offset, EntityId);
            SimpleDatabase.WriteString(bytes, ref offset, Name, strings);
            SimpleDatabase.WriteString(bytes, ref offset, GlobalId, strings);
            SimpleDatabase.WriteString(bytes, ref offset, Description, strings);
        }
    }

    /// <summary>
    /// Points to a value string and a descriptor. 
    /// </summary>
    public class PropVal : ISimpleDatabaseSerializable
    {
        public readonly string Value;
        public readonly int DescriptorIndex;

        public PropVal() { }

        public PropVal(string value, int desc)
        {
            Value = value ?? "";
            DescriptorIndex = desc;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, DescriptorIndex);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            var x = (PropVal)obj;
            return x.Value == Value
                   && x.DescriptorIndex == DescriptorIndex;
        }

        public override string ToString()
        {
            return $"{DescriptorIndex}:{Value}";
        }

        public int Size()
            => 8;

        public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
        {
            return new PropVal(
                strings[SimpleDatabase.ReadInt(bytes, ref offset)],
                SimpleDatabase.ReadInt(bytes, ref offset));
        }

        public void Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
        {
            SimpleDatabase.WriteString(bytes, ref offset, Value, strings);
            SimpleDatabase.WriteInt(bytes, ref offset, DescriptorIndex);
        }
    }

    public IEnumerable<PropVal> PropValues 
        => PropValTable.Objects.Cast<PropVal>();
    
    public IEnumerable<PropDesc> PropDescs
        => PropDescTable.Objects.Cast<PropDesc>();

    public IEnumerable<PropSet> PropSets
        => PropSetTable.Objects.Cast<PropSet>();

    public void AddDocument(StepDocument doc, ILogger logger)
    {
        logger.Log($"Adding document to database");

        // Used to map what property ids in this document,
        // map to what index of a property set in the database
        var propIdToPropSetIndex = new MultiDictionary<int, int>();

        logger.Log("Retrieving property sets");
        foreach (var e in doc.GetEntities("IFCPROPERTYSET"))
        {
            var ps = new IfcPropertySet(e.Id, e.AttributeValues);
            var propSet = new PropSet(e.Id, ps.Name, ps.GlobalId, ps.Description);
            //var propSet = new PropSet(e.Id, ps.Name, "", ps.Description);
            var psIndex = PropSetTable.Add(propSet);
            foreach (var p in ps.Properties)
            {
                propIdToPropSetIndex.Add(p, psIndex);
            }
        }

        logger.Log("Adding values");
        foreach (var e in doc.GetEntities("IFCPROPERTYSINGLEVALUE"))
        {
            var pv = new IfcPropertyValue(e.Id, e.AttributeValues);
            if (!propIdToPropSetIndex.TryGetValue(e.Id, out var propSetIndices))
                throw new Exception($"Could not find the property {e.Id} in any property sets.");

            // Add the entries for relations between property sets and property tables. 
            var pvIndex = AddValue(pv, doc.FilePath);
            foreach (var ps in propSetIndices)
            {
                var psToVal = new PropSetToVal(ps, pvIndex);
                PropSetToValTable.Add(psToVal);
            }
        }

        logger.Log($"Document added");
    }

    public int AddValue(IfcPropertyValue val, FilePath filePath)
    {
        var desc = new PropDesc(val.Name, val.Description, val.Unit, val.Entity, filePath);
        var nDesc = DescriptorSet.Add(desc);
        if (nDesc > PropDescTable.Objects.Count)
            throw new Exception($"Unexpected property descriptor index {nDesc}");
        if (nDesc == PropDescTable.Objects.Count)
            PropDescTable.Add(desc);
        Debug.Assert(nDesc < PropDescTable.Objects.Count);

        var value = new PropVal(val.Value, nDesc);
        var nValue = ValueSet.Add(value);
        if (nValue > PropValTable.Objects.Count)
            throw new Exception($"Unexpected property value index {nValue}");
        if (nValue == PropValTable.Objects.Count)
            PropValTable.Add(value);
        return nValue;
    }
}