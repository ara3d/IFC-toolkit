using Ara3D.Utils;
using NUnit.Framework;
using static Ara3D.IfcParser.Test.Config;

namespace Ara3D.IfcParser.Test
{
    public static class MainTests
    {
        public static IReadOnlyList<FilePath> Files = Config.Files;

        [Test]
        [TestCaseSource(nameof(Files))]
        public static void TestIfcFile(FilePath fp)
        {
            var logger = CreateLogger();
            var (rd, file) = RunDetails.LoadGraph(fp, true, logger);
            Assert.Null(file.Exception, file.Exception?.Message);
            Console.WriteLine(rd.Header());
            Console.WriteLine(rd.RowData());
        }
    }
}
