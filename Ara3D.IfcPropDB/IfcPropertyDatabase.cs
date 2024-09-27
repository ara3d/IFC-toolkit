using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ara3D.Buffers;
using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.NarwhalDB;
using Ara3D.StepParser;
using Ara3D.Utils;
using static Ara3D.NarwhalDB.DB;

namespace Ara3D.IfcPropDB
{

    public class IfcPropertyDatabase
    {
        public IfcPropertyDatabase()
        {
            PropDescTable = Db.AddTable<PropDesc>();
            PropValTable = Db.AddTable<PropVal>();
            PropSetTable = Db.AddTable<PropSet>();
            PropSetToValTable = Db.AddTable<PropSetToVal>();
            PropSetEntityToIndexTable = Db.AddTable<PropSetEntityToIndex>();
        }

        public readonly DB Db = new();

        public readonly Table PropDescTable;
        public readonly Table PropValTable;
        public readonly Table PropSetTable;
        public readonly Table PropSetToValTable;
        public readonly Table PropSetEntityToIndexTable;

        public readonly IndexedSet<PropDesc> DescriptorSet = new();
        public readonly IndexedSet<PropVal> ValueSet = new();
        public readonly IndexedSet<PropSetWithIndices> PropSetSet = new();

        public static readonly Type[] TableTypes = new[]
        {
            typeof(PropDesc),
            typeof(PropVal),
            typeof(PropSet),
            typeof(PropSetToVal),
            typeof(PropSetEntityToIndex),
        };

        /// <summary>
        /// Represents the relationships between property sets and property values. 
        /// </summary>
        public readonly struct PropSetToVal : IBinarySerializable
        {
            public PropSetToVal()
            {
            }

            public readonly int PropSetIndex;
            public readonly int PropValIndex;

            public PropSetToVal(int psi, int pvi)
                => (PropSetIndex, PropValIndex) = (psi, pvi);

            public int Size()
                => 8;

            public object Read(ref nint ptr, IReadOnlyList<string> strings)
                => new PropSetToVal(
                    ReadInt(ref ptr),
                    ReadInt(ref ptr));

            public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
                => new PropSetToVal(
                    ReadInt(bytes, ref offset),
                    ReadInt(bytes, ref offset));

            public int Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
                => WriteInt(bytes, ref offset, PropSetIndex)
                   + WriteInt(bytes, ref offset, PropValIndex);

            public override bool Equals(object? obj)
                => obj is PropSetToVal x
                   && PropSetIndex == x.PropSetIndex
                   && PropValIndex == x.PropValIndex;

            public override int GetHashCode()
                => HashCode.Combine(PropSetIndex, PropValIndex);
        }

        /// <summary>
        /// Represents the relationships between property set entity ids from
        /// the different files and property set rows.
        /// Even if there is lots of repetition in the source file. 
        /// </summary>
        public readonly struct PropSetEntityToIndex : IBinarySerializable
        {
            public PropSetEntityToIndex()
            {
            }

            public readonly int PropSetEntity;
            public readonly int PropSetIndex;
            public readonly string FilePath;

            public PropSetEntityToIndex(int pse, int psi, string filePath)
                => (PropSetEntity, PropSetIndex, FilePath) = (pse, psi, filePath);

            public int Size()
                => 12;

            public object Read(ref nint ptr, IReadOnlyList<string> strings)
                => new PropSetEntityToIndex(
                    ReadInt(ref ptr),
                    ReadInt(ref ptr),
                    ReadString(ref ptr, strings));

            public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
                => new PropSetEntityToIndex(
                    ReadInt(bytes, ref offset),
                    ReadInt(bytes, ref offset),
                    ReadString(bytes, ref offset, strings));

            public int Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
                => WriteInt(bytes, ref offset, PropSetEntity)
                   + WriteInt(bytes, ref offset, PropSetIndex)
                   + WriteString(bytes, ref offset, FilePath, strings);

            public override bool Equals(object? obj)
                => obj is PropSetEntityToIndex x
                   && PropSetEntity == x.PropSetEntity
                   && PropSetIndex == x.PropSetIndex
                   && FilePath == x.FilePath;

            public override int GetHashCode()
                => HashCode.Combine(PropSetEntity, PropSetIndex, FilePath);
        }

