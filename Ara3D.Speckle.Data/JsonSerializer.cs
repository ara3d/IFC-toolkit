using System.Collections;
using System.IO;
using System.Linq;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;

namespace Ara3D.Speckle.Data
{
    public static class JsonSerializer
    {
        public static JsonWriter WriteProperty(this JsonWriter jw, object o, string fieldName)
        {
            jw.WritePropertyName(fieldName);
            var pi = o.GetType().GetProperty(fieldName);
            jw.SafeWriteValue(pi.GetValue(o));
            return jw;
        }

        public static JsonWriter WriteDictionary(this JsonWriter jw, IDictionary d)
        {
            jw.WriteStartObject();
            foreach (var k in d.Keys.Cast<string>().OrderBy(s => s))
            {
                jw.WritePropertyName(k);
                var v = d[k];
                jw.SafeWriteValue(v);
            }
            jw.WriteEndObject();
            return jw;
        }

        public static JsonWriter WriteEnumerable(this JsonWriter jw, IEnumerable es)
        {
            if (es is string s)
            {
                jw.WriteValue(s);
                return jw;
            }
            if (es is IDictionary d)
                return jw.WriteDictionary(d);
            jw.WriteStartArray();
            foreach (var e in es)
                jw.SafeWriteValue(e);
            jw.WriteEndArray();
            return jw;
        }

        public static JsonWriter SafeWriteValue(this JsonWriter jw, object o)
        {
            if (o == null)
                jw.WriteNull();
            else if (o is SpeckleObject so)
                jw.WriteSpeckleObject(so);
            else if (o is string s)
                jw.WriteValue(s);
            else if (o is double d)
                jw.WriteValue(d);
            else if (o is float f)
                jw.WriteValue((double)f);
            else if (o is int i)
                jw.WriteValue(i);
            else if (o is uint ui)
                jw.WriteValue(ui);
            else if (o is long l)
                jw.WriteValue(l);
            else if (o is ulong ul)
                jw.WriteValue(ul);
            else if (o is bool b)
                jw.WriteValue(b);
            else if (o is IDictionary dict)
                jw.WriteDictionary(dict);
            else if (o is IEnumerable es)
                jw.WriteEnumerable(es); 
            else if (o is Base speckleBase)
                jw.WriteValue($"Speckle${speckleBase.id}");
            else
                jw.WriteValue(o.ToString());
            return jw;
        }

        public static string ToJson(this SpeckleObject speckleObject)
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            jw.Formatting = Formatting.Indented;
            jw.Indentation = 2;
            jw.WriteSpeckleObject(speckleObject);
            return sw.ToString();
        }

        public static JsonWriter WriteSpeckleObject(this JsonWriter jw, SpeckleObject speckleObject)
        {
            if (speckleObject == null)
            {
                jw.WriteNull();
                return jw;
            }

            if (speckleObject.IsSimpleList)
                return jw.WriteEnumerable(speckleObject.Elements);

            if (speckleObject.IsSimpleDictionary)
                return jw.WriteDictionary(speckleObject.Properties);

            jw.WriteStartObject();
            jw.WriteProperty(speckleObject, nameof(speckleObject.Id));
            jw.WriteProperty(speckleObject, nameof(speckleObject.SpeckleType));
            jw.WriteProperty(speckleObject, nameof(speckleObject.DotNetType));
            jw.WriteProperty(speckleObject, nameof(speckleObject.Elements));
            if (speckleObject.SpeckleType != "Objects.Geometry.Mesh")
                jw.WriteProperty(speckleObject, nameof(speckleObject.Properties));
            jw.WriteEndObject();

            return jw;
        }
    }
}