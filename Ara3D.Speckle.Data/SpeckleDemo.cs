using System;
using System.IO;
using System.Linq;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;

namespace Ara3D.Speckle.Data
{
    // https://github.com/specklesystems/speckle-examples/blob/main/dotnet/dotnet-speckle-starter/Program.cs
    public class SpeckleDemo
    {
        // Running this program will pull the latest commit from the main branch
        // of the specified stream and duplicate it inside a different branch.
        // (branch should exist already or the program will fail)
        public static void  Main(string[] args)
        {
            // The id of the stream to work with (we're assuming it already exists in your default account's server)
            //var streamId = "51d8c73c9d";
            //var streamId = "97529188be"; 

            // Advanced Revit Project 
            var streamId = "8f64180899"; var branchName = "main";

            // Default Speckle architecture 
            //var streamId = "3247bdd4ee"; var branchName = "base design";

            // Get default account on this machine
            // If you don't have Speckle Manager installed download it from https://speckle-releases.netlify.app
            var defaultAccount = AccountManager.GetDefaultAccount();

            // Or get all the accounts and manually choose the one you want
            // var accounts = AccountManager.GetAccounts();
            // var defaultAccount = accounts.ToList().FirstOrDefault();

            if (defaultAccount == null)
                throw new Exception("Could not find a default account. You may need to install and run the Speckle Manager");
            
            // Authenticate using the account
            using (var client = new Client(defaultAccount))
            {
                // Get the main branch with it's latest commit reference
                var branch = client.BranchGet(streamId, branchName, 1).Result;

                // Get the id of the object referenced in the commit
                var hash = branch.commits.items[0].referencedObject;

                // Create the server transport for the specified stream.
                var transport = new ServerTransport(defaultAccount, streamId);

                // Receive the object
                var root = Operations.Receive(hash, transport).Result;
                Console.WriteLine("Received object:" + root.id);

                OutputNative(root.ToSpeckleObject());

                // Write to a local database 
                var tmp = Path.GetTempPath();
                var localSql = new SQLiteTransport(tmp);
                Operations.Send(root, new[] { localSql });
            }

            Console.WriteLine("Press any key to continue ... ");
            Console.ReadKey();
        }

        public static void OutputNative(SpeckleObject obj, string indent = "")
        {
            Console.WriteLine($"{indent}{obj.Id}:{obj.SpeckleType}:{obj.DotNetType}");
            foreach (var child in obj.Elements.OfType<SpeckleObject>())
                OutputNative(child, indent + "  ");
        }

        public static void ReadFromDisk()
        {
            var filePath = @"C:\Users\cdigg\AppData\Local\Temp\Speckle";
            var localSql = new SQLiteTransport(filePath);
            var root = Operations.Receive("f0fa094f0c24fba78171bd57816f3797", localSql).Result;
            Console.Write($"Received {root}");

            OutputNative(root.ToSpeckleObject()); 
        }
    }
}