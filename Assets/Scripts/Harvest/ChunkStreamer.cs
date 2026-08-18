using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ChunkStreamer
{
    [SerializeField, Min(0)] private int loadRadius = 4;
    [SerializeField, Min(0)] private int unloadRadius = 5;
    [SerializeField, Min(1)] private int maxChunkLoadsPerFrame = 4;

    private readonly Dictionary<Vector2Int, ChunkRuntime> chunkRuntimes = new();
    private readonly HashSet<Vector2Int> loadedCoordinates = new();
    private readonly HashSet<Vector2Int> pendingCoordinates = new();
    private readonly Queue<Vector2Int> pendingLoads = new();
    private readonly List<Transform> loadingTargets = new();
    private readonly List<Vector2Int> loadingTargetCoordinates = new();

    private Transform root;
    private GridGeometry geometry;
    private StageResolver stageResolver;
    private HarvestSpawner spawner;
    private Transform cropContainer;

    private sealed class ChunkRuntime
    {
        public readonly Transform Root;
        public bool IsGenerated;

        // 청크 루트와 최초 생성 여부를 함께 관리한다.
        public ChunkRuntime(Transform root)
        {
            Root = root;
        }
    }

    // 스트리밍에 필요한 루트와 Geometry를 연결한다.
    public void Initialize(
        Transform streamRoot,
        GridGeometry gridGeometry,
        StageResolver resolver)
    {
        root = streamRoot;
        geometry = gridGeometry;
        stageResolver = resolver;
    }

    // 로딩 타깃과 스포너를 설정하고 최초 청크 로딩을 시작한다.
    public void BeginLoading(Transform target, HarvestSpawner harvestSpawner)
    {
        Reset();

        spawner = harvestSpawner;
        cropContainer = root.Find("Crops");

        if (cropContainer == null)
        {
            cropContainer = new GameObject("Crops").transform;
            cropContainer.SetParent(root, false);
        }

        AddLoadingTarget(target);

        for (int i = 0; i < loadingTargetCoordinates.Count; i++)
        {
            Vector2Int coordinate = loadingTargetCoordinates[i];

            if (LoadChunk(coordinate))
            {
                loadedCoordinates.Add(coordinate);
            }
        }

        RefreshLoadingCoordinates();
    }

    public void AddLoadingTarget(Transform target)
    {
        if (target == null || loadingTargets.Contains(target))
        {
            return;
        }

        Vector2Int coordinate = geometry.GetChunkCoordinate(target.position);
        loadingTargets.Add(target);
        loadingTargetCoordinates.Add(coordinate);

        if (spawner == null || cropContainer == null)
        {
            return;
        }

        if (LoadChunk(coordinate))
        {
            loadedCoordinates.Add(coordinate);
        }

        RefreshLoadingCoordinates();
    }

    // 타깃 청크 변경과 프레임당 청크 로드를 처리한다.
    public void Tick()
    {
        bool targetCoordinateChanged = false;

        for (int i = loadingTargets.Count - 1; i >= 0; i--)
        {
            Transform target = loadingTargets[i];

            if (target == null)
            {
                loadingTargets.RemoveAt(i);
                loadingTargetCoordinates.RemoveAt(i);
                targetCoordinateChanged = true;
                continue;
            }

            Vector2Int nextCoordinate = geometry.GetChunkCoordinate(
                target.position);

            if (loadingTargetCoordinates[i] == nextCoordinate)
            {
                continue;
            }

            loadingTargetCoordinates[i] = nextCoordinate;
            targetCoordinateChanged = true;
        }

        if (targetCoordinateChanged)
        {
            RefreshLoadingCoordinates();
        }

        if (spawner != null)
        {
            ProcessPendingLoads();
        }
    }

    // 로딩 상태와 생성된 모든 청크 루트를 초기화한다.
    public void Reset()
    {
        loadedCoordinates.Clear();
        pendingCoordinates.Clear();
        pendingLoads.Clear();

        foreach (ChunkRuntime runtime in chunkRuntimes.Values)
        {
            UnityEngine.Object.Destroy(runtime.Root.gameObject);
        }

        chunkRuntimes.Clear();
    }

    // 이동 Actor를 새 좌표의 청크 루트 아래로 옮긴다.
    public void MoveActorToChunk(Transform target, Vector2Int coordinate)
    {
        if (!geometry.ContainsChunk(coordinate))
        {
            return;
        }

        ChunkRuntime runtime = GetOrCreateChunkRuntime(coordinate);

        if (target.parent != runtime.Root)
        {
            target.SetParent(runtime.Root, true);
        }
    }

    // 청크를 최초 생성하거나 기존 청크 루트를 활성화한다.
    private bool LoadChunk(Vector2Int coordinate)
    {
        if (!geometry.ContainsChunk(coordinate))
        {
            return false;
        }

        ChunkRuntime runtime = GetOrCreateChunkRuntime(coordinate);

        if (!runtime.IsGenerated)
        {
            GenerateChunk(coordinate, runtime);
            runtime.IsGenerated = true;
        }

        if (!runtime.Root.gameObject.activeSelf)
        {
            runtime.Root.gameObject.SetActive(true);
        }

        return true;
    }

    // 생성된 청크 루트를 비활성화해 상태를 보존한다.
    private void UnloadChunk(Vector2Int coordinate)
    {
        if (chunkRuntimes.TryGetValue(coordinate, out ChunkRuntime runtime))
        {
            runtime.Root.gameObject.SetActive(false);
        }
    }

    // 좌표에 대응하는 런타임 청크 루트를 반환하거나 생성한다.
    private ChunkRuntime GetOrCreateChunkRuntime(Vector2Int coordinate)
    {
        if (chunkRuntimes.TryGetValue(coordinate, out ChunkRuntime runtime))
        {
            return runtime;
        }

        GameObject chunkObject = new($"Chunk_{coordinate.x}_{coordinate.y}");
        chunkObject.SetActive(false);
        chunkObject.transform.SetParent(cropContainer, false);
        runtime = new ChunkRuntime(chunkObject.transform);
        chunkRuntimes.Add(coordinate, runtime);

        return runtime;
    }

    // Geometry가 계산한 위치에 정적·이동 작물을 생성한다.
    private void GenerateChunk(Vector2Int coordinate, ChunkRuntime runtime)
    {
        foreach (Vector2 localPosition in
                 geometry.GetCellPositions(coordinate))
        {
            if (stageResolver.TryGetStaticType(
                    localPosition.y,
                    out HarvestType staticType))
            {
                spawner.SpawnCrop(
                    staticType,
                    localPosition,
                    runtime.Root);
            }
        }

        Vector2 movablePosition = geometry.GetRandomPositionInChunk(
            coordinate);

        if (stageResolver.TryGetMovableType(
                movablePosition.y,
                out HarvestType movableType))
        {
            spawner.SpawnCrop(
                movableType,
                movablePosition,
                runtime.Root);
        }
    }

    // 타깃 기준으로 언로드 대상과 신규 로드 대기열을 다시 계산한다.
    private void RefreshLoadingCoordinates()
    {
        List<Vector2Int> coordinatesToUnload = new();

        foreach (Vector2Int coordinate in loadedCoordinates)
        {
            if (!IsWithinRangeOfAnyTarget(coordinate, unloadRadius))
            {
                coordinatesToUnload.Add(coordinate);
            }
        }

        foreach (Vector2Int coordinate in coordinatesToUnload)
        {
            UnloadChunk(coordinate);
            loadedCoordinates.Remove(coordinate);
        }

        pendingLoads.Clear();
        pendingCoordinates.Clear();

        for (int i = 0; i < loadingTargetCoordinates.Count; i++)
        {
            foreach (Vector2Int coordinate in geometry.GetChunksInRange(
                         loadingTargetCoordinates[i],
                         loadRadius))
            {
                if (!loadedCoordinates.Contains(coordinate)
                    && pendingCoordinates.Add(coordinate))
                {
                    pendingLoads.Enqueue(coordinate);
                }
            }
        }
    }

    // 프레임당 허용량만큼 대기 중인 청크를 로드한다.
    private void ProcessPendingLoads()
    {
        int loadCount = Mathf.Min(maxChunkLoadsPerFrame, pendingLoads.Count);

        for (int i = 0; i < loadCount; i++)
        {
            Vector2Int coordinate = pendingLoads.Dequeue();
            pendingCoordinates.Remove(coordinate);

            if (!IsWithinRangeOfAnyTarget(coordinate, loadRadius)
                || loadedCoordinates.Contains(coordinate))
            {
                continue;
            }

            if (LoadChunk(coordinate))
            {
                loadedCoordinates.Add(coordinate);
            }
        }
    }

    private bool IsWithinRangeOfAnyTarget(Vector2Int coordinate, int radius)
    {
        for (int i = 0; i < loadingTargetCoordinates.Count; i++)
        {
            if (geometry.GetChunkDistance(
                    coordinate,
                    loadingTargetCoordinates[i]) <= radius)
            {
                return true;
            }
        }

        return false;
    }
}
