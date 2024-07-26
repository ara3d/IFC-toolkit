using Ara3D.Logging;
using Ara3D.NarwhalDB;
using Ara3D.Utils;
using NUnit.Framework;
using static Ara3D.IfcParser.IfcPropertyDatabase;

namespace Ara3D.IfcParser.Test;

public static class DatabaseTests
{
    public static IEnumerable<FilePath> InputFiles()
        => StepTests.LargeFiles();

    public static void OutputDatabase(DB db, ILogger logger)
    {
        logger.Log("Describing database: ");
        foreach (var t in db.GetTables())
        {
            logger.Log($"  table {t.Name} has {t.Objects.Count} objects");
        }
    }

    [Test]
    [TestCaseSource(nameof(InputFiles))]
    public static void SingleFileDatabaseTest(FilePath f)
    {
        MultiFileTest(new[] { f }, f.GetFileNameWithoutExtension());
    }

    public static void CompareDB(DB db1, DB db2)
    {
        Assert.AreEqual(db1.NumTables, db2.NumTables);
        var tables1 = db1.GetTables().OrderBy(t => t.Name).ToList();
        var tables2 = db2.GetTables().OrderBy(t => t.Name).ToList();

        for (var i = 0; i < tables1.Count; i++)
        {
            var t1 = tables1[i];
            var t2 = tables2[i];
            Assert.AreEqual(t1.Name, t2.Name);
            Assert.AreEqual(t1.Objects.Count, t2.Objects.Count);
            Assert.AreEqual(t1.Objects, t2.Objects);
        }
    }

    [Test]
    public static void VillageTest()
        => MultiFileTest(TestFiles.Village(), "village");

    [Test]
    public static void HealthTest()
        => MultiFileTest(TestFiles.Health(), "health");

    [Test]
    public static void MountainTest()
        => MultiFileTest(TestFiles.Mountain(), "mountain");

    [Test]
    public static void GulfTest()
        => MultiFileTest(TestFiles.Gulf(), "gulf");

    [Test]
    public static void AllFiles()
        => MultiFileTest(TestFiles.AllFiles(), "all");

    public static DirectoryPath OutputFolder
        = @"C:\Users\cdigg\dev\impraria\propdb";

    public static void MultiFileTest(IEnumerable<FilePath> files, string name)
    {
        var logger = Logger.Console;

        var db = new IfcPropertyDatabase();
        var szProps = 0;
        var cntProps = 0;
        var szSets = 0;
        var cntSets = 0;
        var totalSize = 0L;

        var cnt = 0;
        foreach (var f in files)
        {
            var curSize = f.GetFileSize();
            logger.Log($"Opening file {cnt++} of size {PathUtil.BytesToString(curSize)} {f}");
            totalSize += curSize;
            var doc = new StepDocument(f, logger);

            for (var i = 0; i < doc.GetNumLines(); ++i)
            {
                var inst = doc.GetInstance(i);

                if (!inst.IsValid())
                    continue;

                var lineSpan = doc.GetLineSpan(i);

                if (inst.Type.ToString() == "IFCPROPERTYSINGLEVALUE")
                {
                    szProps += lineSpan.Length;
                    cntProps++;
                }

                if (inst.Type.ToString() == "IFCPROPERTYSET")
                {
                    szSets += lineSpan.Length;
                    cntSets++;
                }
            }

            logger.Log($"Adding document to database");
            db.AddDocument(doc, logger);
        }

        OutputDatabase(db.Db, logger);

        var fp = OutputFolder.RelativeFile(name + ".bfast");
        logger.Log("Writing database to disk");
        db.Db.WriteToFile(fp, logger);
        logger.Log("Wrote database to disk");

        logger.Log("Reading database from disk");
        var tmp = DB.ReadFile(fp, TableTypes, logger);
        logger.Log("Read database from disk");
        OutputDatabase(tmp, logger);

        logger.Log("Comparing database written and read");
        CompareDB(db.Db, tmp);
        logger.Log($"Comparison successful");

        var inputSize = PathUtil.BytesToString(totalSize);
        var outputSize = fp.GetFileSizeAsString();

        logger.Log($"Found {cntProps} properties {PathUtil.BytesToString(szProps)}");
        logger.Log($"Found {cntSets} property sets {PathUtil.BytesToString(szSets)}");

        logger.Log($"From {cnt} IFC files of {inputSize} to property database of {outputSize}");

        var zip = fp.Zip();
        logger.Log($"Zipped file as {zip.GetFileSizeAsString()}");
;    }

    [Test]
    public static void TestLoadDb()
    {

        //foreach (var fp in OutputFolder.GetFiles("*.bfast"))
        var fp = OutputFolder.RelativeFile("all.bfast");
        var logger = Logger.Console;
        logger.Log($"Reading database {fp.GetFileName()} from disk");
        var tmp = DB.ReadFile(fp, TableTypes, logger);
        logger.Log("Read database from disk");
        OutputDatabase(tmp, logger);
    }
}