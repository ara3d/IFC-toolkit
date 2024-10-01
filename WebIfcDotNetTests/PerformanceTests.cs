using System.Diagnostics;
using Ara3D.Logging;
using Ara3D.Utils;
using WebIfcClrWrapper;
using ModelGraph = WebIfcDotNet.ModelGraph;

namespace WebIfcDotNetTests
{
    public class Measurement
    {
        public readonly Stopwatch Stopwatch = new Stopwatch();
        public readonly List<(string, TimeSpan)> Steps = new List<(string, TimeSpan)>();
        public void Start() => Stopwatch.Start();
        public void Stop() => Stopwatch.Stop();
        public void AddStep(string name) => Steps.Add((name, Stopwatch.Elapsed));
        public TimeSpan Elapsed => Stopwatch.Elapsed;
        public string StepDescription(int n) => Steps[n].Item1;
        public TimeSpan StepCompletion(int n) => n >= 0 ? Steps[n].Item2 : TimeSpan.Zero;
        public TimeSpan StepLength(int n) => StepCompletion(n) - StepCompletion(n - 1);
        public double StepPercentage(int n) => StepLength(n).TotalSeconds / Elapsed.TotalSeconds;
        public int NumSteps => Steps.Count;
    }

    public class RunDetails
    {
        public RunDetails(FilePath filePath) => (FilePath) = (filePath);
        public readonly FilePath FilePath;
        public int RunIndex { get; set; }
        public int ExpressIds { get; set; }
        public string FileName => FilePath.GetFileName();
        public long FileSize => FilePath.GetFileSize();
        public double FileSizeInMb => (double)FileSize / (1024 * 1024);
        public string FileSizeString => $"{FileSizeInMb:00.00}";
        public Exception Exception { get; set; }
        public bool Success => Exception == null;
        public int Geometries { get; set; }
        public int IfcGraphNodes { get; set; }
        public int IfcGraphRelations { get; set; }
        public long StartingMemory { get; set; }
        public long EndingMemory { get; set; }
        public long MemoryConsumption => EndingMemory - StartingMemory;
        public string MemoryConsumptionString => PathUtil.BytesToString(MemoryConsumption);
        public readonly Measurement Measurement = new Measurement();
        public string ElapsedTimeString => Measurement.Elapsed.ToFixedWidthTimeStamp();
    }

    public static class PerformanceTests
    {
        public static RunDetails MeasureFile(FilePath f)
        {
            var api = new DotNetApi();
            var r = new RunDetails(f);
            var m = r.Measurement;
            try
            {
                r.StartingMemory = GC.GetTotalMemory(true);
                r.EndingMemory = r.StartingMemory;

                m.Start();

                var model = api.Load(f);
                r.ExpressIds = model.GetLineIds().Count;
                m.AddStep("Initial load");

                var geometries = model.GetGeometries();
                r.Geometries = geometries.Count;
                m.AddStep("Retrieved geometries");

                var logger = new Logger(LogWriter.ConsoleWriter, "");
                // For when we want detailed logging. 
                //var graph = new ModelGraph(model, logger);
                var graph = new ModelGraph(model);
                m.AddStep("Created graph");

                r.IfcGraphNodes = graph.Nodes.Count;
                r.IfcGraphRelations = graph.Relations.Count;

                m.Stop();
                r.EndingMemory = GC.GetTotalMemory(true);
            }
            catch (Exception e)
            {
                r.Exception = e;
                m.Stop();
            }

            api.DisposeAll();
            return r;
        }

        
        public static void OutputStep(Measurement m, int n)
        {
            Console.WriteLine(
                $"Step {n}: " +
                $"{m.StepCompletion(n).ToFixedWidthTimeStamp()} " +
                $"[{m.StepLength(n).ToFixedWidthTimeStamp()}] " +
                $"({m.StepPercentage(n):0.##%}) " +
                $"- {m.StepDescription(n)}");
        }

        public static void OutputResult(RunDetails r)
        {
            Console.WriteLine(
                $"File: {r.FileName}, " +
                $"Size: {r.FileSizeString}MB, " +
                $"Success: {r.Success}, " + 
                $"Error: {r.Exception?.Message}, " +
                $"Elapsed: {r.ElapsedTimeString}, " +
                $"ExpressIds: {r.ExpressIds}, " +
                $"Geometries: {r.Geometries}, " +
                $"IfcGraphNodes: {r.IfcGraphNodes}, " +
                $"IfcGraphRelations: {r.IfcGraphRelations}, " +
                $"MemoryConsumption: {r.MemoryConsumptionString}, " + 
                $"Run: {r.RunIndex}");
            
            Console.WriteLine("Steps:");
            for (var i=0; i < r.Measurement.NumSteps; i++)
                OutputStep(r.Measurement, i);
        }

        [Test]
        public static void TestPerformance()
        {
            //var inputFiles = MainTests.InputFiles.Take(1).ToList();
            var inputFiles = MainTests.InputFiles.ToList();
            var results = new List<RunDetails>();
            for (var i = 0; i < 1; i++)
            {
                foreach (var f in inputFiles)
                {
                    var result = MeasureFile(f);
                    result.RunIndex = i;
                    results.Add(result);
                    OutputResult(result);
                }
            }
        }
    }
}