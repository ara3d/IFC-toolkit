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
            => Files;

        [TestCaseSource(nameof(InputFiles))]
        public static void IfcFileToSpeckleToJson(FilePath f)
        {
            var logger = CreateLogger();
            IfcFileToSpeckle(f, logger, false);
        }

        public static Base IfcFileToSpeckle(FilePath f, ILogger logger, bool showJson = false)
        {
            var b = IfcFileToBase(f, logger);

            var convertedRoot = b.ToSpeckleObject();

            var tmp = PathUtil.CreateTempFile("json");
            var json = convertedRoot.ToJson();
            tmp.WriteAllText(json);
            logger?.Log($"Wrote json to: {tmp}");
            
            if (showJson)   
                tmp.OpenFile();

            return b;
        }

        [Test, Explicit]
        public static void ListModelsInProject()
        {
            SpeckleUtils.ListModels(CreateLogger(), Config.TestProjectId);
        }

        [Test, Explicit]
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
            logger?.Log($"Pushed to Speckle at {TestProjectUrl}");
        }

        [Test, Explicit]
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

    }
}
