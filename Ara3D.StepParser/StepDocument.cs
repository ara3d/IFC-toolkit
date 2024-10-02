using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using Ara3D.Buffers;
using Ara3D.Buffers.Modern;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.StepParser
{

    public unsafe class StepDocument : IDisposable
    {
        public readonly FilePath FilePath;
        public readonly byte* DataStart;
        public readonly AlignedMemory Data;

        /// <summary>
        /// This is a list of raw step instance information.
        /// Each one has only a type and an ID.
        /// </summary>
        public readonly List<StepRawInstance> RawInstances = new();

        /// <summary>
        /// This gives us a fast way to look up a StepInstance by their ID
        /// </summary>
        public readonly Dictionary<uint, int> InstanceIdToIndex = new();

        /// <summary>
        /// This tells us the byte offset of the start of each line in the file
        /// </summary>
        public readonly List<int> LineOffsets;

        public StepDocument(FilePath filePath, ILogger logger = null)
        {
            FilePath = filePath;
            logger = logger ?? Logger.Null;

            logger.Log($"Loading {filePath.GetFileSizeAsString()} of data from {filePath.GetFileName()}");
            Data = AlignedMemoryReader.ReadAllBytes(filePath);
            DataStart = Data.BytePtr;

            logger.Log($"Computing the start of each line");
            // NOTE: this estimates that the average line length is more than 16 characters. 
            // This is a reasonable estimate. Only very degenerate files would not meet that. 
            var cap = Data.NumBytes / 16;
            LineOffsets = new List<int>(cap);

            // We are going to report the beginning of the lines, while the "ComputeLines" function
            // will compute the ends of lines.
            var currentLine = 1;
            for (var i = 0; i < Data.NumVectors; i++)
            {
                StepLineParser.ComputeLines(
                    ((Vector256<byte>*)Data.BytePtr)[i], ref currentLine, LineOffsets);
            }

            logger.Log($"Found {LineOffsets.Count} lines");

            logger.Log($"Creating instance records");

            for (var i = 0; i < LineOffsets.Count - 1; i++)
            {
                var lineStart = LineOffsets[i];
                var lineEnd = LineOffsets[i + 1];
                var inst = StepLineParser.ParseLine(DataStart, i, RawInstances.Count, lineStart, lineEnd);
                if (inst.IsValid())
                {
                    RawInstances.Add(inst);
                    InstanceIdToIndex.Add(inst.Id, i);
                }
            }
            logger.Log($"Completed creation of STEP document from {filePath.GetFileName()}");
        }

        public void Dispose() 
            => Data.Dispose();

        public StepRawInstance GetRawInstance(uint id)
            => RawInstances[InstanceIdToIndex[id]];

        public StepInstance GetInstanceWithData(uint id)
            => GetInstanceWithData(GetRawInstance(id));

        public StepInstance GetInstanceWithData(StepRawInstance inst)
        {
            var attr = new StepList(new List<StepValue>());
            var se = new StepEntity(inst.Type, attr);
            return new StepInstance(inst.Id, se);
        }

        public static StepDocument Create(FilePath fp) 
            => new(fp);

        public IEnumerable<StepInstance> GetInstances()
            => RawInstances.Select(GetInstanceWithData);

        public IEnumerable<StepInstance> GetInstances(string typeCode)
            => RawInstances.Where(ri => ri.Type.Equals(typeCode)).Select(GetInstanceWithData);
    }
}