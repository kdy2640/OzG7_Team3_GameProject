using UnityEngine;

public enum HarvestMoveState
{
    Patrol,
    Flee
}

[DisallowMultipleComponent]
public sealed class HarvestMover : MonoBehaviour
{
    [SerializeField, Min(0f)] private float fleeRange = 3f;
    [SerializeField, Min(0f)] private float arrivalDistance = 0.25f;
    [SerializeField] private HarvestMoveState state;

    private Transform player;
    private GridGeometry geometry;
    private ChunkRegistry registry;
    private ChunkStreamer streamer;
    private float speed;
    private Vector3 patrolTarget;
    private bool isInitialized;

    public void Init(
        Transform player,
        float speed,
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

        geometry = gridChunkHandler.Geometry;
        registry = gridChunkHandler.Registry;
        streamer = gridChunkHandler.Streamer;
        state = HarvestMoveState.Patrol;
        patrolTarget = geometry.GetRandomPosition(transform.position);
        isInitialized = true;
        enabled = true;
    }

    private void FixedUpdate()
    {
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
            patrolTarget = geometry.GetRandomPosition(transform.position);
    }

    private void Patrol()
    {
        Vector3 targetOffset = patrolTarget - transform.position;
        targetOffset.y = 0f;

        if (targetOffset.sqrMagnitude <= arrivalDistance * arrivalDistance)
        {
            patrolTarget = geometry.GetRandomPosition(transform.position);
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
}
