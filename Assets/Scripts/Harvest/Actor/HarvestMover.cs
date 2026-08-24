using UnityEngine;

public enum HarvestMoveState
{
    Patrol,
    Flee
}

[DisallowMultipleComponent]
public sealed class HarvestMover : MonoBehaviour
{
    private const float StageBoundaryEpsilon = 0.01f;

    [SerializeField, Min(0f)] private float fleeRange = 3f;
    [SerializeField, Min(0f)] private float arrivalDistance = 0.25f;
    [SerializeField] private HarvestMoveState state;

    private Transform player;
    private Transform gridOrigin;
    private GridGeometry geometry;
    private ChunkRegistry registry;
    private ChunkStreamer streamer;
    private float speed;
    private float stageMinZ;
    private float stageMaxZ;
    private Vector3 patrolTarget;
    private bool isInitialized;

    public void Init(
        Transform player,
        float speed,
        StageType stageType,
        GridChunkHandler gridChunkHandler)
    {
        this.player = player;
        this.speed = Mathf.Max(0f, speed);

        if (player == null || gridChunkHandler == null)
        {
            Debug.LogError(
                "[HarvestMover] Player or GridChunkHandler is not assigned.",
                this);
            enabled = false;
            return;
        }

        gridOrigin = gridChunkHandler.transform;
        geometry = gridChunkHandler.Geometry;
        registry = gridChunkHandler.Registry;
        streamer = gridChunkHandler.Streamer;
        InitializeStageBounds(stageType);
        state = HarvestMoveState.Patrol;
        patrolTarget = GetRandomPatrolPosition();
        isInitialized = true;
        enabled = true;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance?.Harvest?.IsRunning != true)
            return;

        if (!isInitialized)
            return;

        UpdateState();

        switch (state)
        {
            case HarvestMoveState.Patrol:
                Patrol();
                break;
            case HarvestMoveState.Flee:
                Flee();
                break;
        }
    }

    private void UpdateState()
    {
        Vector3 playerOffset = player.position - transform.position;
        playerOffset.y = 0f;

        HarvestMoveState nextState =
            playerOffset.sqrMagnitude <= fleeRange * fleeRange
                ? HarvestMoveState.Flee
                : HarvestMoveState.Patrol;

        if (state == nextState)
            return;

        state = nextState;

        if (state == HarvestMoveState.Patrol)
            patrolTarget = GetRandomPatrolPosition();
    }

    private void Patrol()
    {
        Vector3 targetOffset = patrolTarget - transform.position;
        targetOffset.y = 0f;

        if (targetOffset.sqrMagnitude <= arrivalDistance * arrivalDistance)
        {
            patrolTarget = GetRandomPatrolPosition();
            targetOffset = patrolTarget - transform.position;
            targetOffset.y = 0f;
        }

        Move(targetOffset);
    }

    private void Flee()
    {
        Vector3 fleeDirection = transform.position - player.position;
        fleeDirection.y = 0f;
        Move(fleeDirection);
    }

    private void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f || speed <= 0f)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = currentPosition
            + direction.normalized * (speed * Time.fixedDeltaTime);
        nextPosition = geometry.ClampToArea(nextPosition);
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

    private Vector3 GetRandomPatrolPosition()
    {
        Rect area = geometry.Area;
        Vector3 localPosition = gridOrigin.InverseTransformPoint(
            transform.position);
        localPosition.x = Random.Range(area.xMin, area.xMax);
        localPosition.z = Random.Range(stageMinZ, stageMaxZ);

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
