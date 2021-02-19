using System.Collections.Generic;
using UnityEngine;

public class PlayerModifier : MonoBehaviour
{
    public float _reach;

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _reach))
            {
                Vector3 point = hit.point + transform.forward * .01f;
                Chunk chunk = hit.transform.GetComponent<Chunk>();

                int blockX = Mathf.FloorToInt(point.x % Chunk.ChunkSize.x);
                int blockY = Mathf.FloorToInt(point.y);
                int blockZ = Mathf.FloorToInt(point.z % Chunk.ChunkSize.z);

                if (blockX < 0)
                    blockX += Chunk.ChunkSize.x;
                if (blockZ < 0)
                    blockZ += Chunk.ChunkSize.z;

                Debug.Log($"Modifying Block ({blockX}, {blockY}, {blockZ}) From Point: {point}");

                chunk._blocks[blockX, blockY, blockZ].BlockType = BlockType.Air;
                chunk.ReGenerateMesh();
            }
        }
    }
}
