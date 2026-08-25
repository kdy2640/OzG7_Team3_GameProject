using UnityEngine;

[DisallowMultipleComponent]
public sealed class HarvestMover : MonoBehaviour
{
    private const float StageBoundaryEpsilon = 0.01f;

    [SerializeField, Min(0f)] private float arrivalDistance = 0.25f;

    private Transform gridOrigin;
    private GridGeometry geometry;
    private ChunkRegistry registry;
    private ChunkStreamer streamer;
    private bool ignoreStageBounds;
    private float stageMinZ;
    private float stageMaxZ;

    public void Init(
        StageType stageType,
        GridChunkHandler gridChunkHandler,
        bool ignoreStageBounds)
    {
        this.ignoreStageBounds = ignoreStageBounds;

        if (gridChunkHandler == null)
        {
            Debug.LogError(
                "[HarvestMover] GridChunkHandler is not assigned.",
                this);
            enabled = false;
            return;
        }

        gridOrigin = gridChunkHandler.transform;
        geometry = gridChunkHandler.Geometry;
        registry = gridChunkHandler.Registry;
        streamer = gridChunkHandler.Streamer;

        if (!ignoreStageBounds)
            InitializeStageBounds(stageType);

        enabled = true;
    }

    public bool HasArrived(Vector3 target)
    {
        Vector3 targetOffset = target - transform.position;
        targetOffset.y = 0f;
        return targetOffset.sqrMagnitude <= arrivalDistance * arrivalDistance;
    }

    public void Move(Vector3 direction, float moveSpeed)
    {
        if (GameManager.Instance?.Harvest?.IsRunning != true)
            return;

        if (direction.sqrMagnitude <= 0.0001f || moveSpeed <= 0f)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = currentPosition
            + direction.normalized
            * (moveSpeed * Time.fixedDeltaTime);
        nextPosition = geometry.ClampToArea(nextPosition);

        if (!ignoreStageBounds)
            nextPosition = ClampToStage(nextPosition);

        Vector3 movement = nextPosition - currentPosition;
        movement.y = 0f;

        if (movement.sqrMagnitude <= 0.0001f)
            return;

        transform.position = nextPosition;
        transform.rotation = Quaternion.LookRotation(movement, Vector3.up);

        if (registry.TryUpdateChunk(transform, out Vector2Int coordinate))
        {
            streamer.MoveActorToChunk(transform, coordinate);
        }
    }

    private void InitializeStageBounds(StageType stageType)
    {
        Rect area = geometry.Area;
        StageDataSO stageData = StageDataDB.GetData(stageType);
        stageMinZ = Mathf.Max(area.yMin, stageData.ZStart);

        int nextStageIndex = (int)stageType + 1;
        stageMaxZ = nextStageIndex < (int)StageType.Count
            ? StageDataDB.GetData((StageType)nextStageIndex).ZStart
                - StageBoundaryEpsilon
            : area.yMax;
    }

    public Vector3 GetRandomPatrolPosition()
    {
        Rect area = geometry.Area;
        Vector3 localPosition = gridOrigin.InverseTransformPoint(
            transform.position);
        localPosition.x = Random.Range(area.xMin, area.xMax);
        localPosition.z = ignoreStageBounds
            ? Random.Range(area.yMin, area.yMax)
            : Random.Range(stageMinZ, stageMaxZ);

        return gridOrigin.TransformPoint(localPosition);
    }

    private Vector3 ClampToStage(Vector3 worldPosition)
    {
        Vector3 localPosition = gridOrigin.InverseTransformPoint(worldPosition);
        localPosition.z = Mathf.Clamp(
            localPosition.z,
            stageMinZ,
            stageMaxZ);

        return gridOrigin.TransformPoint(localPosition);
    }
}
