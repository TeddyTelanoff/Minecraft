﻿//#define DRAW_CHUNK_EDGES
//#define DONT_DRAW_CHUNK_EDGES

using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public static Vector3Int ChunkSize { get => new Vector3Int(16, 16, 16); }
	public Vector2Int _position;
	public World _world;

	public Block[,,] _blocks = new Block[ChunkSize.x, ChunkSize.y, ChunkSize.z];

	private Mesh _mesh;

    private void Awake()
    {
		_mesh = new Mesh();
		GetComponent<MeshFilter>().sharedMesh = _mesh;
		GetComponent<MeshCollider>().sharedMesh = _mesh;
	}

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
#if DRAW_CHUNK_EDGES
					if (z <= 0 || _blocks[x, y, z - 1].BlockType == BlockType.Air)
#elif DONT_DRAW_CHUNK_EDGES
					if (z > 0 && _blocks[x, y, z - 1].BlockType == BlockType.Air)
#else
					if (IsTransparent(x, y, z - 1))
#endif
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 0), Vector3.up, Vector3.right, false);
					// Back
#if DRAW_CHUNK_EDGES
					if (z >= ChunkSize.z - 1 || _blocks[x, y, z + 1].BlockType == BlockType.Air)
#elif DONT_DRAW_CHUNK_EDGES
					if (z < ChunkSize.z - 1 && _blocks[x, y, z + 1].BlockType == BlockType.Air)
#else
					if (IsTransparent(x, y, z + 1))
#endif
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 1), Vector3.up, Vector3.right, true);

					// Right
#if DRAW_CHUNK_EDGES
					if (x <= 0 || _blocks[x - 1, y, z].BlockType == BlockType.Air)
#elif DONT_DRAW_CHUNK_EDGES
					if (x > 0 && _blocks[x - 1, y, z].BlockType == BlockType.Air)
#else
					if (IsTransparent(x - 1, y, z))
#endif
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 0), Vector3.up, Vector3.forward, true);
					// Left
#if DRAW_CHUNK_EDGES
					if (x >= ChunkSize.x - 1 || _blocks[x + 1, y, z].BlockType == BlockType.Air)
#elif DONT_DRAW_CHUNK_EDGES
					if (x < ChunkSize.x - 1 && _blocks[x + 1, y, z].BlockType == BlockType.Air)
#else
					if (IsTransparent(x + 1, y, z))
#endif
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 1, y + 0, z + 0), Vector3.up, Vector3.forward, false);

					// Top
#if DRAW_CHUNK_EDGES
					if (y <= 0 || _blocks[x, y - 1, z].BlockType == BlockType.Air)
#elif DONT_DRAW_CHUNK_EDGES
					if (y > 0 && _blocks[x, y - 1, z].BlockType == BlockType.Air)
#else
					if (IsTransparent(x, y - 1, z))
#endif
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 0, z + 0), Vector3.forward, Vector3.right, true);
					// Bottom
#if DRAW_CHUNK_EDGES
					if (y >= ChunkSize.y - 1 || _blocks[x, y + 1, z].BlockType == BlockType.Air)
#elif DONT_DRAW_CHUNK_EDGES
					if (y < ChunkSize.y - 1 && _blocks[x, y + 1, z].BlockType == BlockType.Air)
#else
					if (IsTransparent(x, y + 1, z))
#endif
						BuildMesh(ref vertices, ref triangles, ref uvs, new Vector3(x + 0, y + 1, z + 0), Vector3.forward, Vector3.right, false);
				}

		_mesh.Clear();
		_mesh.vertices = vertices.ToArray();
		_mesh.triangles = triangles.ToArray();
		_mesh.uv = uvs.ToArray();
		_mesh.RecalculateBounds();
		_mesh.RecalculateNormals();
		_mesh.RecalculateTangents();

		// Debug.Log($"Triangles[{triangles.Count}], Mesh Triangles[{mesh.triangles.Length}]");

		GetComponent<MeshFilter>().sharedMesh = _mesh;
		GetComponent<MeshCollider>().sharedMesh = _mesh;
	}

	public void DestroyBlock(int x, int y, int z) =>
		_blocks[x, y, z] = new Block { BlockType = BlockType.Air };

	private void BuildMesh(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, Vector3 pos, Vector3 up, Vector3 right, bool reverse)
	{
		Vector3 half = Vector3.one / 2;
		int firstVert = vertices.Count;
		int irev = reverse ? 1 : 0;

		vertices.Add(pos + up + half);
		vertices.Add(pos + up + right + half);
		vertices.Add(pos + right + half);
		vertices.Add(pos + half);

		triangles.Add(firstVert + 0);
		triangles.Add(firstVert + 1 + irev);
		triangles.Add(firstVert + 2 - irev);

		triangles.Add(firstVert + 0);
		triangles.Add(firstVert + 2 + irev);
		triangles.Add(firstVert + 3 - irev);
	}

	private bool IsTransparent(int relX, int relY, int relZ)
	{
		Vector2Int chunkPos = _position;

		if (relX < 0)
		{
			chunkPos.x--;
			relX = ChunkSize.x - 1;
		}
		if (relZ < 0)
		{
			chunkPos.y--;
			relZ = ChunkSize.z - 1;
		}

		if (relX >= ChunkSize.x)
		{
			chunkPos.x++;
			relX = 0;
		}
		if (relZ >= ChunkSize.z)
		{
			chunkPos.y++;
			relZ = 0;
		}

		return _world.IsTransparent(chunkPos, new Vector3Int(relX, relY, relZ));
	}
}