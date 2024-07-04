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


    [Test]
    [TestCaseSource(nameof(VeryLargeFiles))]
    public static void Timings(FilePath fp)
    {
        Console.WriteLine($"Testing {fp.GetFileName()} which is {fp.FileSizeAsString()}");
        TimingUtils.TimeIt(() => Ara3DLoadIfc(fp), "Ara 3D");
        //TimingUtils.TimeIt(() => HyparLoadIfc(fp), "Hypar");
        //TimingUtils.TimeIt(() => GeometryGymLoadIfc(fp), "GeometryGym");
        //TimingUtils.TimeIt(() => XBimLoadIfc(fp), "XBim");
    }

    public static void Ara3DLoadIfc(FilePath filePath)
    {
        var data = new StepDocument(filePath);
        Console.WriteLine($"#tokens = {data.NumTokens}, #entities = {data.NumEntities}, error = {data.FirstError}");
        /*
        for (var i = 0; i < 50; i++)
        {
            if (i >= data.GetNumTokens())
                break;
            Console.WriteLine($"Token {i}:{data.GetTokenType(i)} = {data.GetTokenString(i)}");
        }\

        */
        data.Dispose();
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