using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Ara3D.Logging;
using Ara3D.Spans;

namespace Ara3D.IfcParser.Test
{
    public static class SimdTests
    {
        [Test]
        public static void TestCountDelimiters()
        {
            var f =
                @"C:/Users/cdigg/dev/impraria/07 - NEOM Mountain/4200000004 - Ski Village/STAGE 3A 100%/IFC/MEC/07-004003-4200000004-AED-MEC-MDL-000001_IFC_D.ifc";

            var logger = Logger.Console;
            var mem = SimdReader.ReadAllBytes(f);
            logger.Log($"Found {mem.Data.Length} vectors in {mem.Length} bytes");
            //var cnt = CountDelimiters(mem);
            //logger.Log($"Counted {cnt} delimiters");
            var lines = NewLineIndices(mem);
            logger.Log($"Counted {lines.Count} lines");
        }

        public static int CountDelimiters(SimdMemory sm)
        {
            var r = 0;
            foreach (var v in sm.Data)
            {
                var mask = SimdIfcTokenizer.DelimiterMask(v, out var bits);
                r += bits;
            }
            return r;
        }

        public static unsafe FixedList<int> NOTFASTER_NewLineIndices(SimdMemory sm)
        {
            var cap = sm.Length / 16;
            var data = new int[cap];
            fixed (int* p = &data[0])
            {
                var lines = new FixedList<int>(p, cap);
                var currentLine = 0;
                foreach (var v in sm.Data)
                {
                    if (lines.Count + 32 >= lines.Capacity)
                        throw new Exception("Too many lines");
                    //SimdIfcTokenizer.ComputeLines(v, ref currentLine, lines);
                }

                return lines;
            }
        }

        public static List<int> NewLineIndices(SimdMemory sm)
        {
            var cap = sm.Length / 16;
            var lines = new List<int>(cap);
            var currentLine = 0;
            foreach (var v in sm.Data)
            {
                SimdIfcTokenizer.ComputeLines(v, ref currentLine, lines);
            }

            return lines;
        }
    }


    public static unsafe class SimdExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfNonZero(Vector256<byte>* pBegin, int numVectors, byte b)
        {
            var pattern = Vector256.Create(b);
            for (var i = 0; i < numVectors; ++i)
            {
                var r = pBegin->IndexOfNonZeroByte(pattern);
                if (r >= 0)
                    return r;
            }

            return -1;
        }

        // TODO:
        // Find index of '=', ';', '\n'

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(this Vector256<byte> self, byte b)
        {
            return self.IndexOfNonZeroByte(Vector256.Create(b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfNonZeroByte(this Vector256<byte> self, in Vector256<byte> repeated)
        {
            var tmp = Avx2.CompareEqual(self, repeated);
            var index = Avx2.MoveMask(tmp);
            if (index == 0) return -1;
            return BitOperations.TrailingZeroCount(index);
        }
    }

    public class SimdIfcLineParser
    {
        // Step 1: count the number of lines.
        // Step 2: allocate integers, to indicate the beginning of each line.
        // Step 3: parse a line (from one line to the next) to see if it contains an instance 

        // NOTE: for faster parsing a line should contain the index.
        // Note: we are not parsing that many integers. 
    }

}