using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class GridGeometry
{
    [SerializeField] private Vector2 areaSize = new(9f, 9f);
    [SerializeField, Min(0.01f)] private float chunkSize = 5f;
    [SerializeField, Min(0.01f)] private float xSpacing = 2.5f;
    [SerializeField, Min(0.01f)] private float zSpacing = 2.5f;

    [NonSerialized] private Transform origin;

    public Rect Area => new(
        areaSize.x * -0.5f,
        areaSize.y * -0.5f,
        areaSize.x,
        areaSize.y);

    public float ChunkSize => chunkSize;

    public Vector2Int MinChunkCoordinate => new(
        Mathf.FloorToInt(Area.xMin / chunkSize),
        Mathf.FloorToInt(Area.yMin / chunkSize));

    public Vector2Int MaxChunkCoordinate => new(
        Mathf.CeilToInt(Area.xMax / chunkSize) - 1,
        Mathf.CeilToInt(Area.yMax / chunkSize) - 1);

    public Rect ChunkBounds
    {
        get
        {
            Vector2Int min = MinChunkCoordinate;
            Vector2Int max = MaxChunkCoordinate;

            return Rect.MinMaxRect(
                min.x * chunkSize,
                min.y * chunkSize,
                (max.x + 1) * chunkSize,
                (max.y + 1) * chunkSize);
        }
    }

    public void Initialize(Transform gridOrigin)
    {
        origin = gridOrigin;
    }

    public Vector2Int GetChunkCoordinate(Vector3 worldPosition)
    {
        Vector3 localPosition = origin.InverseTransformPoint(worldPosition);

        return GetLocalChunkCoordinate(
            new Vector2(localPosition.x, localPosition.z));
    }

    public List<Vector2Int> GetChunksInRange(
        Vector2Int center,
        int radius)
    {
        List<Vector2Int> coordinates = new();

        for (int z = -radius; z <= radius; z++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                Vector2Int coordinate = center + new Vector2Int(x, z);

                if (ContainsChunk(coordinate)
                    && GetChunkDistance(coordinate, center) <= radius)
                {
                    coordinates.Add(coordinate);
                }
            }
        }

        coordinates.Sort((a, b) =>
            GetChunkDistance(a, center)
                .CompareTo(GetChunkDistance(b, center)));

        return coordinates;
    }

    public void GetChunkSearchRange(
        Vector3 worldPosition,
        float range,
        out Vector2Int min,
        out Vector2Int max)
    {
        Vector3 localPosition = origin.InverseTransformPoint(worldPosition);
        min = GetLocalChunkCoordinate(
            new Vector2(localPosition.x - range, localPosition.z - range));
        max = GetLocalChunkCoordinate(
            new Vector2(localPosition.x + range, localPosition.z + range));
    }

    public IEnumerable<Vector2> GetCellPositions(Vector2Int chunkCoordinate)
    {
        Rect area = GetClippedChunkArea(chunkCoordinate);
        int xCount = Mathf.FloorToInt(areaSize.x / xSpacing);
        int zCount = Mathf.FloorToInt(areaSize.y / zSpacing);
        float startX = (xCount - 1) * xSpacing * -0.5f;
        float startZ = (zCount - 1) * zSpacing * -0.5f;
        int minX = Mathf.Clamp(
            Mathf.CeilToInt((area.xMin - startX) / xSpacing),
            0,
            xCount);
        int maxX = Mathf.Clamp(
            Mathf.CeilToInt((area.xMax - startX) / xSpacing),
            0,
            xCount);
        int minZ = Mathf.Clamp(
            Mathf.CeilToInt((area.yMin - startZ) / zSpacing),
            0,
            zCount);
        int maxZ = Mathf.Clamp(
            Mathf.CeilToInt((area.yMax - startZ) / zSpacing),
            0,
            zCount);

        for (int z = minZ; z < maxZ; z++)
        {
            for (int x = minX; x < maxX; x++)
            {
                yield return new Vector2(
                    startX + x * xSpacing,
                    startZ + z * zSpacing);
            }
        }
    }

    public IEnumerable<Vector2> GetGapPositions(Vector2Int chunkCoordinate)
    {
        Rect area = GetClippedChunkArea(chunkCoordinate);
        int xCount = Mathf.FloorToInt(areaSize.x / xSpacing);
        int zCount = Mathf.FloorToInt(areaSize.y / zSpacing);

        if (xCount < 2 || zCount < 2)
        {
            yield break;
        }

        float startX = (xCount - 1) * xSpacing * -0.5f;
        float startZ = (zCount - 1) * zSpacing * -0.5f;
        float gapStartX = startX + xSpacing * 0.5f;
        float gapStartZ = startZ + zSpacing * 0.5f;
        int xGapCount = xCount - 1;
        int zGapCount = zCount - 1;
        int minX = Mathf.Clamp(
            Mathf.CeilToInt((area.xMin - gapStartX) / xSpacing),
            0,
            xGapCount);
        int maxX = Mathf.Clamp(
            Mathf.CeilToInt((area.xMax - gapStartX) / xSpacing),
            0,
            xGapCount);
        int minZ = Mathf.Clamp(
            Mathf.CeilToInt((area.yMin - gapStartZ) / zSpacing),
            0,
            zGapCount);
        int maxZ = Mathf.Clamp(
            Mathf.CeilToInt((area.yMax - gapStartZ) / zSpacing),
            0,
            zGapCount);

        for (int z = minZ; z < maxZ; z++)
        {
            for (int x = minX; x < maxX; x++)
            {
                yield return new Vector2(
                    gapStartX + x * xSpacing,
                    gapStartZ + z * zSpacing);
            }
        }
    }

    public Vector2 GetRandomPositionInChunk(Vector2Int chunkCoordinate)
    {
        Rect area = GetClippedChunkArea(chunkCoordinate);
        float xInset = Mathf.Min(0.01f, area.width * 0.25f);
        float zInset = Mathf.Min(0.01f, area.height * 0.25f);

        return new Vector2(
            UnityEngine.Random.Range(area.xMin + xInset, area.xMax - xInset),
            UnityEngine.Random.Range(area.yMin + zInset, area.yMax - zInset));
    }

    public Vector3 GetRandomPosition(Vector3 referencePosition)
    {
        Vector3 localPosition = origin.InverseTransformPoint(referencePosition);
        Rect area = Area;
        localPosition.x = UnityEngine.Random.Range(area.xMin, area.xMax);
        localPosition.z = UnityEngine.Random.Range(area.yMin, area.yMax);

        return origin.TransformPoint(localPosition);
    }

    public Vector3 ClampToArea(Vector3 worldPosition)
    {
        Vector3 localPosition = origin.InverseTransformPoint(worldPosition);
        Rect area = Area;
        localPosition.x = Mathf.Clamp(localPosition.x, area.xMin, area.xMax);
        localPosition.z = Mathf.Clamp(localPosition.z, area.yMin, area.yMax);

        return origin.TransformPoint(localPosition);
    }

    public int GetChunkDistance(Vector2Int a, Vector2Int b)
    {
        Vector2Int offset = a - b;
        return Mathf.Abs(offset.x) + Mathf.Abs(offset.y);
    }

    public float GetSqrDistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 offset = a - b;
        offset.y = 0f;
        return offset.sqrMagnitude;
    }

    private Vector2Int GetLocalChunkCoordinate(Vector2 localPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(localPosition.x / chunkSize),
            Mathf.FloorToInt(localPosition.y / chunkSize));
    }

    public bool ContainsChunk(Vector2Int coordinate)
    {
        Vector2Int min = MinChunkCoordinate;
        Vector2Int max = MaxChunkCoordinate;

        return coordinate.x >= min.x
            && coordinate.x <= max.x
            && coordinate.y >= min.y
            && coordinate.y <= max.y;
    }

    private Rect GetChunkArea(Vector2Int coordinate)
    {
        return new Rect(
            coordinate.x * chunkSize,
            coordinate.y * chunkSize,
            chunkSize,
            chunkSize);
    }

    private Rect GetClippedChunkArea(Vector2Int coordinate)
    {
        Rect chunkArea = GetChunkArea(coordinate);
        Rect area = Area;

        return Rect.MinMaxRect(
            Mathf.Max(chunkArea.xMin, area.xMin),
            Mathf.Max(chunkArea.yMin, area.yMin),
            Mathf.Min(chunkArea.xMax, area.xMax),
            Mathf.Min(chunkArea.yMax, area.yMax));
    }

}
