// Apache 2.0 License
// Author: Christopher Diggins of Ara 3D Inc for Speckle Systems Ltd.
// This is a C++ wrapper around the Web-IFC component library by Tom van Diggelen and That Open Company 
// that is appropriate for use via PInvoke
// It is built based on the specific needs of a Speckle IFC import service:
// https://github.com/specklesystems/speckle-server/blob/main/packages/fileimport-service/ifc/parser_v2.js#L26
//
// And was inspired by:
// - https://github.com/ThatOpen/engine_web-ifc/blob/main/src/ts/web-ifc-api.ts
// - https://github.com/ThatOpen/engine_web-ifc/blob/main/src/cpp/web-ifc-wasm.cpp
// - https://github.com/ThatOpen/engine_web-ifc/blob/main/src/cpp/web-ifc-test.cpp

#include <string>
#include <algorithm>
#include <vector>
#include <stack>
#include <cstdint>
#include <memory>
#include "../engine_web-ifc/src/cpp/modelmanager/ModelManager.h"
#include "../engine_web-ifc/src/cpp/version.h"
#include <iostream>
#include <fstream>

using namespace webifc::manager;
using namespace webifc::parsing;
using namespace webifc::geometry;
using namespace webifc::schema;

// Forward declarations of classes
class Model;
class Api;
class Mesh;
class Geometry;
class Vertex;

// Exposed C functions 
extern "C"
{
    __declspec(dllexport) Api* InitializeApi();
    __declspec(dllexport) void FinalizeApi(Api* api);
    __declspec(dllexport) Model* LoadModel(Api* api, const char* fileName);
    __declspec(dllexport) ::Geometry* GetGeometryFromId(Api* api, Model* model, uint32_t id);
    __declspec(dllexport) int GetNumGeometries(Api* api, Model* model);
    __declspec(dllexport) ::Geometry* GetGeometryFromIndex(Api* api, Model* model, int32_t index);
    __declspec(dllexport) int GetNumMeshes(Api* api, ::Geometry* geom);
    __declspec(dllexport) uint32_t GetGeometryId(Api* api, ::Geometry* geom);
    __declspec(dllexport) Mesh* GetMesh(Api* api, ::Geometry* geom, int index);
    __declspec(dllexport) uint32_t GetMeshId(Api* api, ::Mesh* mesh);
    __declspec(dllexport) double* GetTransform(Api* api, Mesh* mesh);
    __declspec(dllexport) double* GetColor(Api* api, Mesh* mesh);
    __declspec(dllexport) int GetNumVertices(Api* api, Mesh* mesh);
    __declspec(dllexport) Vertex* GetVertices(Api* api, Mesh* mesh);
    __declspec(dllexport) int GetNumIndices(Api* api, Mesh* mesh);
    __declspec(dllexport) uint32_t* GetIndices(Api* api, Mesh* mesh);
}

// Vertex data structure as used by the web-IFC engine
struct Vertex 
{
    double Vx, Vy, Vz;
    double Nx, Ny, Nz;
};

// Color data
struct Color 
{
    double R, G, B, A;
    Color() : R(0), G(0), B(0), A(0) {}
    Color(double r, double g, double b, double a)
        : R(r), G(g), B(b), A(a) {}
};

struct Mesh 
{
    IfcGeometry* geometry;
    Color color;
    uint32_t id;
    std::array<double, 16> transform;
    Mesh(uint32_t id) 
        : geometry(nullptr), id(id), transform({}), color() 
    { }
};

struct Geometry 
{
    uint32_t id;
    IfcFlatMesh* flatMesh;
    std::vector<Mesh*> meshes;    
    Geometry(uint32_t id)
        : id(id), flatMesh(nullptr) 
    {}
};

// Model class, abstraction over the web-IFC engine concept of Model ID
struct Model
{
    uint32_t id;
    IfcLoader* loader;
    IfcGeometryProcessor* geometryProcessor;
    std::vector<::Geometry*> geometryList;
    std::unordered_map<uint32_t, ::Geometry*> geometries;

