using Ara3D.Logging;
using Ara3D.SimpleDB;
using Ara3D.Utils;
using NUnit.Framework;
using static Ara3D.IfcParser.IfcPropertyDatabase;

namespace Ara3D.IfcParser.Test;

public static class DatabaseTests
{
    public static IEnumerable<FilePath> InputFiles()
        => StepTests.LargeFiles();

    public static void OutputDatabase(SimpleDatabase db, ILogger logger)
    {
        logger.Log("Describing database: ");
        foreach (var t in db.GetTables())
        {
            logger.Log($"  table {t.Name} has {t.Objects.Count} objects and schema {t.TableSchema}");
        }
    }

    [Test]
    [TestCaseSource(nameof(InputFiles))]
    public static void DatabaseTest(FilePath f)
    {
        var logger = Logger.Console;
        var doc = new StepDocument(f, logger);
        var db = new IfcPropertyDatabase();

        var szProps = 0;
        var cntProps = 0;

        var szSets = 0;
        var cntSets = 0;

        for (var i = 0; i < doc.GetNumLines(); ++i)
        {
            var inst = doc.GetInstance(i);
            if (!inst.IsValid())
                continue;

            var lineSpan = doc.GetLineSpan(i);
            var type = inst.Type.ToString();
            if (type == "IFCPROPERTYSINGLEVALUE")
            {
                szProps += lineSpan.Length;
                cntProps++;
            }
            if (type == "IFCPROPERTYSET")
            {
                szSets += lineSpan.Length;
                cntSets++;
            } 
        }

        logger.Log($"Found {cntProps} properties {PathUtil.BytesToString(szProps)}");
        logger.Log($"Found {cntSets} property sets {PathUtil.BytesToString(szSets)}");

        logger.Log($"Adding document to database");
        db.AddDocument(doc, logger);
        OutputDatabase(db.Db, logger);

        var fp = PathUtil.CreateTempFile();
        logger.Log("Writing database to disk");
        db.Db.WriteToFile(fp, logger);
        logger.Log("Wrote database to disk");

        logger.Log("Reading database from disk");
        var tmp = SimpleDatabase.ReadFile(fp, TableTypes, logger);
        logger.Log("Read database from disk");
        OutputDatabase(tmp, logger);

        CompareDB(db.Db, tmp, logger);

        var inputSize = f.GetFileSizeAsString();
        var outputSize = fp.GetFileSizeAsString();
        var propSize = PathUtil.BytesToString(szSets + szProps);
        logger.Log($"From IFC file of {inputSize} with {propSize} props to database of {outputSize}");
    }


    public static void CompareDB(SimpleDatabase db1, SimpleDatabase db2, ILogger logger)
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

        logger.Log("Database comparsion works");
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

        var fp = OutputFolder.RelativeFile(name + ".db");
        logger.Log("Writing database to disk");
        db.Db.WriteToFile(fp, logger);
        logger.Log("Wrote database to disk");

        var inputSize = PathUtil.BytesToString(totalSize);
        var outputSize = fp.GetFileSizeAsString();

        logger.Log($"Found {cntProps} properties {PathUtil.BytesToString(szProps)}");
        logger.Log($"Found {cntSets} property sets {PathUtil.BytesToString(szSets)}");

        logger.Log($"From {cnt} IFC files of {inputSize} to property database of {outputSize}");
    }

    [Test]
    public static void TestLoadDb()
    {
        var logger = Logger.Console;

        foreach (var fp in OutputFolder.GetFiles("*.db"))
        {
            logger.Log($"Reading database {fp.GetFileName()} from disk");
            var tmp = SimpleDatabase.ReadFile(fp, TableTypes, logger);
            logger.Log("Read database from disk");
            OutputDatabase(tmp, logger);
        }
    }
}