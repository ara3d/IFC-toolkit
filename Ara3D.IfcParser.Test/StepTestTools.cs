using Ara3D.Utils;
using NUnit.Framework;

namespace Ara3D.IfcParser.Test
{
    public static class StepTestTools
    {
        [Test, Explicit]
        public static void OutputFilesInFolder()
        {
            var folder = new DirectoryPath(@"C:\Users\cdigg\dev\impraria");
            var files = folder.GetAllFilesRecursively().Where(f => f.HasExtension("ifc")).OrderBy(f => f.GetFileSize());

            foreach (var f in files)
            {
                Console.WriteLine($@"// {f.GetFileSizeAsString()}");
                Console.WriteLine($"@\"{f.Value}\",");
            }
        }
    }
}