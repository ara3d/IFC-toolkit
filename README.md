# Ara 3D IFC Toolkit

A set of open-source C# libraries (.NET 8.0) 
for extremely fast loading and querying of IFC files in STEP format.

A C++ DLL is built from a copy of source code taken from [Web IFC engine](https://github.com/ThatOpen/engine_web-ifc) to 
generate meshes from geometry definitions in the IFC files.

## Background

Speckle hired us (Ara 3D Inc) to improve the performance of their [IFC import service](https://github.com/specklesystems/speckle-server/blob/main/packages/fileimport-service/ifc/parser_v2.js), which was written in JavaScript and executed on the server using Node.JS. 

They were using the [Web IFC engine](https://github.com/ThatOpen/engine_web-ifc) to parse files, but were running into some time out issues with certain files. 

We originally started by wrapping the [Web IFC engine in a C++/CLR wrapper](https://github.com/ara3d/web-ifc-dotnet). This project was successful, in that it allowed us to build a new IFC importer in C#, but it was not cross-platform. 
C++/CLR unforuntately only runs on Windows.

Instead we decided to open-source our STEP file parser and combine it with a C++ DLL built on top of the Web IFC engine, for the purpose of constructing meshes from the geometry. 

## Solution Structure

This is the structure as it appears in the solution view in Visual Studio:  

* _sandbox_ - experiments and WIP projects 
  * **Ara3D.IFCBrower** - A Windows WPF application for viewing IFC properties
  * **Ara3D.IfcPropDB** - A library for storing properties in a simple in-memory database with serialization to/from disk
* _tests_
  * **Ara3D.IfcParser.Test** - An NUnit test for working with IFC files
* _testfiles_ - public IFC files copied from the Web IFC engine
* _schemas_ - Formal definitions of IFC schemas in express (.exp) files
* **Ara3D.IfcParser** - Creates an IFC graph containing nodes and relations.   
* **Ara3D.StepParser** - Loads and Parses a STEP file 
* **WebIfcDll** - A DLL that exposes functions for loading a file, and generating meshes from them.    

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

## Future Directions

We have not yet set up the project to build on Linux, this will be done in conjunction with the Speckle engineering team,
either in this repo, or in another one. 

In the longer term we will be integrating the [Plato.Geometry](https://github.com/ara3d/Plato.Geometry) to generate meshes,
at which point we will replace the dependency on the Web IFC Engine.  

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

Test files and geometry creation code provide by [Tom van Diggelen](https://github.com/tomvandig) and [That Open Company](https://github.com/ThatOpen/engine_web-ifc).

<image src="https://github.com/user-attachments/assets/443135ed-431e-4088-acf1-5a271d0c0e41"/>
<image src="https://github.com/user-attachments/assets/76431694-9005-4344-a8fa-3a993aaf50ed" width="200" valign="center"/>
<image src="https://github.com/user-attachments/assets/79298b1e-4765-42aa-b345-1e88d776694a"/>
<image src="https://github.com/user-attachments/assets/9e940db2-d496-4f8f-bb84-7f6ac5b2c15f"/>

