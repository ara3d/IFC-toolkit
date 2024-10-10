using Ara3D.IfcLoader;
using Ara3D.IfcParser.Test;
using Ara3D.Speckle.Data;
using Ara3D.Utils;
using Ara3D.Logging;
using Ara3D.Speckle.IfcLoader;
using NUnit.Framework;
using static Ara3D.IfcParser.Test.Config;
using Base = Speckle.Core.Models.Base;

namespace WebIfcDotNetTests
{
    public static class SpeckleTests
    {
        public static Base IfcFileToBase(FilePath fp, ILogger logger)
        {
            var f = IfcFile.Load(fp, true, logger);
            logger?.Log("Converting to speckle");
            var r = f.ToSpeckle();
            logger?.Log("Conversion completed");
            return r;
        }

        public static IEnumerable<FilePath> InputFiles
            => new[] { AC20Haus };

        [TestCaseSource(nameof(InputFiles))]
        public static void IfcFileToSpeckleToJson(FilePath f)
        {
            var logger = CreateLogger();
            var b = IfcFileToBase(f, logger);

            var convertedRoot = b.ToSpeckleObject();

            var tmp = PathUtil.CreateTempFile("json");
            var json = convertedRoot.ToJson();
            tmp.WriteAllText(json);
            logger?.Log($"Wrote json to: {tmp}");
            tmp.OpenFile();
        }

        [Test]
        public static void ListModels()
        {
            SpeckleUtils.ListModels(CreateLogger(), Config.TestProjectId);
        }

        [Test]
        public static void PushAc20Haus()
        {
            var logger = CreateLogger();
            var f = AC20Haus;
            logger?.Log($"Converting {f} to Speckle");
            var b = IfcFileToBase(f, logger);
            logger?.Log($"Conversion completed");
            var client = SpeckleUtils.LoginDefaultClient(logger);
            var result = client.PushModel(TestProjectId, f.GetFileName(), b, logger);
            logger?.Log(result);
        }

        [Test]
        public static void PushAllTestFiles()
        {
            var inputFiles = Files;
            foreach (var f in inputFiles)
            {
                var logger = CreateLogger();
                try
                {
                    logger?.Log($"Converting {f} to Speckle");
                    var b = IfcFileToBase(f, logger);
                    logger?.Log($"Conversion completed");
                    var client = SpeckleUtils.LoginDefaultClient(logger);
                    var result = client.PushModel("d1553c3803", f.GetFileName(), b, logger);
                    logger?.Log(result);
                }
                catch (Exception e)
                {
                    logger?.LogError(e);
                }
            }
        }

        [Test]
        public static void MoveHaus()
        {
            var logger = CreateLogger();
            var client = SpeckleUtils.LoginDefaultClient(logger);

            // https://app.speckle.systems/projects/68da6db112/models/c78d273327
            var srcModel = client.PullModelFromId("68da6db112", "c78d273327", logger);

            // https://app.speckle.systems/projects/d1553c3803/models/15e913104e
            var result = client.PushModelToId("d1553c3803", "15e913104e", srcModel, logger);

            Console.WriteLine($"Got model and pushed it somwhere else. Result was {result}");
        }
    }
}
