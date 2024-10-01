using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using Ara3D.Speckle.Data;
using Ara3D.Utils;
using Speckle.Core.Models;
using Ara3D.Logging;
using WebIfcClrWrapper;
using WebIfcDotNet;

namespace WebIfcDotNetTests
{
    public static class SpeckleTests
    {
        public static ILogger CreateLogger()
            => new Logger(LogWriter.ConsoleWriter, "");

        [Test]
        public static void LoadSpeckleObjectToJson()
        {
            // https://app.speckle.systems/projects/68da6db112/models/c78d273327@6e1954cfca

            var logger = CreateLogger();
            using var client = SpeckleUtils.LoginDefaultClient(logger);

            logger?.Log($"Getting the main branch and retrieving a model");
            var projectId = "68da6db112";
            var modelId = "c78d273327";
            var model = client.Model.Get(modelId, projectId).Result;
            logger?.Log($"Retrieved model {model.name}:{model.id}");

            // Create the server transport for the specified stream.
            var transport = new ServerTransport(client.Account, projectId);
            logger?.Log($"Created transport {transport.BaseUri}");

            // Receive the object

            var versionList = client.Version.GetVersions(modelId, projectId, 1).Result;
            var firstVersion = versionList.items.FirstOrDefault()?.id;
            if (firstVersion == null)
                throw new Exception("No versions found for this model");
            logger?.Log($"Found version {firstVersion}");

            var objectId = client.Version.Get(firstVersion, modelId, projectId).Result.referencedObject;
            logger?.Log($"Object ID: {objectId}");

            logger?.Log($"Receiving object: {objectId}");
            var baseRoot = Operations.Receive(objectId, transport).Result;
            logger?.Log($"Receipt successful: {baseRoot.id}");

            var convertedRoot = baseRoot.ToSpeckleObject();

            var tmp = PathUtil.CreateTempFile("json");
            var json = convertedRoot.ToJson();
            tmp.WriteAllText(json);
            logger?.Log($"Wrote json to: {tmp}");
            ProcessUtil.OpenFile(tmp);
        }

        public static Base IfcFileToBase(FilePath fp)
        {
            var api = new DotNetApi();
            var g = ModelGraph.Load(api, CreateLogger(), fp);
            return g.ToSpeckle();
        }

        [Test]
        public static void IfcFileToSpeckleToJson()
        {
            var f = "C:\\Users\\cdigg\\git\\web-ifc-dotnet\\src\\engine_web-ifc\\tests\\ifcfiles\\public\\AC20-FZK-Haus.ifc";
            var b = IfcFileToBase(f);

            var convertedRoot = b.ToSpeckleObject();

            var logger = CreateLogger();
            var tmp = PathUtil.CreateTempFile("json");
            var json = convertedRoot.ToJson();
            tmp.WriteAllText(json);
            logger?.Log($"Wrote json to: {tmp}");
            ProcessUtil.OpenFile(tmp);
        }

        [Test]
        public static void ListModels()
        {
            var logger = CreateLogger();
            var client = SpeckleUtils.LoginDefaultClient(logger);
            //var models = client.GetModels("68da6db112");
            var models = client.GetModels("d1553c3803");
            logger?.Log($"Found {models.Count()} models");
            foreach (var m in models)
                logger?.Log($"{m.name}:{m.id}");
        }

        [Test]
        public static void PushAc20Haus()
        {
            var logger = CreateLogger();
            var f = new FilePath("C:\\Users\\cdigg\\git\\web-ifc-dotnet\\src\\engine_web-ifc\\tests\\ifcfiles\\public\\AC20-FZK-Haus.ifc");
            logger?.Log($"Converting {f} to Speckle");
            var b = IfcFileToBase(f);
            logger?.Log($"Conversion completed");
            var client = SpeckleUtils.LoginDefaultClient(logger);
            var result = client.PushModel("d1553c3803", f.GetFileName(), b, logger);
            logger?.Log(result);
        }

        [Test]
        public static void PushAllTestFiles()
        {
            var inputFiles = MainTests.InputFiles.ToList();
            foreach (var f in inputFiles)
            {
                var logger = CreateLogger();
                try
                {
                    logger?.Log($"Converting {f} to Speckle");
                    var b = IfcFileToBase(f);
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
