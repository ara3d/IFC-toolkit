using Ara3D.Utils;

namespace Ara3D.IfcParser.Test
{
    public static class InputFiles
    {
        public static FilePath AC20Haus = InputFolder.RelativeFile("AC20-FZK-Haus.ifc");
        public static DirectoryPath InputFolder = PathUtil.GetCallerSourceFolder().RelativeFolder("..", "test-files");
        public static IReadOnlyList<FilePath> Files = InputFolder.GetFiles("*.ifc").ToList();
    }
}
