﻿using Ara3D.IfcLoader;
using Ara3D.Logging;
using Ara3D.Utils;
using NUnit.Framework;
using static Ara3D.IfcParser.Test.Config;

namespace Ara3D.IfcParser.Test
{
    public static class IfcLoadTests
    {
        public static IReadOnlyList<FilePath> Files = Config.Files;

        [Test, Explicit("Takes a long time to run")]
        [TestCaseSource(nameof(Files))]
        public static void TestIfcFile(FilePath fp)
        {
            var logger = CreateLogger();
            var (rd, file) = RunDetails.LoadGraph(fp, true, logger);
            OutputDetails(file, logger);
            Console.WriteLine(rd.Header());
            Console.WriteLine(rd.RowData());
        }

        [Test]
        [TestCaseSource(nameof(Files))]
        public static void TestIfcFileWithoutGeometry(FilePath fp)
        {
            var logger = CreateLogger();
            var (rd, file) = RunDetails.LoadGraph(fp, false, logger);
            OutputDetails(file, logger);
            Console.WriteLine(rd.Header());
            Console.WriteLine(rd.RowData());
        }

        public static void OutputDetails(IfcFile file, ILogger logger)
        {
            logger.Log($"Loaded {file.FilePath.GetFileName()}");
            logger.Log($"Graph nodes: {file.Graph.Nodes.Count}");
            logger.Log($"Graph relations: {file.Graph.Relations.Count}");
            logger.Log($"Graph roots: {file.Graph.RootIds.Count}");
            logger.Log($"Property sets: {file.Graph.GetPropSets().Count()}");
            logger.Log($"Property values: {file.Graph.GetProps().Count()}");
            logger.Log($"Express ids: {file.Document.NumRawInstances}");
            logger.Log($"Geometry loaded: {file.GeometryDataLoaded}");
            logger.Log($"Num geometries loaded: {file.Model?.GetNumGeometries()}");
            logger.Log($"Num meshes loaded: {file.Model?.GetGeometries().Select(g => g.GetNumMeshes()).Sum()}");
        }
    }
}
