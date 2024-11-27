# Ara 3D IFC Toolkit

A set of open-source C# libraries (.NET 8.0) for loading and querying IFC files in STEP format
and a C++ DLL built from source code copied from [Web IFC engine](https://github.com/ThatOpen/engine_web-ifc) 
for generating meshes. 

Parsing IFC files without creating meshes is very fast. Mesh creation is currently a bottleneck. 

## Background

Speckle hired us ([Ara 3D Inc](https://ara3d.com)) to improve the performance of their 
[IFC import service](https://github.com/specklesystems/speckle-server/blob/main/packages/fileimport-service/ifc/parser_v2.js). 
which was written in JavaScript and executed on the server using Node.JS and was using the [Web IFC engine](https://github.com/ThatOpen/engine_web-ifc) 
to parse files and generate geometry. 

We originally started by wrapping the [Web IFC engine in a C++/CLR wrapper](https://github.com/ara3d/web-ifc-dotnet). 
This project was successful, in that it allowed us to build a new IFC importer in C#, but it was not cross-platform. 
C++/CLR unforuntately only runs on Windows. It is now deprecated and replaced with this project.

Thanks to the funding from Speckle, at Ara 3D we decided to open-source our IFC/STEP file parser and combine it with a 
C++ DLL built on top of the Web IFC engine.

## Next Steps 

1. [Short term] Speckle is going to start work on assuring that the code runs on Linux. 
2. [Medium term] Ara 3D future plans will replace the mesh code in C# using the [Plato.Geometry](https://github.com/ara3d/Plato.Geometry) library. 

## Solution Structure

This is the structure as it appears in the solution view in Visual Studio:  

* **Ara3D.IfcLoader** 
    * A C# .NET 8 project with a dependency on a Windows DLL
    * Uses Ara3D.IfcParser to load the IFC graph and STEP document, uses the WebIfcDll to generate meshes.
    * Provides additional helper classes that wrap the WebIfcDll and provide a more C# friendly API.
* **Ara3D.IfcParser** 
    * A C# .NET 8 library 
    * Creates an IFC graph containing nodes and relations, and IFC data structures. It uses the Ara3D.StepParser.    
* **Ara3D.StepParser** 
    * A C# .NET 8 library 
    * Loads and Parses an arbitrary IFC/STEP file. Parsing of individual entity values is done on demand. 
* **WebIfcDll** 
    * A C++ DLL that exposes functions for loading a file, and generates meshes from them. 
    * Built from a [snapshot of the Web IFC engine](https://github.com/ThatOpen/engine_web-ifc).
* **speckle/**
    * **Ara3D.Speckle.Data** 
        - A C# .NET 8 library of utilities for working with Speckle data 
    * **Ara3D.Speckle.IfcLoader** 
        - A C# .NET 8 library for loading IFC files and generating Speckle objects 
* **tests/**
    * **Ara3D.IfcParser.Test** 
        - An NUnit test project for working with IFC files and comparing different libraries 
* **testfiles/** 
    * test IFC files copied from the Web IFC engine repository 
* **schemas/** 
    * Formal definitions of IFC schemas in express (.exp) files

## C++ DLL API

The public API of the C++ DLL, which is restricted to extracting gometry is as follows:

* `Api* InitializeApi();`
* `void FinalizeApi(Api* api);`
* `Model* LoadModel(Api* api, const char* fileName);`
* `::Geometry* GetGeometryFromId(Api* api, Model* model, uint32_t id);`
* `::Geometry* GetGeometryFromIndex(Api* api, Model* model, int index);`
* `int GetNumGeometries(Api* api, Model* model);`
* `int GetNumMeshes(Api* api, ::Geometry* geom);`
* `Mesh* GetMesh(Api* api, ::Geometry* geom, int index);`
* `double* GetTransform(Api* api, Mesh* mesh);`
* `double* GetColor(Api* api, Mesh* mesh);`
* `int GetNumVertices(Api* api, Mesh* mesh);`
* `Vertex* GetVertices(Api* api, Mesh* mesh);`
* `int GetNumIndices(Api* api, Mesh* mesh);`
* `uint32_t* GetIndices(Api* api, Mesh* mesh);`

See the file `Ara3D.IfcLoader/WebIfcDll.cs` for the C# PInvoke code.

## Next Steps 

- [x] Be able to compile project outside of Ara 3D main repo 
- [x] Assure that all Ara 3D dependencies are taken from NuGet packages (not local)
- [x] Make a separate C++ project for the DLL for Linux
- [ ] Test the C# and C++ code on Linux
- [x] Extract Global IDs from all product elements 
- [ ] Create a list of Express IDs that represent product elements 
- [ ] Testing: Convert loaded IFC file to OBJ files 
- [ ] Testing: Convert Speckle objects to OBJ files
- [ ] Testing: Convert IFC to local Speckle objects
- [ ] Testing: Automate pushing IFC files to Speckle project

C# to Plato.Geometry

- [ ] Fill out IFC geometry structures using Plato.Geometry 
- [ ] Compute bounding boxes 
- [ ] Visualize bounding boxes, curves, points, alignments  
- [ ] Visualize polygon meshes 
- [ ] Implement CSG operations 
- [ ] Implement curve/profile to surface operations 

# WPF Viewer

There is a WPF 3D viewer that leverage the IFC library at [https://github.com/ara3d/ara3d/tree/main/labs/Ara3D.Viewer.Wpf](https://github.com/ara3d/ara3d/tree/main/labs/Ara3D.Viewer.Wpf) 

![image](https://github.com/user-attachments/assets/15a7f44f-a7fa-4c48-ada0-5bb0891318f1)

# Related Open-Source Work 

## C# Projects 

* [XBim](https://github.com/xBimTeam/XbimEssentials)
* [GeometryGymIFC](https://github.com/GeometryGym/GeometryGymIFC)
* [Hypar IFC-gen](https://github.com/hypar-io/ifc-gen)
* [Web IFC .NET](https://github.com/ara3d/web-ifc-dotnet) 

## Other Languages 

* [Web IFC](https://github.com/ThatOpen/engine_web-ifc) - C++ with JavaScript bindings
* [BIM Server](https://github.com/opensourceBIM/BIMserver) - Java
* [IfcOpenShell](https://ifcopenshell.org/) - C++ with Python bindings

# Acknowledgements 

This work was developed by [Ara 3D](https://ara3d.com) and funded by [Speckle Systems](https://speckle.systems/) 
with additional guidance and test data provided by [Impararia Solutions](https://www.impararia.com/).
Test files and geometry creation code provide by [Tom van Diggelen](https://github.com/tomvandig) 
and [That Open Company](https://github.com/ThatOpen/engine_web-ifc).

<image src="https://github.com/user-attachments/assets/443135ed-431e-4088-acf1-5a271d0c0e41"/>
<image src="https://github.com/user-attachments/assets/76431694-9005-4344-a8fa-3a993aaf50ed" width="200" valign="center"/>
<image src="https://github.com/user-attachments/assets/79298b1e-4765-42aa-b345-1e88d776694a"/>
<image src="https://github.com/user-attachments/assets/9e940db2-d496-4f8f-bb84-7f6ac5b2c15f"/>

