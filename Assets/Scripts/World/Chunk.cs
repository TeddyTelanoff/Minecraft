using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public static Vector3Int ChunkSize { get => new Vector3Int(16, 16, 16); }
	public Vector2Int _position;

	public Block[,,] _blocks = new Block[ChunkSize.x, ChunkSize.y, ChunkSize.z];

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
					if (z <= 0 || _blocks[x, y, z - 1].BlockType == BlockType.Air)
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 0), Vector3.up, Vector3.right, false);
					// Back
					if (z >= ChunkSize.z - 1 || _blocks[x, y, z + 1].BlockType == BlockType.Air)
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 1), Vector3.up, Vector3.right, true);

					// Right
					if (x <= 0 || _blocks[x - 1, y, z].BlockType == BlockType.Air)
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 0), Vector3.up, Vector3.forward, true);
					// Left
					if (x >= ChunkSize.x - 1 || _blocks[x + 1, y, z].BlockType == BlockType.Air)
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 1, y + 0, z + 0), Vector3.up, Vector3.forward, false);

					// Top
					if (y <= 0 || _blocks[x, y - 1, z].BlockType == BlockType.Air)
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 0), Vector3.forward, Vector3.right, true);
					// Bottom
					if (y >= ChunkSize.x - 1 || _blocks[x, y + 1, z].BlockType == BlockType.Air)
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 1, z + 0), Vector3.forward, Vector3.right, false);
				}

		var mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		// Debug.Log($"Triangles[{triangles.Count}], Mesh Triangles[{mesh.triangles.Length}]");

		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	private void BuildMesh(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, Vector3 pos, Vector3 up, Vector3 right, bool reverse)
	{
		int firstVert = vertices.Count;
		int irev = reverse ? 1 : 0;

		vertices.Add(pos + up);
		vertices.Add(pos + up + right);
		vertices.Add(pos + right);
		vertices.Add(pos);

		triangles.Add(firstVert + 0);
		triangles.Add(firstVert + 1 + irev);
		triangles.Add(firstVert + 2 - irev);

		triangles.Add(firstVert + 0);
		triangles.Add(firstVert + 2 + irev);
		triangles.Add(firstVert + 3 - irev);
	}
}
