using Ara3D.Logging;
using Ara3D.Spans;
using Ara3D.Utils;
using GeometryGym.Ifc;
using IFC;
using NUnit.Framework;
using STEP;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using IfcDoor = GeometryGym.Ifc.IfcDoor;

namespace Ara3D.IfcParser.Test;

public static class ProfilingTests
{
    public static IEnumerable<FilePath> VeryLargeFiles() 
        => TestFiles.VeryLargeFiles();

    public static IEnumerable<FilePath> LargeFiles()
        => TestFiles.LargeFiles();

    public static IEnumerable<FilePath> MediumFiles()
        => TestFiles.MediumFiles();

    public static IEnumerable<FilePath> HugeFiles()
        => TestFiles.HugeFiles();

    public static void VisitRecords(StepDocument doc, ILogger logger)
    {
        var i = 0;
        var recs = doc.GetInstances().AsParallel();
        foreach (var rec in recs)
        {
            Interlocked.Increment(ref i);
        }
        logger.Log($"Visited {i} records");
    }

    public static void ListEntityTypes(StepDocument doc, ILogger logger)
    {
        logger.Log("Gathering distinct entity types");
        var recs = doc.GetInstances().Select(r => r.Type).Distinct().ToList();
        logger.Log($"Completed, found {recs.Count} distinct records");
        var i = 0;
        foreach (var r in recs)
        {
            Console.WriteLine($"{i++}: {r} [{r.GetHashCode()}]");
        }
    }

    private static ByteSpan IFCDOOR = "IFCDOOR".ToByteSpanPinned();

    public static void Ara3DListDoors(StepDocument doc, ILogger logger)
    {
        var doors = doc.GetInstances(IFCDOOR).ToList();
        Console.WriteLine($"Found {doors.Count} doors in {doc.FilePath}");
    }

    public static void XBimListDoors(IfcStore store, ILogger logger)
    {
        var doors = store.Instances.OfType<IIfcDoor>().ToList();
        Console.WriteLine($"Found {doors.Count} doors in {store.FileName}");
    }

    public static void GeometryGymListDoors(DatabaseIfc db, ILogger logger)
    {
        var doors = db.OfType<IfcDoor>().ToList();
        Console.WriteLine($"Found {doors.Count} doors in {db.FileName}");
    }

    [Test]
    public static void XBimTimingsMultiFiles()
    {
        var files = MediumFiles();
        var totalSize = PathUtil.BytesToString(files.Sum(f => f.GetFileSize()));
        var count = files.Count();
        Console.WriteLine($"Loading {count} files of total size {totalSize}");
        var actions = files
            .Select<FilePath, Action>(f => () => XBimLoadIfc(f, Logger.Null))
            .ToArray();
        TimingUtils.TimeIt(() => { Parallel.Invoke(actions); });
    }

    [Test]
    public static void GeometryGymTimingsMultiFiles()
    {
        var files = MediumFiles();
        var totalSize = PathUtil.BytesToString(files.Sum(f => f.GetFileSize()));
        var count = files.Count();
        Console.WriteLine($"Loading {count} files of total size {totalSize}");
        var actions = files
            .Select<FilePath, Action>(f => () => GeometryGymLoadIfc(f))
            .ToArray();
        TimingUtils.TimeIt(() => { Parallel.Invoke(actions); });
    }

    [Test]
    public static void Ara3DSkiHillCountDoors()
    {
        var f = @"C:/Users/cdigg/dev/impraria/07 - NEOM Mountain/4200000004 - Ski Village/STAGE 3A 100%/IFC/MEC/07-004003-4200000004-AED-MEC-MDL-000001_IFC_D.ifc";
        TimingUtils.TimeIt(() => Ara3DLoadIfc(f));
    }

    [Test]
    public static void Ara3DTimingsMultiFiles()
    {
        var files = MediumFiles();
        var totalSize = PathUtil.BytesToString(files.Sum(f => f.GetFileSize()));
        var count = files.Count();
        Console.WriteLine($"Loading {count} files of total size {totalSize}");
        var actions = files
            .Select<FilePath, Action>(f => () => Ara3DLoadIfc(f, Logger.Null))
            .ToArray();
        TimingUtils.TimeIt(() => { Parallel.Invoke(actions); });
    }

    [Test]
    [TestCaseSource(nameof(VeryLargeFiles))]
    public static void Timings(FilePath fp)
    {
        Console.WriteLine($"Testing {fp.GetFileName()} which is {fp.GetFileSizeAsString()}");
        TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        //TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        //TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        //TimingUtils.TimeIt(() => HyparLoadIfc(fp), "Hypar");
        //TimingUtils.TimeIt(() => GeometryGymLoadIfc(fp), "GeometryGym");
        //TimingUtils.TimeIt(() => XBimLoadIfc(fp), "XBim");
    }

    [Test]
    public static void TestSerialize()
    {
        var fp = LargeFiles().First();
        var doc = new StepDocument(fp);
        var bis = new BinaryIfcSerializer();
        var r = bis.Serialize(doc);
        Console.WriteLine($"Text = {fp.GetFileSizeAsString()} and Binary = {PathUtil.BytesToString(r.Count)}");
        doc.Dispose();
    }


    [Test]
    public static void IdentifierCounts()
    {
        var fp = LargeFiles().First();
        var doc = new StepDocument(fp);
        var d = doc.GetInstances().GroupBy(r => r.Type).ToDictionary(g => g.Key, g => g.Count());
        foreach (var kv in d.OrderByDescending(pair => pair.Value))
        {
            Console.WriteLine($"Identifier = {kv.Key} count = {kv.Value}");
        }
        doc.Dispose();
    }

    public static void Ara3DLoadIfc(FilePath filePath, ILogger logger = null)
    {
        logger ??= new Logger(LogWriter.ConsoleWriter, "Ara 3D Step Document Loader");
        var doc = new StepDocument(filePath, logger);
        //VisitRecords(doc, logger);
        //ListEntityTypes(doc, logger);
        Ara3DListDoors(doc, logger);
        logger.Log("Disposing");
        doc.Dispose();
    }

    public static void HyparLoadIfc(FilePath filePath)
    {
        var ifcModel = new Document(filePath, out List<STEPError> errors);
    }

    public static void GeometryGymLoadIfc(FilePath filePath)
    {
        var database = new DatabaseIfc(filePath);
        database.FileName = filePath;
        GeometryGymListDoors(database, Logger.Console);
        database.Dispose();
    }

    public static void XBimLoadIfc(FilePath filePath, ILogger logger = null)
    {
        var editor = new XbimEditorCredentials
        {
            ApplicationDevelopersName = "Ara 3D Inc ",
            ApplicationFullName = "IfcParsingTest",
            ApplicationIdentifier = "Ara3D.IfcParsingTest",
            ApplicationVersion = "1.0",
            EditorsFamilyName = "Diggins",
            EditorsGivenName = "Christopher",
            EditorsOrganisationName = "Ara 3D Inc."
        };

        // Bizarre loading logic requires file size. 
        var sizeLimit = 20.0 * 1000 * 1000 * 1000;
        logger ??= new Logger(LogWriter.ConsoleWriter, "XBim Document Loader");
        
        logger.Log("Opening file");
        var model = IfcStore.Open(filePath, editor, sizeLimit);
        
        logger.Log("Opened");
        XBimListDoors(model, logger);
        
        logger.Log("Disposing");
        model.Dispose();
    }
}