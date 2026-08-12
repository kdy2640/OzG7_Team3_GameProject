using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GridChunkHandler : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float chunkSize = 5f;
    [SerializeField] private bool showGridGizmos = true;

    private readonly Dictionary<Vector2Int, GridChunk> chunks = new();
    private readonly Dictionary<Transform, Vector2Int> registeredChunks = new();

    private sealed class GridChunk
    {
        public readonly List<Transform> Transforms = new();
    }

    public float ChunkSize => Mathf.Max(0.01f, chunkSize);

    public Vector2Int GetChunkCoordinate(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        return new Vector2Int(
            Mathf.FloorToInt(localPosition.x / ChunkSize),
            Mathf.FloorToInt(localPosition.z / ChunkSize));
    }

    public void Register(Transform target)
    {
        if (target == null)
        {
            return;
        }

        if (registeredChunks.ContainsKey(target))
        {
            UpdateChunk(target);
            return;
        }

        Vector2Int coordinate = GetChunkCoordinate(target.position);
        GetOrCreateChunk(coordinate).Transforms.Add(target);
        registeredChunks.Add(target, coordinate);
    }

    public void UpdateChunk(Transform target)
    {
        if (target == null)
        {
            return;
        }

        if (!registeredChunks.TryGetValue(target, out Vector2Int currentCoordinate))
        {
            Register(target);
            return;
        }

        Vector2Int nextCoordinate = GetChunkCoordinate(target.position);

        if (currentCoordinate == nextCoordinate)
        {
            return;
        }

        RemoveFromChunk(target, currentCoordinate);
        GetOrCreateChunk(nextCoordinate).Transforms.Add(target);
        registeredChunks[target] = nextCoordinate;
    }

    public void Unregister(Transform target)
    {
        if (target == null
            || !registeredChunks.TryGetValue(target, out Vector2Int coordinate))
        {
            return;
        }

        RemoveFromChunk(target, coordinate);
        registeredChunks.Remove(target);
    }

    public List<Transform> GetNearbyTransforms(
        Vector3 worldPosition,
        float range)
    {
        List<Transform> results = new();
        float safeRange = Mathf.Max(0f, range);
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        Vector2Int minCoordinate = GetChunkCoordinate(
            transform.TransformPoint(
                localPosition + new Vector3(-safeRange, 0f, -safeRange)));
        Vector2Int maxCoordinate = GetChunkCoordinate(
            transform.TransformPoint(
                localPosition + new Vector3(safeRange, 0f, safeRange)));
        float sqrRange = safeRange * safeRange;

        for (int z = minCoordinate.y; z <= maxCoordinate.y; z++)
        {
            for (int x = minCoordinate.x; x <= maxCoordinate.x; x++)
            {
                Vector2Int coordinate = new(x, z);

                if (!chunks.TryGetValue(coordinate, out GridChunk chunk))
                {
                    continue;
                }

                for (int i = chunk.Transforms.Count - 1; i >= 0; i--)
                {
                    Transform target = chunk.Transforms[i];

                    if (target == null)
                    {
                        chunk.Transforms.RemoveAt(i);
                        continue;
                    }

                    Vector3 offset = target.position - worldPosition;
                    offset.y = 0f;

                    if (offset.sqrMagnitude <= sqrRange)
                    {
                        results.Add(target);
                    }
                }

                if (chunk.Transforms.Count == 0)
                {
                    chunks.Remove(coordinate);
                }
            }
        }

        return results;
    }

    private GridChunk GetOrCreateChunk(Vector2Int coordinate)
    {
        if (!chunks.TryGetValue(coordinate, out GridChunk chunk))
        {
            chunk = new GridChunk();
            chunks.Add(coordinate, chunk);
        }

        return chunk;
    }

    private void RemoveFromChunk(Transform target, Vector2Int coordinate)
    {
        if (!chunks.TryGetValue(coordinate, out GridChunk chunk))
        {
            return;
        }

        chunk.Transforms.Remove(target);

        if (chunk.Transforms.Count == 0)
        {
            chunks.Remove(coordinate);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGridGizmos)
        {
            return;
        }

        HarvestSpawner spawner = GetComponent<HarvestSpawner>();

        if (spawner == null)
        {
            return;
        }

        float safeChunkSize = ChunkSize;
        Vector2 halfArea = spawner.SpawnArea * 0.5f;
        int minX = Mathf.FloorToInt(-halfArea.x / safeChunkSize);
        int maxX = Mathf.CeilToInt(halfArea.x / safeChunkSize);
        int minZ = Mathf.FloorToInt(-halfArea.y / safeChunkSize);
        int maxZ = Mathf.CeilToInt(halfArea.y / safeChunkSize);
        float minLocalX = minX * safeChunkSize;
        float maxLocalX = maxX * safeChunkSize;
        float minLocalZ = minZ * safeChunkSize;
        float maxLocalZ = maxZ * safeChunkSize;
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.cyan;

        for (int x = minX; x <= maxX; x++)
        {
            float localX = x * safeChunkSize;
            Gizmos.DrawLine(
                new Vector3(localX, 0f, minLocalZ),
                new Vector3(localX, 0f, maxLocalZ));
        }

        for (int z = minZ; z <= maxZ; z++)
        {
            float localZ = z * safeChunkSize;
            Gizmos.DrawLine(
                new Vector3(minLocalX, 0f, localZ),
                new Vector3(maxLocalX, 0f, localZ));
        }

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}
