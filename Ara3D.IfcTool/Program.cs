using System.Diagnostics;
using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.Spans;
using Ara3D.Utils;

namespace Ara3D.IfcTool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Usage();
                return;
            }

            if (args[0] != "find")
            {
                Console.WriteLine($"{args[0]} is not a recognized verbs");
                Usage();
                return;
            }
            
            var e = args[1];

            var fp = new FilePath(args[2]);
            if (!fp.Exists())
            {
                Console.WriteLine($"Could not file {fp}");
            }

            var logger = Logger.Console;
            logger.Log($"Opening file {fp.GetFileName()} of size {fp.GetFileSizeAsString()}");
            var doc = new StepDocument(fp, logger);
            var span = e.ToByteSpanPinned();
            var recs = doc.GetInstances(span).ToList();
            logger.Log($"Found {recs.Count} instances of type {span}");
            logger.Log("Completed");
        }

        public static void Usage()
        {
            var name = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine($"Usage is {name} <verb> <entity> <filename>");
            Console.WriteLine("Currently the only valid <verb> is find");
            Console.WriteLine("Product is an IFC entity type like IFCDOOR.");
            Console.WriteLine("Will output the number of entities found");
        }
    }
}
