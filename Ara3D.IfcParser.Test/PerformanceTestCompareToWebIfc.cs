using System.Diagnostics;
using Ara3D.Logging;
using Ara3D.StepParser;
using Ara3D.Utils;
using NUnit.Framework;

namespace Ara3D.IfcParser.Test
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
                var r = new RunDetails(f);
                var m = r.Measurement;
                try
                {
                    r.StartingMemory = GC.GetTotalMemory(true);
                    r.EndingMemory = r.StartingMemory;

                    m.Start();
                    var d = new StepDocument(f);
                    r.ExpressIds = d.RawInstances.Count;
                    m.AddStep("Initial load");

                    var geometries = 0;
                    // NOTE: not supported currently 
                    m.AddStep("Retrieved geometries");

                    var logger = new Logger(LogWriter.ConsoleWriter, "");
                    var g = new IfcGraph(d, null);
                    m.AddStep("Created graph");

                    r.IfcGraphNodes = g.Nodes.Count;
                    r.IfcGraphRelations = g.Relations.Count;

                    m.Stop();
                    r.EndingMemory = GC.GetTotalMemory(true);
                }
                catch (Exception e)
                {
                    r.Exception = e;
                    m.Stop();
                }

                return r;
            }

            public static void OutputStep(Measurement m, int n)
            {
                /*
                Console.WriteLine(
                    $"Step {n}: " +
                    $"{m.StepCompletion(n).ToFixedWidthTimeStamp()} " +
                    $"[{m.StepLength(n).ToFixedWidthTimeStamp()}] " +
                    $"({m.StepPercentage(n):0.##%}) " +
                    $"- {m.StepDescription(n)}");
                */
                Console.Write($", {m.StepDescription(n)}, {m.StepLength(n).ToFixedWidthTimeStamp()}");
            }

            public static void OutputResult(RunDetails r)
            {
                /*
                Console.Write(
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
                */

                Console.Write(
                    $"{r.FileName}, " +
                    $"{r.FileSizeString}MB, " +
                    $"{r.Success}, " +
                    $"{r.Exception?.Message}, " +
                    $"{r.ElapsedTimeString}, " +
                    $"{r.ExpressIds}, " +
                    $"{r.Geometries}, " +
                    $"{r.IfcGraphNodes}, " +
                    $"{r.IfcGraphRelations}, " +
                    $"{r.MemoryConsumptionString}, " +
                    $"{r.RunIndex}");

                for (var i = 0; i < r.Measurement.NumSteps; i++)
                    OutputStep(r.Measurement, i);

                Console.WriteLine();
        }

            public static IEnumerable<FilePath> InputFiles
                => new DirectoryPath(@"C:\Users\cdigg\dev\speckle\private-test-files").GetFiles();

            [Test]
            public static void TestPerformance()
            {
                //var inputFiles = InputFiles.Skip(3).Take(1).ToList();
                var inputFiles = InputFiles.ToList();
                var results = new List<RunDetails>();
                Console.WriteLine($"File Name, File size, Success, Error, Elapsed time, # Ids, # Geometries, # Graph nodes, # Relations, Memory consumption, Run index, Step 1, Step 1 Timing, Step 2, Step 2 Timing, Step 3, Step 3 Timing");
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
