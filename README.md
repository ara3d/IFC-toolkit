# Ara 3D IFC Toolkit

A set of open-source C# libraries (.NET 8.0) for loading and querying IFC files in STEP format.

A DLL built from source code taken from [Web IFC engine](https://github.com/ThatOpen/engine_web-ifc) is used to 
generate meshes. 

## Status

We are just finishing up porting and merging code from two earlier prototypes: 
   1) a private implementation of a C# parser
   2) a C++/CLR wrapper around the Web IFC engine. 

Parsing IFC files without creating meshes is very fast. Mesh creation is currently a bottleneck. 

Temporarily compiling this solution requires it to be built as part of the Ara3D mono-repository. 

## Background

Speckle hired us ([Ara 3D Inc](https://ara3d.com)) to improve the performance of their 
[IFC import service](https://github.com/specklesystems/speckle-server/blob/main/packages/fileimport-service/ifc/parser_v2.js). 
which was written in JavaScript and executed on the server using Node.JS and was using the [Web IFC engine](https://github.com/ThatOpen/engine_web-ifc) 
to parse files and generate geometry. 

We originally started by wrapping the [Web IFC engine in a C++/CLR wrapper](https://github.com/ara3d/web-ifc-dotnet). 
This project was successful, in that it allowed us to build a new IFC importer in C#, but it was not cross-platform. 
C++/CLR unforuntately only runs on Windows. It is now deprecated and replaced with this project.

Thanks to the funding from Speckle, at Ara 3D we decided to open-source our IFC/STEP file parser and combine it with a C++ DLL built on top of the Web IFC engine.

Our next step is to generate meshes directly in C# using the [Plato.Geometry](https://github.com/ara3d/Plato.Geometry) library. 

## Solution Structure

This is the structure as it appears in the solution view in Visual Studio:  

* **Ara3D.IfcLoader** 
    * A C# .NET 8 solution that depends on the WebIfcDll output DLL being in the executable folder.  
    * Combines the Ara3D.IfcParser to load the IFC graph and STEP document, and the WebIfcDll to generate meshes.
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
    * **Ara3D.Speckle** 
        - WIP: A C# .NET 8 library of utilities for working with Speckle data 
    * **Ara3D.Speckle.IfcLoader** 
        - WIP: A C# .NET 8 library for loading IFC files and generating Speckle objects 
* **sandbox/** 
    * **Ara3D.IFCBrower** 
        - An experiemental Windows WPF application for viewing IFC properties
    * **Ara3D.IfcPropDB** 
        - An experimental library for storing properties in a simple in-memory database with serialization to/from disk
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
* `::Geometry* GetGeometry(Api* api, Model* model, uint32_t id);`
* `int GetNumMeshes(Api* api, ::Geometry* geom);`
* `Mesh* GetMesh(Api* api, ::Geometry* geom, int index);`
* `double* GetTransform(Api* api, Mesh* mesh);`
* `double* GetColor(Api* api, Mesh* mesh);`
* `int GetNumVertices(Api* api, Mesh* mesh);`
* `Vertex* GetVertices(Api* api, Mesh* mesh);`
* `int GetNumIndices(Api* api, Mesh* mesh);`
* `uint32_t* GetIndices(Api* api, Mesh* mesh);`

If an element has no associated geometry then GetGeometry will return null. 

## Next Steps 

- [ ] Be able to compile project outside of Ara 3D main repo 
- [ ] Assure that all Ara 3D dependencies are taken from NuGet packages (not local)
- [ ] Make a separate C++ project for the DLL for Linux
- [ ] Test the C# and C++ code on Linux
- [ ] Extract Global IDs from all product elements 
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

