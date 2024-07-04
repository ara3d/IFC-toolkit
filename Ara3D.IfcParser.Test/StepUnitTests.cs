using Ara3D.Utils;

namespace Ara3D.IfcParser.Test
{
    public static class StepUnitTests
    {
        [Test, Explicit]
        public static void OutputFilesInFolder()
        {
            var folder = new DirectoryPath(@"C:\Users\cdigg\dev\impraria");
            var files = folder.GetAllFilesRecursively().Where(f => f.HasExtension("ifc")).OrderBy(f => f.GetFileSize());

            foreach (var f in files)
            {
                Console.WriteLine($@"// {f.FileSizeAsString()}");
                Console.WriteLine($"@\"{f.Value}\",");
            }
        }
    }
}