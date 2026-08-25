using UnityEngine;

public sealed class AnimalEatState : AnimalStateBase
{
    [SerializeField, Min(0f)] private float minDuration = 1.5f;
    [SerializeField, Min(0f)] private float maxDuration = 3f;

    private float remainingDuration;

    public override void StateStart()
    {
        remainingDuration = Random.Range(minDuration, maxDuration);
        Controller.SetBlend(0f);
        Controller.SetEating(true);
    }

    private void Update()
    {
        if (!Controller.IsRunning)
            return;

        if (Controller.IsPlayerWithin(Controller.AnimalStat.DetectionRange))
        {
            Controller.SetState(AnimalStateType.Flee);
            return;
        }

        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0f)
        {
            Controller.SetState(AnimalStateType.Patrol);
        }
    }

    public override void StateEnd()
    {
        Controller.SetEating(false);
    }
}
