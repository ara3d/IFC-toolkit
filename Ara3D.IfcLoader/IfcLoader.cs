using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcLoader
{
    public class IfcLoader
    {
        public IfcGraph Graph;
        public IfcModel Model;
        public IntPtr ApiPtr;
        public Exception Exception;

        public IfcLoader(FilePath fp, ILogger logger = null)
        {
            try
            {
                if (!File.Exists(fp))
                    throw new FileNotFoundException($"File not found: {fp}");
                ApiPtr = WebIfcDll.InitializeApi();
                Parallel.Invoke(
                    () => { Model = LoadModel(ApiPtr, fp); },
                    () => { Graph = IfcGraph.Load(fp, logger); });
                logger?.Log($"Completed loading {fp.GetFileName()}");
            }
            catch (Exception e)
            {
                Exception = e;
                logger?.LogError(e);
            }
        }

        public static IfcModel LoadModel(IntPtr apiPtr, FilePath fp)
            => new IfcModel(apiPtr, WebIfcDll.LoadModel(apiPtr, fp));
    }
}
