using Ara3D.Logging;
using Ara3D.Utils;
using GeometryGym.Ifc;
using IFC;
using STEP;
using Xbim.Ifc;

namespace Ara3D.IfcParser.Test;

public static class ProfilingTests
{
    public static IEnumerable<FilePath> VeryLargeFiles() 
        => TestFiles.VeryLargeFiles();

    public static IEnumerable<FilePath> HugeFiles()
        => TestFiles.HugeFiles();

    public static void CreateRecords(StepDocument doc)
    {
        foreach (var rec in doc.Records)
        {
            var tmp = StepFactory.CreateRecord(doc, rec);
            //Console.WriteLine($"Create {tmp.Id} = {tmp.Value}");
        }
    }


    [Test]
    public static void CreateRecords()
    {
        using var d = TimingUtils.TimeIt("Creating records"); 
        //var file = @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEC\07-004003-4200000004-AED-MEC-MDL-000001_IFC_D.ifc";
        var file = @"C:\Users\cdigg\dev\impraria\Trojena\4200000004 - NEOM SKI VILLAGE CONCEPT DESIGN\07-004003-4200000004-AED-MEC-MDL-006100.ifc";
        var doc = new StepDocument(file);
        Console.WriteLine($"#tokens = {doc.NumTokens}, #entities = {doc.NumRecords}, error = {doc.FirstError}");
        foreach (var rec in doc.Records)
        {
            var tmp = StepFactory.CreateRecord(doc, rec);
            Console.WriteLine($"Create {tmp.Id} = {tmp.Value}");
        }
    }

    [Test]
    [TestCaseSource(nameof(VeryLargeFiles))]
    public static void Timings(FilePath fp)
    {
        Console.WriteLine($"Testing {fp.GetFileName()} which is {fp.GetFileSizeAsString()}");
        TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        //TimingUtils.TimeIt(() => HyparLoadIfc(fp), "Hypar");
        //TimingUtils.TimeIt(() => GeometryGymLoadIfc(fp), "GeometryGym");
        //TimingUtils.TimeIt(() => XBimLoadIfc(fp), "XBim");
    }

    public static void Ara3DLoadIfc(FilePath filePath)
    {
        var doc = new StepDocument(filePath);
        Console.WriteLine($"#tokens = {doc.NumTokens}, #entities = {doc.NumRecords}, error = {doc.FirstError}");
        
        //CreateRecords(doc);
        /*
        for (var i = 0; i < 50; i++)
        {
            if (i >= data.GetNumTokens())
                break;
            Console.WriteLine($"Token {i}:{data.GetTokenType(i)} = {data.GetTokenString(i)}");
        }\
        */
        doc.Dispose();
    }

    public static void HyparLoadIfc(FilePath filePath)
    {
        var ifcModel = new Document(filePath, out List<STEPError> errors);
    }

    public static void GeometryGymLoadIfc(FilePath filePath)
    {
        var database = new DatabaseIfc(filePath);
    }

    public static void XBimLoadIfc(FilePath filePath)
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

        // Bizarre loading logic.
        var sizeLimit = 20.0 * 1000 * 1000 * 1000;
        var model = IfcStore.Open(filePath, editor, sizeLimit);
        model.Dispose();
    }
}