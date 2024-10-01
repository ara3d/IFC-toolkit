﻿using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.StepParser;
using Ara3D.Utils;

namespace Ara3D.IfcLoader
{
    public class IfcFile
    {
        public IfcGraph Graph;
        public StepDocument Document => Graph.Document;
        public IfcModel Model;
        public IntPtr ApiPtr;
        public FilePath FilePath;
        public Exception Exception;

        public IfcFile(FilePath fp, bool includeGeometry, ILogger? logger = null)
        {
            try
            {
                if (!File.Exists(fp))
                    throw new FileNotFoundException($"File not found: {fp}");
                FilePath = fp;

                logger?.Log($"Starting load of {fp.GetFileName()}");
                if (includeGeometry)
                {
                    Parallel.Invoke(
                        () => { LoadGeometryData(logger); },
                        () => { LoadNonGeometryData(logger); });
                }
                else
                {
                    LoadNonGeometryData(logger);
                }
                logger?.Log($"Completed loading {fp.GetFileName()}");
            }
            catch (Exception e)
            {
                Exception = e;
                logger?.LogError(e);
            }
        }

        private void LoadGeometryData(ILogger? logger)
        {
            logger?.Log($"Loading IFC geometry");
            ApiPtr = WebIfcDll.InitializeApi();
            Model = new IfcModel(ApiPtr, WebIfcDll.LoadModel(ApiPtr, FilePath));
            logger?.Log($"Completed loading IFC geometry");
        }

        private void LoadNonGeometryData(ILogger? logger)
        {
            logger?.Log($"Loading IFC data");
            Graph = IfcGraph.Load(FilePath, logger);
            logger?.Log($"Completed loading IFC data");
        }

        public static IfcFile Load(FilePath fp, bool includeGeometry, ILogger logger = null)
            => new IfcFile(fp, includeGeometry, logger);

        public bool GeometryDataLoaded 
            => Model != null;
    }
}