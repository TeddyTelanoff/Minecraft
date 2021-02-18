using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	private Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();

    private void Start() => GenerateChunk(new Vector2Int(0, 0));

    public void GenerateChunk(Vector2Int pos)
    {
        Debug.Assert(!_chunks.ContainsKey(pos), $"Chunk Already Generated at [{pos.x}, {pos.y}]");

        var chunk = new GameObject($"Chunk ({pos.x}, {pos.y})");
        chunk.transform.position = new Vector3(pos.x * Chunk.ChunkSize.x, 0, pos.y * Chunk.ChunkSize.z);
        Chunk chunkComponent = chunk.AddComponent<Chunk>();
        chunk.AddComponent<MeshFilter>();
        chunk.AddComponent<MeshRenderer>();
        chunk.AddComponent<MeshCollider>();

        chunkComponent._position = pos;
        chunkComponent.GenerateMesh();
    }
}