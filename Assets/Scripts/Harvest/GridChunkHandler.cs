using UnityEngine;

[DisallowMultipleComponent]
public sealed class GridChunkHandler : MonoBehaviour
{
    [SerializeField] private GridGeometry geometry = new();
    [SerializeField] private ChunkStreamer streamer = new();
    [SerializeField] private bool showGridGizmos = true;

    private readonly ChunkRegistry registry = new();
    private readonly StageResolver stageResolver = new();

    public GridGeometry Geometry => geometry;
    public ChunkStreamer Streamer => streamer;
    public ChunkRegistry Registry => registry;

    // Geometry, Streamer, Registry의 런타임 참조를 연결한다.
    private void Awake()
    {
        geometry.Initialize(transform);
        stageResolver.Initialize();
        streamer.Initialize(transform, geometry, stageResolver);
        registry.Initialize(geometry);
    }

    // 프레임당 청크 스트리밍 작업을 진행한다.
    private void Update()
    {
        streamer.Tick();
    }

    // 설정된 전체 영역의 청크 경계를 Scene 뷰에 표시한다.
    private void OnDrawGizmos()
    {
        if (!showGridGizmos)
        {
            return;
        }

        Vector2Int min = geometry.MinChunkCoordinate;
        Vector2Int max = geometry.MaxChunkCoordinate;
        Rect chunkBounds = geometry.ChunkBounds;
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.cyan;

        for (int x = min.x; x <= max.x + 1; x++)
        {
            float localX = x * geometry.ChunkSize;
            Gizmos.DrawLine(
                new Vector3(localX, 0f, chunkBounds.yMin),
                new Vector3(localX, 0f, chunkBounds.yMax));
        }

        for (int z = min.y; z <= max.y + 1; z++)
        {
            float localZ = z * geometry.ChunkSize;
            Gizmos.DrawLine(
                new Vector3(chunkBounds.xMin, 0f, localZ),
                new Vector3(chunkBounds.xMax, 0f, localZ));
        }

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}
