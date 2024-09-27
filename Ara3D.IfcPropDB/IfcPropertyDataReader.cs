using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Buffers;
using Ara3D.Collections;
using Ara3D.NarwhalDB;

namespace Ara3D.IfcPropDB
{

    public class IfcPropertyDataReader
    {
        public IfcPropertyDataReader(IReadOnlyList<ByteSpanBuffer> buffers)
        {
            LoadTable(out PropSetToValTable, buffers);
            LoadTable(out PropSetEntityToIndexTable, buffers);
            LoadTable(out PropDescTable, buffers);
            LoadTable(out PropSetTable, buffers);
            LoadTable(out PropValTable, buffers);

            var stringsBuffer = buffers.Single(b => b.Name == "_STRINGS_");
            var unpackedStrings = stringsBuffer.Span.UnpackStrings().ToIArray();

            // The conversion to an actual string, happens as late as possible. 
            // However: I eventually want even more usage of "ByteSpan" throughout. 
            Strings = unpackedStrings.Select(bs => bs.ToString());
        }

        public static void LoadTable<T>(out TypedSpan<T> span, IEnumerable<ByteSpanBuffer> buffers) where T : unmanaged
        {
            var name = typeof(T).Name;
            var buffer = buffers.FirstOrDefault(b => b.Name == name);
            if (buffer == null) throw new Exception($"Table {name} could not be found");
            span = new TypedSpan<T>(buffer.Span);
        }

        public TypedSpan<PropSetToVal> PropSetToValTable;
        public TypedSpan<PropSetEntityToIndex> PropSetEntityToIndexTable;
        public TypedSpan<PropDesc> PropDescTable;
        public TypedSpan<PropSet> PropSetTable;
        public TypedSpan<PropVal> PropValTable;
        public IArray<string> Strings;

        public struct PropSetToVal
        {
            public int PropSetIndex;
            public int PropValIndex;

            public PropSet GetPropSet(IArray<PropSet> xs) => xs[PropSetIndex];
            public PropVal GetPropVal(IArray<PropVal> xs) => xs[PropValIndex];
        }

        public struct PropSetEntityToIndex
        {
            public int PropSetEntity;
            public int PropSetIndex;
            public int FilePathStringIndex;

            public PropSet GetPropSet(IArray<PropSet> xs) => xs[PropSetIndex];
            public string GetFilePath(IArray<string> strings) => strings[FilePathStringIndex];
        }

        public struct PropDesc
        {
            public int NameStringIndex;
            public int DescriptionStringIndex;
            public int UnitStringIndex;
            public int EntityStringIndex;

            public string GetName(IArray<string> strings) => strings[NameStringIndex];
            public string GetDescription(IArray<string> strings) => strings[DescriptionStringIndex];
            public string GetUnit(IArray<string> strings) => strings[UnitStringIndex];
            public string GetEntity(IArray<string> strings) => strings[EntityStringIndex];
        }

        public struct PropSet
        {
            public int NameStringIndex;
            public int DescriptionStringIndex;

            public string GetName(IArray<string> strings) => strings[NameStringIndex];
            public string GetDescriptions(IArray<string> strings) => strings[DescriptionStringIndex];
        }

        public struct PropVal
        {
            public int ValueStringIndex;
            public int PropDescIndex;

            public string GetValue(IArray<string> strings) => strings[ValueStringIndex];
            public PropDesc GetDescriptor(IArray<PropDesc> xs) => xs[PropDescIndex];
        }
    }
}