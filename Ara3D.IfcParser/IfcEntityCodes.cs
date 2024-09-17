using Ara3D.Buffers;

namespace Ara3D.IfcParser
{
    public static class IfcEntityCodes
    {
        public static readonly string[] MeshEntities = new string[]
        {
            "IFCPOLYLOOP",
            "IFCFACEOUTERBOUND",
            "IFCFACE",
            "IFCCARTESIANPOINT",
            "IFCINDEXEDPOLYGONALFACE",
            //"IFCAXIS2PLACEMENT3D",
            "IFCDIRECTION",
            //"IFCLOCALPLACEMENT",
            "IFCPOLYLINE",
            "IFCLINE",
            "IFCVECTOR",
            "IFCVERTEXPOINT"
        };
    
        public static readonly string[] GeometricEntities = new string[]
        {
            "IFC2DCOMPOSITECURVE",
            "IFCAXIS1PLACEMENT",
            "IFCAXIS2PLACEMENT2D",
            "IFCAXIS2PLACEMENT3D",
            "IFCBSPLINECURVE",
            "IFCBEZIERCURVE",
            "IFCBOUNDEDCURVE",
            "IFCBOUNDEDSURFACE",
            "IFCCARTESIANPOINT",
            "IFCCARTESIANTRANSFORMATIONOPERATOR",
            "IFCCARTESIANTRANSFORMATIONOPERATOR2D",
            "IFCCARTESIANTRANSFORMATIONOPERATOR2DNONUNIFORM",
            "IFCCARTESIANTRANSFORMATIONOPERATOR3D",
            "IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM",
            "IFCCIRCLE",
            "IFCCOMPOSITECURVE",
            "IFCCOMPOSITECURVESEGMENT",
            "IFCCONIC",
            "IFCCURVE",
            "IFCCURVEBOUNDEDPLANE",
            "IFCDIRECTION",
            "IFCELEMENTARYSURFACE",
            "IFCELLIPSE",
            "IFCGEOMETRICREPRESENTATIONITEM",
            "IFCLINE",
            "IFCMAPPEDITEM",
            "IFCOFFSETCURVE2D",
            "IFCOFFSETCURVE3D",
            "IFCPLACEMENT",
            "IFCPLANE",
            "IFCPOINT",
            "IFCPOINTONCURVE",
            "IFCPOINTONSURFACE",
            "IFCPOLYLINE",
            "IFCRATIONALBEZIERCURVE",
            "IFCRECTANGULARTRIMMEDSURFACE",
            "IFCREPRESENTATIONITEM",
            "IFCREPRESENTATIONMAP",
            "IFCSURFACE",
            "IFCSURFACEOFLINEAREXTRUSION",
            "IFCSURFACEOFREVOLUTION",
            "IFCSWEPTSURFACE",
            "IFCTRIMMEDCURVE",
            "IFCVECTOR",

            "IFCRECTANGLEPROFILEDEF",
            "IFCCIRCLEPROFILEDEF",
            "IFCISHAPEPROFILEDEF",

            "IFCARBITRARYCLOSEDPROFILEDEF",
            "IFCSHAPEREPRESENTATION",
            "IFCARBITRARYPROFILEDEFWITHVOIDS",
            "IFCGRID",
            "IFCGRIDAXIS",
            "IFCGEOMETRICCURVESET",
            "IFCSTYLEDITEM",
            "IFCEXTRUDEDAREASOLID",
            "IFCPRESENTATIONLAYERASSIGNMENT",

            "IFCFACETEDBREP",
            "IFCBOOLEANRESULT",
            "IFCBOOLEANCLIPPINGRESULT",
            "IFCFACEBASEDSURFACEMODEL",
            "IFCHALFSPACESOLID",

            "IFCFACEOUTERBOUND",
            "IFCFACE"
        };

        public static readonly string[] PropertyEntities = new string[]
        {
            "IFCPROPERTYSET",
            "IFCRELDEFINESBYPROPERTIES",
            "IFCPROPERTYSINGLEVALUE"
        };

        public static Dictionary<string, int> EntityToCodeLookup 
            = CreateEntityLookup();

        public static Dictionary<string, int> CreateEntityLookup()
        {
            var r = new Dictionary<string, int>();
            foreach (var entity in GeometricEntities)
            {
                r.Add(entity, r.Count);
            }

            return r;
        }

        public static bool IsGeometryEntity(ByteSpan bs)
            => IsGeometryEntity(bs.ToString());

        public static bool IsGeometryEntity(string e)
            => EntityToCodeLookup.ContainsKey(e);

        public static bool IsPropertyEntity(string str) 
            => PropertyEntities.Contains(str);
    }
}
