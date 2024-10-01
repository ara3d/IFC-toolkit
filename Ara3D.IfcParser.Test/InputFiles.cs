using Ara3D.Utils;

namespace Ara3D.IfcParser.Test
{
    public static class InputFiles
    {
        public static DirectoryPath InputFolder = PathUtil.GetCallerSourceFolder().RelativeFolder("..", "test-files");
        public static IReadOnlyList<FilePath> Files = InputFolder.GetFiles("*.ifc").ToList();
    }
}
