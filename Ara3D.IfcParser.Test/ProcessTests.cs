using System.Diagnostics;
using Ara3D.Logging;
using Ara3D.Utils;
using NUnit.Framework;

namespace Ara3D.IfcParser.Test;

public static class ProcessTests
{

    public static string WebIfcExe = @"C:\Users\cdigg\git\temp\engine_web-ifc\src\cpp\Release\web-ifc-test.exe";
    public static string Ara3DIfcExe = @"C:\Users\cdigg\git\ifc-parser\Ara3D.IfcTool\bin\Release\net8.0\Ara3D.IfcTool.exe";

    public static void TestAra3D(string filePath, string entityType)
    {
        var proc = new ProcessStartInfo(Ara3DIfcExe, $"find {entityType} \"{filePath}\"");
        LaunchAndProfile(proc);
    }

    public static void TestWebIfc(string filePath, string entityType)
    {
        var proc = new ProcessStartInfo(WebIfcExe, $"{entityType} \"{filePath}\"");
        LaunchAndProfile(proc);
    }

    public static void LaunchAndProfile(ProcessStartInfo psi)
    {
        TimingUtils.TimeIt(() =>
        {
            psi.RedirectStandardOutput = true;
            var p = new Process() { StartInfo = psi,  };
            p.Start();
            Console.WriteLine(p.StandardOutput.ReadToEnd());
            p.WaitForExit();
        }, psi.FileName);
    }

    [Test]
    public static void RunProcessTests()
    {
        var files = TestFiles.HugeFiles();

        foreach (var f in files)
        {
            TestAra3D(f, "IFCDOOR");
            TestWebIfc(f, "IFCDOOR");
            TestAra3D(f, "IFCFACE");
            TestWebIfc(f, "IFCFACE");
        }
    }

    [Test]
    public static void ThreadedWebIfcTest()
    {
        var logger = Logger.Console;
        var dp = new DirectoryPath(@"C:\Users\cdigg\dev\impraria");
        if (!dp.Exists())
        {
            Console.WriteLine($"Could not find directory {dp}");
            return;
        }
        logger.Log($"Looking for IFC files in {dp}");
        
        // Starting with just 8 files.
        var files = dp.GetFiles("*.ifc", true).ToList();

        logger.Log($"Found {files.Count} files");

        const int numThreads = 4;
        ThreadUtil.RunThreads(numThreads,
            curThread =>
            {
                for (var i = 0; i < files.Count; i++)
                    if (i % numThreads == curThread)
                        TestWebIfc(files[i], "IFCDOOR");
            });

        //Parallel.ForEach(files, f => Find(f, entity, logger));
        //foreach (var f in files) Find(f, entity, logger);
        logger.Log("Completed");
    }
}