        /// <summary>
        /// Represents the reused meta-data found in property values. 
        /// </summary>
        public readonly struct PropDesc : IBinarySerializable
        {
            public PropDesc()
            {
            }

            public readonly string Name;
            public readonly string Description;
            public readonly string Unit;
            public readonly string Entity;

            public PropDesc(string name, string desc, string unit, string entity)
            {
                Name = name ?? "";
                Description = desc ?? "";
                Unit = unit ?? "";
                Entity = entity ?? "";
            }

            public override int GetHashCode()
                => HashCode.Combine(Name, Description, Unit, Entity);

            public override bool Equals(object? obj)
                => obj is PropDesc x
                   && Name == x.Name
                   && Description == x.Description
                   && Unit == x.Unit
                   && Entity == x.Entity;

            public override string ToString()
                => $"{Name}:{Description}:{Unit}:{Entity}";

            public int Size()
                => 16;

            public object Read(ref nint ptr, IReadOnlyList<string> strings)
                => new PropDesc(
                    ReadString(ref ptr, strings),
                    ReadString(ref ptr, strings),
                    ReadString(ref ptr, strings),
                    ReadString(ref ptr, strings));

            public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
                => new PropDesc(
                    ReadString(bytes, ref offset, strings),
                    ReadString(bytes, ref offset, strings),
                    ReadString(bytes, ref offset, strings),
                    ReadString(bytes, ref offset, strings));

            public int Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
                => WriteString(bytes, ref offset, Name, strings)
                   + WriteString(bytes, ref offset, Description, strings)
                   + WriteString(bytes, ref offset, Unit, strings)
                   + WriteString(bytes, ref offset, Entity, strings);
        }

        /// <summary>
        /// A named grouping of property values.  
        /// </summary>
        public readonly struct PropSet : IBinarySerializable
        {
            public PropSet()
            {
            }

            public readonly string Name;
            public readonly string Description;

            public PropSet(string name, string desc)
            {
                Name = name ?? "";
                Description = desc ?? "";
            }

            public override int GetHashCode()
                => HashCode.Combine(Name, Description);

            public override bool Equals(object? obj)
                => obj is PropSet x && Name == x.Name
                                    && Description == x.Description;

            public override string ToString()
                => $"{Name}:{Description}";

            public int Size()
                => 8;

            public object Read(ref nint ptr, IReadOnlyList<string> strings)
                => new PropSet(
                    ReadString(ref ptr, strings),
                    ReadString(ref ptr, strings));

            public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
                => new PropSet(
                    ReadString(bytes, ref offset, strings),
                    ReadString(bytes, ref offset, strings));

            public int Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
                => WriteString(bytes, ref offset, Name, strings)
                   + WriteString(bytes, ref offset, Description, strings);
        }

        /// <summary>
        /// A property set with a list of indices.
        /// Not used in a table, just used to make sure that we aren't
        /// adding prop-sets that are effectively duplicates. 
        /// </summary>
        public class PropSetWithIndices
        {
            public readonly PropSet PropSet;
            public readonly IReadOnlyList<int> OrderedPropVals;

            public PropSetWithIndices(PropSet propSet, IEnumerable<int> propVals)
            {
                PropSet = propSet;
                OrderedPropVals = propVals.OrderBy(x => x).ToList();
            }

            public override int GetHashCode()
                => OrderedPropVals.Aggregate(PropSet.GetHashCode(),
                    HashCode.Combine);

            public override bool Equals(object? obj)
                => obj is PropSetWithIndices x
                   && PropSet.Equals(x.PropSet)
                   && OrderedPropVals.SequenceEqual(x.OrderedPropVals);

            public override string ToString()
                => $"{PropSet}({OrderedPropVals.JoinStringsWithComma()})";
        }

