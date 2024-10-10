using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ara3D.Logging;
using Ara3D.Utils;
using Speckle.Core.Api;
using Speckle.Core.Api.GraphQL.Inputs;
using Speckle.Core.Api.GraphQL.Models;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using Model = Speckle.Core.Api.GraphQL.Models.Model;

namespace Ara3D.Speckle.Data
{
    public static class SpeckleUtils
    {
        public static Client LoginDefaultClient(ILogger logger)
        {
            var accounts = AccountManager.GetAccounts();
            foreach (var account in accounts)
                logger?.Log($"Account: {account.serverInfo.url} {account.userInfo.email}");

            logger?.Log($"Getting default account for this machine");
            var defaultAccount = AccountManager.GetDefaultAccount();
            if (defaultAccount == null)
                throw new Exception(
                    "Could not find a default account. You may need to install and run the Speckle Manager");

            logger?.Log($"Authenticating with this account");
            return new Client(defaultAccount);
        }

        public static FilePath WriteToSqlDatabase(this Base root)
            => root.WriteToSqlDatabase(Path.GetTempPath());

        public static FilePath WriteToSqlDatabase(this Base root, FilePath fp)
        {
            var localSql = new SQLiteTransport(fp);
            Operations.Send(root, new[] { localSql });
            return fp;
        }

        public static Base ReadFromSqlDatabase(FilePath fp, string objectId)
        {
            var localSql = new SQLiteTransport(fp);
            return Operations.Receive(objectId, localSql).Result;
        }

        public static string ToJson(this Base speckleBase)
            => speckleBase.ToSpeckleObject().ToJson();

        public static Project GetProject(this Client client, string projectId)
            => client.Project.Get(projectId).Result;

        public static IEnumerable<Model> GetModels(this Client client, string projectId)
            => client.Model.GetModels(projectId).Result.items;

        public static Model CreateModel(this Client client, string projectId, string name, ILogger logger)
        {
            var input = new CreateModelInput(name, null, projectId);
            var model = client.Model.Create(input).Result;
            logger?.Log($"Created model {model.name}:{model.id}");
            return model;
        }

        public static Model GetModelOrDefault(this Client client, string projectId, string name)
            => client.GetModels(projectId).FirstOrDefault(m => m.name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public static Model GetModelOrCreate(this Client client, string projectId, string name, ILogger logger)
        {
            var model = client.GetModelOrDefault(projectId, name);
            if (model != null)
            {
                logger?.Log($"Found model {model.name}:{model.id}");
                return model;
            }
            logger?.Log($"No model named '{name}' found so creating one");
            return client.CreateModel(projectId, name, logger);
        }

        public static string PushModel(this Client client, string projectId, string name, Base root, ILogger logger)
        {
            logger?.Log($"Pushing model {name} to project {projectId}");
            var model = client.GetModelOrCreate(projectId, name, logger);
            logger?.Log($"Model Id = {model.id}");
            return client.PushModelToId(projectId, model.id, root, logger);
        }

        public static string PushModelToId(this Client client, string projectId, string modelId, Base root, ILogger logger)
        {
            logger?.Log($"Sending the Base object to a transport and getting the object ID");
            var transport = new ServerTransport(client.Account, projectId);
            var objectId = Operations.Send(root, new List<ITransport> { transport }).Result;
            logger?.Log($"Sent object {objectId}");

            logger?.Log($"Creating a CommitCreateInput object with the required details");
            var commitInput = new CommitCreateInput
            {
                objectId = objectId,
                branchName = modelId,
                streamId = projectId
            };

            logger?.Log($"Creating a commit with the object ID");
            return client.Version.Create(commitInput).Result;
        }

        public static Base PullModelFromId(this Client client, string projectId, string modelId, ILogger logger)
        {
            logger?.Log($"Getting the latest commit for the model");
            var versionList = client.Version.GetVersions(modelId, projectId, 1).Result;
            var firstVersion = versionList.items.FirstOrDefault()?.id;
            if (firstVersion == null)
                throw new Exception("No versions found for this model");
            logger?.Log($"Found version {firstVersion}");

            var objectId = client.Version.Get(firstVersion, modelId, projectId).Result.referencedObject;
            logger?.Log($"Object ID: {objectId}");

            return client.PullObject(projectId, objectId, logger);
        }

        public static Base PullObject(this Client client, string projectId, string objectId, ILogger logger)
        {
            logger?.Log($"Creating a ServerTransport object for the project");
            var transport = new ServerTransport(client.Account, projectId);

            var baseRoot = Operations.Receive(objectId, transport).Result;
            logger?.Log($"Receipt successful: {baseRoot.id}");

            return baseRoot;
        }

        public static Base PullModel(this Client client, string projectId, string name, ILogger logger)
        {
            logger?.Log($"Pulling model {name} from project {projectId}");
            var model = client.GetModelOrDefault(projectId, name);
            if (model == null)
                throw new Exception($"Model {name} not found in project {projectId}");
            logger?.Log($"Model Id = {model.id}");
            return client.PullModelFromId(projectId, name, logger);
        }

        public static string ProjectUrl(string projectId)
            => $"https://app.speckle.systems/projects/{projectId}";

        public static void ListModels(ILogger logger, string projectId)
        {
            logger.Log($"Connecting to project {ProjectUrl(projectId)}");
            var client = SpeckleUtils.LoginDefaultClient(logger);
            var models = client.GetModels(projectId);
            if (models == null)
            {
                logger?.Log($"Unable to retrieved models for project {projectId}");
                return;
            }
            logger?.Log($"Found {models.Count()} models");
            foreach (var m in models)
                logger?.Log($"{m.name}:{m.id}");
        }
    }
}
