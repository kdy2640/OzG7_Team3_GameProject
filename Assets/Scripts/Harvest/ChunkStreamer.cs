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
    private readonly Queue<Vector2Int> pendingLoads = new();

    private Transform root;
    private GridGeometry geometry;
    private HarvestSpawner spawner;
    private Transform cropContainer;
    private Transform loadingTarget;
    private Vector2Int loadingTargetCoordinate;

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
    public void Initialize(Transform streamRoot, GridGeometry gridGeometry)
    {
        root = streamRoot;
        geometry = gridGeometry;
    }

    // 로딩 타깃과 스포너를 설정하고 최초 청크 로딩을 시작한다.
    public void BeginLoading(Transform target, HarvestSpawner harvestSpawner)
    {
        Reset();

        loadingTarget = target;
        spawner = harvestSpawner;
        loadingTargetCoordinate = geometry.GetChunkCoordinate(
            loadingTarget.position);
        cropContainer = root.Find("Crops");

        if (cropContainer == null)
        {
            cropContainer = new GameObject("Crops").transform;
            cropContainer.SetParent(root, false);
        }

        if (LoadChunk(loadingTargetCoordinate))
        {
            loadedCoordinates.Add(loadingTargetCoordinate);
        }

        RefreshLoadingCoordinates();
    }

    // 타깃 청크 변경과 프레임당 청크 로드를 처리한다.
    public void Tick()
    {
        Vector2Int nextCoordinate = geometry.GetChunkCoordinate(
            loadingTarget.position);

        if (loadingTargetCoordinate != nextCoordinate)
        {
            loadingTargetCoordinate = nextCoordinate;
            RefreshLoadingCoordinates();
        }

        ProcessPendingLoads();
    }

    // 로딩 상태와 생성된 모든 청크 루트를 초기화한다.
    public void Reset()
    {
        loadedCoordinates.Clear();
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
        if (spawner.HasStaticTypes)
        {
            foreach (Vector2 localPosition in
                     geometry.GetCellPositions(coordinate))
            {
                spawner.SpawnStaticCrop(localPosition, runtime.Root);
            }
        }

        if (spawner.HasMovableTypes)
        {
            Vector2 localPosition = geometry.GetRandomPositionInChunk(
                coordinate);
            spawner.SpawnMovableCrop(localPosition, runtime.Root);
        }
    }

    // 타깃 기준으로 언로드 대상과 신규 로드 대기열을 다시 계산한다.
    private void RefreshLoadingCoordinates()
    {
        List<Vector2Int> coordinatesToUnload = new();

        foreach (Vector2Int coordinate in loadedCoordinates)
        {
            if (geometry.GetChunkDistance(
                    coordinate,
                    loadingTargetCoordinate) > unloadRadius)
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

        foreach (Vector2Int coordinate in
                 geometry.GetChunksInRange(loadingTargetCoordinate, loadRadius))
        {
            if (!loadedCoordinates.Contains(coordinate))
            {
                pendingLoads.Enqueue(coordinate);
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

            if (geometry.GetChunkDistance(
                    coordinate,
                    loadingTargetCoordinate) > loadRadius
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
}