        /// <summary>
        /// Points to a value string and a descriptor. 
        /// </summary>
        public readonly struct PropVal : IBinarySerializable
        {
            public readonly string Value;
            public readonly int DescriptorIndex;

            public PropVal()
            {
            }

            public PropVal(string value, int desc)
            {
                Value = value ?? "";
                DescriptorIndex = desc;
            }

            public override int GetHashCode()
                => HashCode.Combine(Value, DescriptorIndex);

            public override bool Equals(object? obj)
                => obj is PropVal x
                   && x.Value == Value
                   && x.DescriptorIndex == DescriptorIndex;

            public override string ToString()
                => $"{DescriptorIndex}:{Value}";

            public int Size()
                => 8;

            public object Read(ref nint ptr, IReadOnlyList<string> strings)
                => new PropVal(
                    ReadString(ref ptr, strings),
                    ReadInt(ref ptr));

            public object Read(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
                => new PropVal(
                    ReadString(bytes, ref offset, strings),
                    ReadInt(bytes, ref offset));

            public int Write(byte[] bytes, ref int offset, IndexedSet<string> strings)
                => WriteString(bytes, ref offset, Value, strings)
                   + WriteInt(bytes, ref offset, DescriptorIndex);
        }

        //==
        // Implementation of IfcPropertyDatabase

        public IEnumerable<PropVal> PropValues
            => PropValTable.Objects.Cast<PropVal>();

        public IEnumerable<PropDesc> PropDescs
            => PropDescTable.Objects.Cast<PropDesc>();

        public IEnumerable<PropSet> PropSets
            => PropSetTable.Objects.Cast<PropSet>();

        public void AddDocument(StepDocument doc, ILogger logger)
        {
            logger.Log($"Adding document to database");

            var valEntityToIndex = new Dictionary<int, int>();
            var propValEntities = doc.GetInstances("IFCPROPERTYSINGLEVALUE");

            logger.Log("Adding values");
            foreach (var e in propValEntities)
            {
                var pv = new IfcPropertyValue(e.Id, e.AttributeValues);
                var pvIndex = AddValue(pv);
                valEntityToIndex.Add(e.Id, pvIndex);
            }

            logger.Log("Retrieving property sets");
            foreach (var e in doc.GetInstances("IFCPROPERTYSET"))
            {
                var ps = new IfcPropertySet(e.Id, e.AttributeValues);
                var propSet = new PropSet(ps.Name, ps.Description);

                var indices = new List<int>();
                foreach (var p in ps.Properties)
                {
                    // TODO: this might happen if the value is not an "IfcPropertySingleValue
                    // For example it might be a IfcPropertyListValue
                    if (!valEntityToIndex.ContainsKey(p))
                        break;
                    var index = valEntityToIndex[p];
                    indices.Add(index);
                }

                var propSetWithProps = new PropSetWithIndices(propSet, indices);
                if (!PropSetSet.ContainsOrAdd(propSetWithProps, out var psIndex))
                {
                    var tmp = PropSetTable.Add(propSet);
                    Debug.Assert(tmp == psIndex);

                    foreach (var pvIndex in indices)
                    {
                        var psToVal = new PropSetToVal(psIndex, pvIndex);
                        PropSetToValTable.Add(psToVal);
                    }
                }

                var pseToIndex = new PropSetEntityToIndex(e.Id, psIndex, doc.FilePath);
                PropSetEntityToIndexTable.Add(pseToIndex);
            }

            logger.Log($"Document added");
        }

        public int AddValue(IfcPropertyValue val)
        {
            var desc = new PropDesc(val.Name, val.Description, val.Unit, val.Entity);
            if (!DescriptorSet.ContainsOrAdd(desc, out var nDesc))
                PropDescTable.Add(desc);

            var value = new PropVal(val.Value, nDesc);
            if (!ValueSet.ContainsOrAdd(value, out var nValue))
                PropValTable.Add(value);
            return nValue;
        }
    }
}