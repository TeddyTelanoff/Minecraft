using System.Collections.Generic;
using UnityEngine;

public class PlayerModifier : MonoBehaviour
{
    public float _reach;

    private Vector3Int _selectedBlock;

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _reach))
        {
            Vector3 point = hit.point + transform.forward * .01f;
            Chunk chunk = hit.transform.GetComponent<Chunk>();

            _selectedBlock = new Vector3Int(Mathf.FloorToInt(point.x + .5f), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z + .5f));

            int blockX = _selectedBlock.x % Chunk.ChunkSize.x;
            int blockY = _selectedBlock.y;
            int blockZ = _selectedBlock.z % Chunk.ChunkSize.z;

            if (blockX < 0)
                blockX += Chunk.ChunkSize.x;
            if (blockZ < 0)
                blockZ += Chunk.ChunkSize.z;

            if (Input.GetMouseButton(0))
            {
                Debug.Log($"Modifying Block ({blockX}, {blockY}, {blockZ}) From Point: {point}");

                chunk.DestroyBlock(blockX, blockY, blockZ);
                chunk.ReGenerateMesh();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(_selectedBlock, Vector3.one);
    }
}
