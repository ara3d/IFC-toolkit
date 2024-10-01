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
            var gPtr = WebIfcDll.GetGeometry(ApiPtr, ModelPtr, id);
            return gPtr == IntPtr.Zero ? null : new IfcGeometry(ApiPtr, gPtr);
        }
    }
}