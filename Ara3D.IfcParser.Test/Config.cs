using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcParser.Test
{
    public static class Config
    {
        public static ILogger CreateLogger() => new Logger(LogWriter.ConsoleWriter, "");
        public static FilePath AC20Haus => InputFolder.RelativeFile("AC20-FZK-Haus.ifc");
        public static DirectoryPath InputFolder => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "test-files");
        public static IReadOnlyList<FilePath> Files => InputFolder.GetFiles("*.ifc").ToList();
        
        public static string TestProjectId = "5ff38fb6b1";
    }
}
