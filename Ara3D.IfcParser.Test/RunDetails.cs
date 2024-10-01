using System.Diagnostics;
using Ara3D.Utils;

namespace Ara3D.IfcParser.Test
{
    public class RunDetails
    {
        public RunDetails(FilePath filePath)
        {
            FilePath = filePath;
            StartingMemory = GC.GetTotalMemory(true);
            EndingMemory = StartingMemory;
        }

        public void Stop()
        {
            EndingMemory = GC.GetTotalMemory(true);
            Stopwatch.Stop();
        }

        public Stopwatch Stopwatch = Stopwatch.StartNew();
        public readonly FilePath FilePath;
        public int RunIndex { get; set; }
        public int ExpressIds { get; set; }
        public string FileName => FilePath.GetFileName();
        public long FileSize => FilePath.GetFileSize();
        public double FileSizeInMb => (double)FileSize / (1024 * 1024);
        public string FileSizeString => $"{FileSizeInMb:00.00}";
        public Exception? Exception { get; set; }
        public bool Success => Exception == null;
        public int Geometries { get; set; }
        public int IfcGraphNodes { get; set; }
        public int IfcGraphRelations { get; set; }
        public long StartingMemory { get; set; }
        public long EndingMemory { get; set; }
        public long MemoryConsumption => EndingMemory - StartingMemory;
        public string MemoryConsumptionString => PathUtil.BytesToString(MemoryConsumption);
        public TimeSpan Elapsed => Stopwatch.Elapsed; 
        public string ElapsedTimeString => Elapsed.ToFixedWidthTimeStamp();

        public string RowData()
            => $"{FileName}, " +
                $"{FileSizeString}MB, " +
                $"{Success}, " +
                $"{Exception?.Message}, " +
                $"{ElapsedTimeString}, " +
                $"{ExpressIds}, " +
                $"{Geometries}, " +
                $"{IfcGraphNodes}, " +
                $"{IfcGraphRelations}, " +
                $"{MemoryConsumptionString}, " +
                $"{RunIndex}";
        
        public string Header()
            => "File Name, " +
               "File size, " +
               "Success, " +
               "Error, " +
               "Elapsed time, " +
               "# Ids, " +
               "# Geometries, " +
               "# Graph nodes, " +
               "# Relations, " +
               "Memory consumption, " +
               "Run index";

        public static RunDetails LoadGraph(FilePath fp)
        {
            var r = new RunDetails(fp);
            var loader = new IfcLoader.IfcLoader(fp);
            r.Stop();
            r.IfcGraphNodes = loader.Graph.Nodes.Count;
            r.IfcGraphRelations = loader.Graph.Relations.Count;
            return r;
        }
    }
}
