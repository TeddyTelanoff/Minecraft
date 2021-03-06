﻿using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public Vector2Int[] _chunksToGenerate;
	public GameObject _chunkPrefab;
	public float _heightStep;

	public Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();
	private void Start()
	{
		foreach (var chunkToGenerate in _chunksToGenerate)
			GenerateChunk(chunkToGenerate);
		foreach (var chunk in _chunks.Values)
			chunk.ReGenerateMesh();
	}

	public void GenerateChunk(Vector2Int pos)
	{
		Debug.Assert(!_chunks.ContainsKey(pos), $"Chunk Already Generated at [{pos.x}, {pos.y}]");

		var chunk = Instantiate(_chunkPrefab, new Vector3(pos.x * Chunk.ChunkSize.x, 0, pos.y * Chunk.ChunkSize.z), Quaternion.identity);
		chunk.name = $"Chunk ({pos.x}, {pos.y})";
		Chunk chunkComponent = chunk.GetComponent<Chunk>();
		chunkComponent._position = pos;
		chunkComponent._world = this;

		for (var x = 0; x < Chunk.ChunkSize.x; x++)
			for (var z = 0; z < Chunk.ChunkSize.z; z++)
			{
				int height = Mathf.FloorToInt(Mathf.PerlinNoise((pos.x * Chunk.ChunkSize.x + x) * _heightStep, (pos.y * Chunk.ChunkSize.z + z) * _heightStep) * Chunk.ChunkSize.y);
				for (var y = 0; y < Chunk.ChunkSize.y; y++)
					chunkComponent._blocks[x, y, z] = new Block { BlockType = y < height ? BlockType.Grass : BlockType.Air };
			}

		_chunks.Add(pos, chunkComponent);
		//chunkComponent.GenerateMesh();
	}

	public bool IsTransparent(Vector2Int chunkPos, Vector3Int blockPos)
    {
		if (blockPos.y < 0 || blockPos.y >= Chunk.ChunkSize.y)
			return true;
		else if (_chunks.TryGetValue(chunkPos, out Chunk chunk))
		{
			//Debug.Log($"Chunk ({chunkPos.x}, {chunkPos.y}) Exists");
			return chunk._blocks[blockPos.x, blockPos.y, blockPos.z].BlockType == BlockType.Air;
		}
		else
		{
			//Debug.Log($"Chunk ({chunkPos.x}, {chunkPos.y}) Does Not Exists");
			return true;
		}
    }
}