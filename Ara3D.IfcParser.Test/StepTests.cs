using Ara3D.Buffers;
using Ara3D.Logging;
using Ara3D.StepParser;
using Ara3D.Utils;
using GeometryGym.Ifc;
using IFC;
using NUnit.Framework;
using STEP;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using IfcDoor = GeometryGym.Ifc.IfcDoor;

namespace Ara3D.IfcParser.Test;

public static class StepTests
{
    public static IReadOnlyList<FilePath> Files => Config.Files;

    public static IEnumerable<StepRawInstance> GetInstances(StepDocument doc, ByteSpan type) =>
        doc.RawInstances.Where(inst
            => inst.Type.Equals(type));

    public static IEnumerable<StepRawInstance> GetInstances(StepDocument doc, string type) =>
        type.WithSpan(span =>
            doc.RawInstances.Where(inst =>
                inst.Type.Equals(span)));

    public static void Ara3DListDoors(StepDocument doc, ILogger logger)
    {
        var doors = GetInstances(doc, "IFCDOOR").ToList();
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
        var totalSize = PathUtil.BytesToString(Files.Sum(f => f.GetFileSize()));
        Console.WriteLine($"Loading {Files.Count} files of total size {totalSize}");
        var actions = Files
            .Select<FilePath, Action>(f => () => XBimLoadIfc(f, Logger.Null))
            .ToArray();
        TimingUtils.TimeIt(() => { Parallel.Invoke(actions); });
    }

    [Test]
    public static void GeometryGymTimingsMultiFiles()
    {
        var totalSize = PathUtil
            .BytesToString(Files.Sum(f => f.GetFileSize()));
        Console.WriteLine($"Loading {Files.Count} files of total size {totalSize}");
        var actions = Files
            .Select<FilePath, Action>(f 
                => () => GeometryGymLoadIfc(f))
            .ToArray();
        TimingUtils.TimeIt(() => { Parallel.Invoke(actions); });
    }

    [Test]
    public static void Ara3DTimingsMultiFiles()
    {
        var totalSize = PathUtil.BytesToString(Files.Sum(f => f.GetFileSize()));
        Console.WriteLine($"Loading {Files.Count} files of total size {totalSize}");
        var actions = Files
            .Select<FilePath, Action>(f => () => Ara3DLoadIfc(f, Logger.Null))
            .ToArray();
        TimingUtils.TimeIt(() => { Parallel.Invoke(actions); });
    }

    [Test]
    [TestCaseSource(nameof(Files))]
    public static void Timings(FilePath fp)
    {
        Console.WriteLine($"Testing {fp.GetFileName()} which is {fp.GetFileSizeAsString()}");
        TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        TimingUtils.TimeIt(() => HyparLoadIfc(fp), "Hypar");
        TimingUtils.TimeIt(() => GeometryGymLoadIfc(fp), "GeometryGym");
        TimingUtils.TimeIt(() => XBimLoadIfc(fp), "XBim");
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