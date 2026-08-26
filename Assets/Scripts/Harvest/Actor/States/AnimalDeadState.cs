using UnityEngine;

public sealed class AnimalDeadState : AnimalStateBase
{
    private bool enteredDeath;

    public override void StateStart()
    {
        enteredDeath = false;
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

        if (stateInfo.normalizedTime >= 1f)
        {
            Controller.CompleteDeath();
        }
    }
}
