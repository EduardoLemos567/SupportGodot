using System.Collections.Generic;
using Godot;
using GodotArray = Godot.Collections.Array;
using GodotColor = Godot.Color;

namespace Support.MeshBuilder;

public class SurfaceArray
{
    public readonly struct VertexData
    {
        public Vector3 Vertex { get; init; }
        public Vector3 Normal { get; init; }
        public GodotColor Color { get; init; }
        public Vector2 Uv { get; init; }
    }
    private readonly List<VertexData> data;
    private readonly List<int> indices;
    public int VertexCount => data.Count;
    public int IndexCount => indices.Count;
    public SurfaceArray(int expectedVertices, int? expectedIndices = null)
    {
        expectedIndices ??= expectedVertices;
        data = new(expectedVertices);
        indices = new(expectedIndices.Value);
    }
    public int AddVertex(in VertexData vertexData)
    {
        data.Add(vertexData);
        return data.Count - 1;
    }
    public void SetVertex(int index, in VertexData vertexData)
    {
        data[index] = vertexData;
    }
    public VertexData GetVertex(int index)
    {
        return data[index];
    }
    public void AddTriangle(int indexA, int indexB, int indexC)
    {
        indices.Add(indexA);
        indices.Add(indexB);
        indices.Add(indexC);
    }
    private void RecalculateNormalFromTriangle(int firstIndexOfIndex)
    {
        int a = indices[firstIndexOfIndex];
        int b = indices[firstIndexOfIndex + 1];
        int c = indices[firstIndexOfIndex + 2];

        var da = data[a];
        var db = data[b];
        var dc = data[c];

        Vector3 edge1 = db.Vertex - da.Vertex;
        Vector3 edge2 = dc.Vertex - da.Vertex;
        Vector3 normal = edge1.Cross(edge2).Normalized();

        data[a] = da with { Normal = (da.Normal + normal).Normalized() };
        data[b] = db with { Normal = (db.Normal + normal).Normalized() };
        data[c] = dc with { Normal = (dc.Normal + normal).Normalized() };
    }
    public void RecalculateNormals()
    {
        for (int i = 0; i < indices.Count; i += 3)
        {
            RecalculateNormalFromTriangle(i);
        }
    }
    public GodotArray ToArray()
    {
        var surfaceArray = new GodotArray();
        var vertices = new Vector3[data.Count];
        var normals = new Vector3[data.Count];
        var colors = new GodotColor[data.Count];
        var uvs = new Vector2[data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            var d = data[i];
            vertices[i] = d.Vertex;
            normals[i] = d.Normal;
            colors[i] = d.Color;
            uvs[i] = d.Uv;
        }
        surfaceArray.Resize((int)Mesh.ArrayType.Max);
        surfaceArray[(int)Mesh.ArrayType.Vertex] = vertices;
        surfaceArray[(int)Mesh.ArrayType.Normal] = normals;
        surfaceArray[(int)Mesh.ArrayType.Color] = colors;
        surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs;
        surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();
        return surfaceArray;
    }
    public ArrayMesh AddSurfaceToArrayMesh(ArrayMesh? arrayMesh = null)
    {
        arrayMesh ??= new();
        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, ToArray());
        return arrayMesh;
    }
}