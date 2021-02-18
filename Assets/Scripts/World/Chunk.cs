using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static Vector3Int ChunkSize { get => new Vector3Int(3, 3, 3); }
    public Vector2Int _position;

    private Block[,,] _blocks = new Block[ChunkSize.x, ChunkSize.y, ChunkSize.z];

    public void ReGenerateMesh() => GenerateMesh();
    public void GenerateMesh()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();

        for (var x = 0; x < ChunkSize.x; x++)
            for (var y = 0; y < ChunkSize.y; y++)
                for (var z = 0; z < ChunkSize.z; z++)
                {
                    if (_blocks[x, y, z].BlockType == BlockType.Air)
                        continue;

                    // Front
                    int firstVert = vertices.Count;

                    vertices.Add(new Vector3(x + 0, y + 0, z));
                    vertices.Add(new Vector3(x + 1, y + 0, z));
                    vertices.Add(new Vector3(x + 1, y + 1, z));
                    vertices.Add(new Vector3(x + 0, y + 1, z));

                    triangles.Add(firstVert + 0);
                    triangles.Add(firstVert + 1);
                    triangles.Add(firstVert + 2);

                    triangles.Add(firstVert + 0);
                    triangles.Add(firstVert + 2);
                    triangles.Add(firstVert + 3);
                }

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
