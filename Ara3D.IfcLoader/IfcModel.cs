namespace Ara3D.IfcLoader
{
    public class IfcModel
    {
        public IntPtr ApiPtr;
        public IntPtr ModelPtr;

        public IfcModel(IntPtr apiPtr, IntPtr modelPtr)
        {
            ApiPtr = apiPtr;
            ModelPtr = modelPtr;
        }

        public IfcGeometry? GetGeometry(uint id)
        {
            var gPtr = WebIfcDll.GetGeometryFromId(ApiPtr, ModelPtr, id);
            return gPtr == IntPtr.Zero ? null : new IfcGeometry(ApiPtr, gPtr);
        }

        public IEnumerable<IfcGeometry> GetGeometries()
        {
            var numGeometries = WebIfcDll.GetNumGeometries(ApiPtr, ModelPtr);
            for (int i = 0; i < numGeometries; ++i)
            {
                var gPtr = WebIfcDll.GetGeometryFromIndex(ApiPtr, ModelPtr, i);
                if (gPtr != IntPtr.Zero)
                    yield return new IfcGeometry(ApiPtr, gPtr);
            }
        }
    }
}