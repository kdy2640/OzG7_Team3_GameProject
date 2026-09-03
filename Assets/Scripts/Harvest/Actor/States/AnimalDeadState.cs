using UnityEngine;

public sealed class AnimalDeadState : AnimalStateBase
{
    private const float DeathHoldDuration = 0.25f;

    private bool enteredDeath;
    private float deathHoldElapsed;

    public override void StateStart()
    {
        enteredDeath = false;
        deathHoldElapsed = 0f;
        Controller.SetEating(false);
        Controller.SetBlend(0f);
        Controller.PlayDeath();
    }

    public override void StateEnd()
    {
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo =
            Controller.Animator.GetCurrentAnimatorStateInfo(0);

        if (!enteredDeath)
        {
            enteredDeath = stateInfo.IsName("Death");
            return;
        }

        if (stateInfo.normalizedTime < 1f)
            return;

        deathHoldElapsed += Time.deltaTime;

        if (deathHoldElapsed >= DeathHoldDuration)
        {
            Controller.CompleteDeath();
        }
    }
}
