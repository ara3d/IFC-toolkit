using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.IfcParser
{
    public class IfcFace
    {
        public IfcFaceBound FaceBound;
    }

    public class IfcFaceBound
    {
        public IfcLoop Loop;
        public bool Orientation;
        public bool IsOuter;
    }
    
    public class IfcConnectedFaceSet
    {
        public List<IfcFace> Faces;
    }

    public class IfcClosedShell : IfcConnectedFaceSet
    {
    }

    public class IfcOpenShell : IfcConnectedFaceSet
    {
        
    }

    public class IfcLoop
    { }

    public class IfcPolyLoop : IfcLoop 
    {
        public List<IfcPoint> Points;
    }

    public class IfcPoint
    {
        public double X;
        public double Y;
        public double Z;
    }
}
