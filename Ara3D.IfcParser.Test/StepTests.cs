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

    public static void Ara3DListDoors(StepDocument doc, ILogger logger)
    {
        
        var doors = "IFCDOOR".WithSpan(doc.GetInstances).ToList();
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
        var totalSize = PathUtil
            .BytesToString(files.Sum(f => f.GetFileSize()));
        var count = files.Count();
        Console.WriteLine($"Loading {count} files of total size {totalSize}");
        var actions = files
            .Select<FilePath, Action>(f 
                => () => GeometryGymLoadIfc(f))
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
    [TestCaseSource(nameof(LargeFiles))]
    public static void CountPropValues(FilePath fp)
    {
        Console.WriteLine(fp);
        Console.WriteLine();
        var logger = Logger.Console;
        using var doc = new StepDocument(fp, logger);

        var propNames = new HashSet<string>();
        var propDescriptions = new HashSet<string>();
        var propUnits = new HashSet<string>();
        var propValues = new HashSet<string>();
        var nProps = 0;
        var szProps = 0;
        for (var i=0; i < doc.GetNumLines(); ++i)
        {
            var inst = doc.GetInstance(i);
            if (!inst.IsValid() || inst.Type.ToString() != "IFCPROPERTYSINGLEVALUE")
                continue;

            var lineSpan = doc.GetLineSpan(i);
            var e = doc.GetEntityFromLine(i);
            var pv = new IfcPropertyValue(e.Id, e.AttributeValues);
            propNames.Add(pv.Name);
            propDescriptions.Add(pv.Description);
            propUnits.Add(pv.Unit);
            propValues.Add(pv.Value);
            nProps++;
            szProps += lineSpan.Length;
        }

        Console.WriteLine($"Found {nProps} properties, total size was {szProps}");

        Console.WriteLine($"Found {propNames.Count} property names");
        Console.WriteLine($"Found {propDescriptions.Count} property descriptions");
        Console.WriteLine($"Found {propUnits.Count} property units");
        Console.WriteLine($"Found {propValues.Count} property values");

        Console.WriteLine("Property names:");
        foreach (var name in propNames)
            Console.WriteLine($"  {name}");
        
        Console.WriteLine("Property descriptions:");
        foreach (var description in propDescriptions)
            Console.WriteLine($"  {description}");

        Console.WriteLine("Property units:");
        foreach (var unit in propUnits)
            Console.WriteLine($"  {unit}");
    }

    [Test]
    public static void CountEntities()
    {
        var logger = Logger.Console;
        logger.Log($"Loading files");
        var files = LargeFiles().Concat(VeryLargeFiles()).Concat(HugeFiles());
        var docs = files.AsParallel().Select(f => new StepDocument(f)).ToList();
        logger.Log($"Loaded {docs.Count} files");

        logger.Log("Performing analysis");
        var ea = EntitySizeAnalysis.Create(docs);
        logger.Log($"Found {ea.EntitySizes.Count} distinct entities");

        var es = ea.GetStats();
        logger.Log("Computed stats");

        var totalSize = es.Values.Sum(s => s.Sum);
        logger.Log($"Total size of data is {PathUtil.BytesToString((long)totalSize)}");

        foreach (var kv in es.OrderByDescending(kv => kv.Value.Sum))
        {
            var s = kv.Value;
            var p = 100.0 * (s.Sum / totalSize);
            Console.WriteLine($"{kv.Key}\t{s.Count}\t{s.Average:F3}\t{s.Sum}\t{p:F3}");
        }
    }

    [Test]
    [TestCaseSource(nameof(LargeFiles))]
    public static void Timings(FilePath fp)
    {
        Console.WriteLine($"Testing {fp.GetFileName()} which is {fp.GetFileSizeAsString()}");
        TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        TimingUtils.TimeIt(() => HyparLoadIfc(fp), "Hypar");
        TimingUtils.TimeIt(() => GeometryGymLoadIfc(fp), "GeometryGym");
        TimingUtils.TimeIt(() => XBimLoadIfc(fp), "XBim");
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
    public static unsafe void TestPropSets()
    {
        var fp = LargeFiles().First();
        Console.WriteLine(fp);
        Console.WriteLine();

        var logger = Logger.Console;
        using var doc = new StepDocument(fp, logger);
        var propSetInstances = doc.GetInstances("IFCPROPERTYSET").ToList();
        logger.Log($"Found {propSetInstances.Count} property sets");

        for (var i = 0; i < 25; ++i)
        {
            if (i >= propSetInstances.Count)
                break;
            var inst = propSetInstances[i];
            var index = doc.Lookup.Find(inst.Id);
            Console.WriteLine($"Id {i}, Index {index}, Type {inst}");
            var span = doc.GetLineSpan(index);
            Console.WriteLine(span);
            var attr = StepFactory.GetAttributes(inst, span.End());
            Assert.IsNotNull(attr);
            Console.WriteLine(attr);
            var ps = new IfcPropertySet(inst.Id, attr.Values);
            Console.WriteLine(ps);
        }
    }

    [Test]
    public static void IdentifierCounts()
    {
        var fp = LargeFiles().First();
        var doc = new StepDocument(fp);
        var d = doc.GetInstances().Where(i => i.IsValid()).GroupBy(r => r.Type).ToDictionary(g => g.Key, g => g.Count());
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