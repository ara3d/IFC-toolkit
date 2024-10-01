namespace Ara3D.IfcLoader
{
    public class IfcGeometry
    {
        public readonly IntPtr ApiPtr;
        public readonly IntPtr GeometryPtr;
        public readonly int NumMeshes;
        public IfcGeometry(IntPtr apiPtr, IntPtr geometryPtr)
        {
            ApiPtr = apiPtr;
            GeometryPtr = geometryPtr;
            NumMeshes = WebIfcDll.GetNumMeshes(ApiPtr, GeometryPtr);
        }
        public IfcMesh GetMesh(int i) 
            => new IfcMesh(ApiPtr, WebIfcDll.GetMesh(ApiPtr, GeometryPtr, i));
    }
}