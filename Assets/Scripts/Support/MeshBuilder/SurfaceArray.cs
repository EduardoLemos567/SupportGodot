using System.Collections.Generic;
using Godot;
using GodotArray = Godot.Collections.Array;
using GodotColor = Godot.Color;

namespace Support.MeshBuilder
{
    public class SurfaceArray
    {
        private readonly List<Vector3> vertices;
        private readonly List<Vector3> normals;
        private readonly List<GodotColor> colors;
        private readonly List<Vector2> uvs;
        private readonly List<int> indices;
        public int VertexCount => vertices.Count;
        public int IndexCount => indices.Count;
        public SurfaceArray(int expectedVertices, int? expectedIndices = null)
        {
            expectedIndices ??= expectedVertices;
            vertices = new(expectedVertices);
            normals = new(expectedVertices);
            colors = new(expectedVertices);
            uvs = new(expectedVertices);
            indices = new(expectedIndices.Value);
        }
        public int AddVertex(Vector3 vertex, Vector3 normal = default, GodotColor color = default, Vector2 uv = default)
        {
            vertices.Add(vertex);
            normals.Add(normal);
            colors.Add(color);
            uvs.Add(uv);
            return vertices.Count - 1;
        }
        public void SetVertex(int index, Vector3 vertex, Vector3 normal = default, GodotColor color = default, Vector2 uv = default)
        {
            vertices[index] = vertex;
            normals[index] = normal;
            colors[index] = color;
            uvs[index] = uv;
        }
        public void AddTriangle(int indexA, int indexB, int indexC)
        {
            indices.Add(indexA);
            indices.Add(indexB);
            indices.Add(indexC);
        }
        private GodotArray ToArray()
        {
            var surfaceArray = new GodotArray();
            surfaceArray.Resize((int)Mesh.ArrayType.Max);
            surfaceArray[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
            surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
            surfaceArray[(int)Mesh.ArrayType.Color] = colors.ToArray();
            surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
            surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();
            return surfaceArray;
        }
        public ArrayMesh AddSurfaceToArrayMesh(ArrayMesh? arrayMesh = null)
        {
            arrayMesh ??= new ArrayMesh();
            arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, ToArray());
            return arrayMesh;
        }
    }
}