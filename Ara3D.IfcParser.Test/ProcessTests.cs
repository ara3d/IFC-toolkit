using System.Diagnostics;
using Ara3D.Utils;

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
}