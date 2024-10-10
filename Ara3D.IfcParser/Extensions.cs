using Ara3D.Buffers;
using Ara3D.StepParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ara3D.IfcParser
{
    public static class Extensions
    {
        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
        {
            if (!self.ContainsKey(key))
                self[key] = new List<TValue>();
            self[key].Add(value);
        }

        public static uint AsId(this StepValue value)
            => value is StepUnassigned
                ? 0u
                : ((StepId)value).Id;

        public static string AsString(this StepValue value)
            => value is StepString ss ? ss.AsString() :
                value is StepNumber sn ? sn.Value.ToString() :
                value is StepId si ? si.Id.ToString() :
                value is StepSymbol ssm ? ssm.Name.ToString() :
                "";

        public static double AsNumber(this StepValue value)
            => value is StepUnassigned
                ? 0
                : ((StepNumber)value).Value;

        public static List<StepValue> AsList(this StepValue value)
            => value is StepUnassigned
                ? new List<StepValue>()
                : ((StepList)value).Values;

        public static List<uint> AsIdList(this StepValue value)
            => value is StepUnassigned
                ? new List<uint>()
                : value.AsList().Select(AsId).ToList();

        // Uses Latin1 encoding (aka ISO-8859-1)
        // Extended characters are escaped with a backslash and are converted 
        public static string AsString(this ByteSpan span)
            => Encoding.Latin1.GetString(span.ToSpan()).IfcToUnicode();

        public static readonly Regex IfcRegex
            = new Regex(@"\\X(\d+)\\([0-9A-Fa-f]+)\\X\d+\\", RegexOptions.Compiled);

        public static string IfcToUnicode(this string self)
            => IfcRegex.Replace(self, match =>
            {
                // Extract byte count and hex values (e.g., \X2\00F600DF\X0\)
                var byteCountStr = match.Groups[1].Value;
                var hexValue = match.Groups[2].Value;

                // Parse the byte count
                var byteCount = int.Parse(byteCountStr);

                // Split the hex string into chunks of two characters (each representing one byte)
                var result = new StringBuilder();
                for (var i = 0; i < hexValue.Length; i += 4)
                {
                    // Get the next 4 characters (representing 2 bytes, or 1 Unicode character)
                    var hexChunk = hexValue.Substring(i, 4);

                    // Convert the hex chunk to a Unicode character
                    var unicodeCodePoint = Convert.ToInt32(hexChunk, 16);
                    result.Append(char.ConvertFromUtf32(unicodeCodePoint));
                }

                return result.ToString();
            });

        public static string AsString(this StepString ss)
            => ss.Value.AsString();

        public static object ToJsonObject(this StepValue sv)
        {
            switch (sv)
            {
                case StepEntity stepEntity:
                {
                    var attr = stepEntity.Attributes;
                    if (attr.Values.Count == 0)
                        return stepEntity.ToString();

                    if (attr.Values.Count == 1)
                        return attr.Values[0].ToJsonObject();
                    
                    return attr.Values.Select(ToJsonObject).ToList();
                }

                case StepId stepId:
                    return stepId.Id;

                case StepList stepList:
                    return stepList.Values.Select(ToJsonObject).ToList();

                case StepNumber stepNumber:
                    return stepNumber.AsNumber();

                case StepRedeclared stepRedeclared:
                    return null;
                
                case StepString stepString:
                    return stepString.AsString();
                
                case StepSymbol stepSymbol:
                    var tmp = stepSymbol.Name.AsString();
                    if (tmp == "T")
                        return true;
                    if (tmp == "F")
                        return false;
                    return tmp;
                
                case StepUnassigned stepUnassigned:
                    return null;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sv));
            }
        }
    }
}
