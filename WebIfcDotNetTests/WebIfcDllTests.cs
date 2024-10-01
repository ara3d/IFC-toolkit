using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.StepParser;
using Ara3D.Utils;
using WebIfcClrWrapper;

namespace WebIfcDotNetTests;

public static class WebIfcDllTests
{
    public static ILogger CreateLogger()
        => new Logger(LogWriter.ConsoleWriter, "");


    public static FilePath inputFile =
        "C:\\Users\\cdigg\\git\\web-ifc-dotnet\\src\\engine_web-ifc\\tests\\ifcfiles\\public\\AC20-FZK-Haus.ifc";

    [Test]
    public static void WebIfcTest()
    {
        var logger = CreateLogger();

        logger.Log($"Opening file {inputFile}");
        var api = new DotNetApi();
        var model = api.Load(inputFile);
        logger.Log($"Id = {model.Id}, Size = {model.Size()}");
        var lines = model.GetLineIds();
        logger.Log($"Found {lines.Count} lines");
        var geometries = model.GetGeometries();
        logger.Log($"Found {geometries.Count} geometries");

        logger.Log($"Initializing DLL API");
        var api2 = WebIfcDll.InitializeApi();
        Assert.IsTrue(api2 != IntPtr.Zero);
        logger.Log($"Retrieving model");
        var modelPtr = WebIfcDll.LoadModel(api2, inputFile);
        Assert.NotNull(modelPtr);

        foreach (var g in geometries)
        {
            var g2 = WebIfcDll.GetGeometry(api2, modelPtr, g.ExpressId);
            Assert.NotNull(g2);
        }

        logger.Log("Finished retrieving all geometries from 2nd API");

        WebIfcDll.FinalizeApi(api2);
    }

    public static IfcGraph LoadIfc(FilePath f)
    {
        var logger = CreateLogger();
        logger.Log($"Opening STEP document {f.GetFileName()}");
        var d = new StepDocument(f);
        logger.Log("Creating graph");
        var g = new IfcGraph(d, logger);
        logger.Log($"# node = {g.Nodes.Count}");
        logger.Log($"# relations = {g.Relations.Count}");
        return g;
    }

    [Test]
    public static void TestMultiThreadedLoader()
    {
        var logger = CreateLogger();
        logger.Log($"Opening file {inputFile.GetFileName()}");
        var api2 = WebIfcDll.InitializeApi();
        IfcGraph g = null;
        IntPtr model2 = IntPtr.Zero;
        Parallel.Invoke(
            () =>
            {
                model2 = WebIfcDll.LoadModel(api2, inputFile);
            },
            () =>
            {
                g = LoadIfc(inputFile);
            });
        logger.Log("Loaded model and graph");

        foreach (var n in g.GetNodes())
        {
            if (n.Type.ToUpperInvariant().Contains("WALL"))
            {
                Console.WriteLine($"Found wall {n.Id}");
                var geo = WebIfcDll.GetGeometry(api2, model2, n.Id);
                var numMeshes = WebIfcDll.GetNumMeshes(api2, geo);
                Console.WriteLine($"Meshes = {numMeshes}");
                for (var i = 0; i < numMeshes; ++i)
                {
                    var mesh = WebIfcDll.GetMesh(api2, geo, i);
                    var numVertices = WebIfcDll.GetNumVertices(api2, mesh);
                    var numIndices = WebIfcDll.GetNumIndices(api2, mesh);
                    Console.WriteLine($"Mesh {i}: # verts = {numVertices}, # indices {numIndices}");
                }
            }
        }

        logger.Log("Enumerated over all nodes");
    }
}
