using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.StepParser;
using Ara3D.Utils;
using WebIfcClrWrapper;

namespace WebIfcDotNetTests
{
    public class LineSummary
    {
        public uint ExpressId;
        public string LineType;
        public bool HasGeometry;
        public int NumMeshes;
    }

    public class FileSummary
    {
        public FilePath File;
        public string FileName => File.GetFileName();
        public string Size => File.GetFileSizeAsString();
        public TimeSpan Duration;
        public List<LineSummary> Lines = new List<LineSummary>();
        public Exception Exception;
        public int TotalGeometries => Lines.Count(l => l.HasGeometry);
        public int TotalMeshes => Lines.Sum(l => l.NumMeshes);
    }

    public static class CompareAPIs
    {
        public static ILogger CreateLogger()
            => new Logger(LogWriter.ConsoleWriter, "");

        public static IfcGraph LoadIfc(FilePath f)
        {
            var d = new StepDocument(f);
            var g = new IfcGraph(d);
            return g;
        }

        public static FileSummary LoadFileClr(FilePath fp)
        {
            var r = new FileSummary() { File = fp };
            try
            {
                var sw = Stopwatch.StartNew();
                var api = new DotNetApi();
                var logger = CreateLogger();
                logger.Log("Using CLR approach");
                logger.Log($"Opening file {fp}");
                var model = api.Load(fp);

                logger.Log("Finished loading file");

                var geos = model.GetGeometries();
                var geoLookup = new Dictionary<uint, Geometry>();
                var nDuplicate = 0;
                foreach (var g in geos)
                {
                    if (geoLookup.ContainsKey(g.ExpressId))
                        nDuplicate++;
                    geoLookup[g.ExpressId] = g;
                }

                logger.Log("Finished extracting geometry");

                foreach (var id in model.GetLineIds())
                {
                    var typeCode = model.GetLineType(id);
                    var lineData = new LineSummary
                    {
                        ExpressId = id,
                        LineType = DotNetApi.GetNameFromTypeCode(typeCode),
                    };
                    r.Lines.Add(lineData);

                    if (geoLookup.ContainsKey(id))
                    {
                        lineData.HasGeometry = true;
                        var g = geoLookup[id];
                        lineData.NumMeshes = g.Meshes.Count;
                    }
                }

                r.Duration = sw.Elapsed;
                logger.Log("Finished extracting all data");
            }
            catch (Exception e)
            {
                r.Exception = e;
            }

            return r;
        }
        
        public static FileSummary LoadFileDll(FilePath fp)
        {
            var r = new FileSummary { File = fp };
            try
            {
                var sw = Stopwatch.StartNew();
                var logger = CreateLogger();
                logger.Log("Using DLL approach");
                logger.Log($"Opening file {fp.GetFileName()}");
                var api2 = WebIfcDll.InitializeApi();
                IfcGraph g = null;
                IntPtr model2 = IntPtr.Zero;
                Parallel.Invoke(
                    () => { model2 = WebIfcDll.LoadModel(api2, fp); },
                    () => { g = LoadIfc(fp); });

                logger.Log("Loaded model and graph");

                foreach (var n in g.GetNodes())
                {
                    var line = new LineSummary
                    {
                        ExpressId = n.Id,
                        LineType = n.Type,
                    };
                    r.Lines.Add(line);

                    var geo = WebIfcDll.GetGeometry(api2, model2, n.Id);
                    if (geo == IntPtr.Zero)
                        continue;
                    line.HasGeometry = true;
                    var numMeshes = WebIfcDll.GetNumMeshes(api2, geo);
                    line.NumMeshes = numMeshes;

                    for (var i = 0; i < numMeshes; ++i)
                    {
                        var mesh = WebIfcDll.GetMesh(api2, geo, i);
                        var numVertices = WebIfcDll.GetNumVertices(api2, mesh);
                        var numIndices = WebIfcDll.GetNumIndices(api2, mesh);
                    }
                }

                logger.Log("Finished extracting all data");
                r.Duration = sw.Elapsed;
            }
            catch (Exception e)
            {
                r.Exception = e;
            }
            return r;
        }

        public static void OutputSummary(FileSummary fs)
        {
            Console.WriteLine($"File: {fs.FileName}, Size: {fs.Size}, Duration: {fs.Duration.TotalSeconds:F2}sec, Lines: {fs.Lines.Count}, Geometries: {fs.TotalGeometries}, Meshes: {fs.TotalMeshes}, Error: {fs.Exception?.Message ?? "SUCCESS"}");

            /*
            foreach (var l in fs.Lines)
            {
                Console.WriteLine($"  {l.ExpressId} {l.LineType} {l.NumArgs} {l.HasGeometry} {l.NumMeshes}");
            }*/
        }

        public const long MaxSize = 100 * 1024 * 1024;

        public static IEnumerable<FilePath> InputFiles()
            => MainTests.InputFiles.Where(fp => fp.GetFileSize() < MaxSize);

        public static IEnumerable<FilePath> BigInputFiles()
            => MainTests.InputFiles.Where(fp => fp.GetFileSize() > MaxSize);

        [Test]
        [TestCaseSource(nameof(BigInputFiles))]
        public static void LoadBigFiles(FilePath f)
        {
            Compare(f);
            /*
            var logger = CreateLogger();
            var d = new StepDocument(f);
            logger.Log("Created document");
            var g = new IfcGraph(d);
            logger.Log("Created graph");
            */
            /*
            var d2 = LoadFileDll(f);
            Console.WriteLine("Using DLL");
            OutputSummary(d2);
            */
        }

        [Test]
        [TestCaseSource(nameof(InputFiles))]
        public static void RunComparison(FilePath f)
        {
            Compare(f);
        }

        public static void Compare(FilePath f)
        {
            var d1 = LoadFileClr(f);
            var d2 = LoadFileDll(f);
            Console.WriteLine("Using CLR");
            OutputSummary(d1);
            Console.WriteLine("Using DLL");
            OutputSummary(d2);
        }
    }
}
