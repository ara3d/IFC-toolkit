using System.Diagnostics;
using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.Spans;
using Ara3D.Utils;

namespace Ara3D.IfcTool
{
    public static class Program
    {
        public static void Find(FilePath fp, ByteSpan entity, ILogger logger)
        {
            if (!fp.Exists())
            {
                Console.WriteLine($"Could not file {fp}");
                return;
            }
            logger.Log($"Opening file {fp.GetFileName()} of size {fp.GetFileSizeAsString()}");
            using var doc = new StepDocument(fp, Logger.Debug);
            var recs = doc.GetInstances(entity).ToList();
            logger.Log($"Found {recs.Count} instances of type {entity}");
        }

        public static void RunThreads(int threadCount, Action<int> action)
        {
            var threads = new List<Thread>();
            for (var i = 0; i < threadCount; i++)
            {
                var thread = new Thread(startData => action((int)startData!));
                threads.Add(thread);
                thread.Start(i);
            }
            foreach (var t in threads)
            {
                t.Join();
            }
        }

        public static void FindAll(DirectoryPath dp, ByteSpan entity, ILogger logger)
        {
            if (!dp.Exists())
            {
                Console.WriteLine($"Could not find directory {dp}");
                return;
            }
            logger.Log($"Looking for IFC files in {dp}");
            var files = dp.GetFiles("*.ifc", true).ToList();
            logger.Log($"Found {files.Count} files");

            const int numThreads = 8;
            RunThreads(numThreads,
                curThread =>
                {
                    for (var i = 0; i < files.Count; i++)
                        if (i % numThreads == curThread)
                            Find(files[i], entity, logger);
                });

            //Parallel.ForEach(files, f => Find(f, entity, logger));
            //foreach (var f in files) Find(f, entity, logger);
            logger.Log("Completed");
        }

        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Usage();
                return;
            }

            var verb = args[0].ToLowerInvariant();

            var logger = Logger.Console;
            if (verb == "find")
            {
                Find(args[2], args[1].ToByteSpanPinned(), logger);
            }
            else if (verb == "findall")
            {
                FindAll(args[2], args[1].ToByteSpanPinned(), logger);
            }
            else 
            {
                Console.WriteLine($"{verb} is not a recognized verbs");
                Usage();
            }
        }

        public static void Usage()
        {
            var name = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine($"Usage is {name} <verb> <entity> <filename>");
            Console.WriteLine("Currently the only valid <verb>");
            Console.WriteLine("Product is an IFC entity type like IFCDOOR.");
            Console.WriteLine("Will output the number of entities found");
        }
    }
}