    Model(IfcSchemaManager* schemas, IfcLoader* loader, IfcGeometryProcessor* processor, uint32_t id)
        : loader(loader), geometryProcessor(processor), id(id)
    {
        for (auto type : schemas->GetIfcElementList())
        {
            // TODO: maybe some of these elments are desired. In fact, I think there may be explicit requests for IFCSPACE?
            if (type == IFCOPENINGELEMENT
                || type == IFCSPACE
                || type == IFCOPENINGSTANDARDCASE)
            {
                continue;
            }

            for (auto eId : loader->GetExpressIDsWithType(type))
            {
                auto flatMesh = geometryProcessor->GetFlatMesh(eId);
                auto g = new ::Geometry(eId);
                for (auto& placedGeom : flatMesh.geometries)
                {
                    auto mesh = ToMesh(placedGeom);
                    g->meshes.push_back(mesh);
                }
                geometries[eId] = g;
                geometryList.push_back(g);
            }
        }        
    }

    ::Geometry* GetGeometry(uint32_t id)
    {
        auto it = geometries.find(id);
        if (it == geometries.end())
            return nullptr;
        return it->second;
    }

    Mesh* ToMesh(IfcPlacedGeometry& pg) 
    {
        auto r = new Mesh(pg.geometryExpressID);
        r->color = Color(pg.color.r, pg.color.g, pg.color.b, pg.color.a);
        r->geometry = &(geometryProcessor->GetGeometry(pg.geometryExpressID));
        r->transform = pg.flatTransformation;
        return r;
    }
};

struct Api 
{
    ModelManager* manager;
    IfcSchemaManager* schemaManager;
    LoaderSettings* settings;

    Api() 
    {
        schemaManager = new IfcSchemaManager();
        manager = new ModelManager(false);
        manager->SetLogLevel(6); // Turns off logging
        settings = new webifc::manager::LoaderSettings();
    }   

    Model* LoadModel(const char* fileName)
    {
        auto modelId = manager->CreateModel(*settings);
        auto loader = manager->GetIfcLoader(modelId);
        std::ifstream ifs;
        // NOTE: may fail if the file has unicode characters. This needs to be tested  
        ifs.open(fileName, std::ifstream::in);
        loader->LoadFile(ifs);
        return new ::Model(schemaManager, loader, manager->GetGeometryProcessor(modelId), modelId);
    }
};

//==

Api* InitializeApi() {
    return new Api();
}

void FinalizeApi(Api* api) {
    delete api->manager;
    delete api->schemaManager;
    delete api->settings;
    delete api;
}

Model* LoadModel(Api* api, const char* fileName) {
    return api->LoadModel(fileName);
}

double* GetTransform(Api* api, Mesh* mesh) {
    return mesh->transform.data();
}

double* GetColor(Api* api, Mesh* mesh) {
    return &mesh->color.R;
}

::Geometry* GetGeometryFromId(Api* api, Model* model, uint32_t id) {
    return model->GetGeometry(id);
}

int GetNumGeometries(Api* api, Model* model) {
    return model->geometries.size();
}

::Geometry* GetGeometryFromIndex(Api* api, Model* model, int32_t index) {
    return model->geometryList[index];
}

uint32_t GetGeometryId(Api* api, ::Geometry* geom) {
    return geom->id;
}

int GetNumMeshes(Api* api, ::Geometry* geom) {
    return geom->meshes.size();
}

Mesh* GetMesh(Api* api, ::Geometry* geom, int index) {
    return geom->meshes[index];
}

uint32_t GetMeshId(Api* api, ::Mesh* mesh) {
    return mesh->id;
}

int GetNumVertices(Api* api, Mesh* mesh) {
    return mesh->geometry->vertexData.size() / 6;
}

Vertex* GetVertices(Api* api, Mesh* mesh) {
    return reinterpret_cast<Vertex*>(mesh->geometry->vertexData.data());
}

int GetNumIndices(Api* api, Mesh* mesh) {
    return mesh->geometry->indexData.size();
}

uint32_t* GetIndices(Api* api, Mesh* mesh) {
    return mesh->geometry->indexData.data();
}
