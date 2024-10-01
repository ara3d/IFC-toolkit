using System.Reflection;
using Ara3D.Logging;
using Speckle.Core.Models;
using Objects.Geometry;
using Objects.Other;
using WebIfcClrWrapper;
using WebIfcDotNet;
using Color = System.Drawing.Color;
using Mesh = Objects.Geometry.Mesh;

namespace WebIfcDotNetTests;

public static class IfcModelGraphToSpeckle
{
    public static Base ToSpeckle(this ModelGraph g)
    {
        var b = new Base();
        b["Name"] = "Root";
        var children = g.GetSources().Select(ToSpeckle).ToList();
        b["elements"] = children;
        return b;
    }

    public static unsafe Mesh ToSpeckle(this TransformedMesh tm)
    {
        var r = new Mesh();
        var vertexData = tm.Mesh.GetVertexData();
        var indexData = tm.Mesh.GetIndexData();
        var m = tm.Transform;
        var vp = (double*)vertexData.DataPtr.ToPointer();
        var ip = (int*)indexData.DataPtr.ToPointer();
        
        for (var i=0; i < vertexData.Count; i += 6)
        {
            var x = vp[i];
            var y = vp[i + 1];
            var z = vp[i + 2];
            r.vertices.Add(m[0] * x + m[4] * y + m[8] * z + m[12]);
            r.vertices.Add(-(m[2] * x + m[6] * y + m[10] * z + m[14]));
            r.vertices.Add(m[1] * x + m[5] * y + m[9] * z + m[13]);
        }

        for (var i = 0; i < indexData.Count; i += 3)
        {
            var a = ip[i];
            var b = ip[i + 1];
            var c = ip[i + 2];
            r.faces.Add(0);
            r.faces.Add(a);
            r.faces.Add(b);
            r.faces.Add(c);
        }

        var rm = new RenderMaterial();
        rm.diffuseColor = Color.FromArgb((int)(tm.Color.A * 255), (int)(tm.Color.R * 255), (int)(tm.Color.G * 255), (int)(tm.Color.B * 255));
        r["renderMaterial"] = rm;
        return r;
    }

    public static Collection ToSpeckle(this Geometry geometry)
    {
        var c = new Collection();
        foreach (var tm in geometry.Meshes ?? [])
            c.elements.Add(tm.ToSpeckle());
        return c;
    }

    public static object IfcValJsonObject(this object obj)
    {
        switch (obj)
        {
            case List<object> list:
                return list.Select(IfcValJsonObject).ToList();
            case LabelValue lv:
                return IfcValJsonObject(lv.Arguments.Count == 1 
                    ? lv.Arguments[0] 
                    : lv.Arguments);
            case EnumValue ev:  
                return ev.Name;
            case RefValue rv:
                return rv.ExpressId;
            case string s:
                return s;
            case null:
                return null;
            default:
                return obj;
        }
    }

    public static Dictionary<string, object> ToSpeckleDictionary(this ModelPropSet ps)
    {
        var d = new Dictionary<string, object>();
        foreach (var p in ps.Properties)
            d[p.Name] = p.Value.IfcValJsonObject();
        return d;
    }

    public static Base ToSpeckle(this ModelNode n)
    {
        var b = new Base();
        if (n is ModelPropSet ps)
        {
            b["Name"] = ps.Name;
            b["GlobalId"] = ps.Guid;
        }

        // https://github.com/specklesystems/speckle-server/issues/1180
        b["ifc_type"] = n.Type;

        // This is required because "speckle_type" has no setter, but is backed by a private field.  
        var baseType = typeof(Base);
        var typeField = baseType.GetField("_type", BindingFlags.Instance | BindingFlags.NonPublic);
        typeField?.SetValue(b, n.Type);

        b["expressID"] = n.Id;

        if (n.Graph.Geometries.TryGetValue(n.Id, out var m))
        {
            var c = m.ToSpeckle();
            if (c.elements.Count > 0)
                b["displayValue"] = c.elements;
        }

        var children = n.GetChildren().Select(ToSpeckle).ToList();
        b["elements"] = children;

        foreach (var p in n.GetPropSets())
            b[p.Name] = p.ToSpeckleDictionary();

        return b;
    }
}