using UnityEngine;

public enum AnimalStateType
{
    Eat,
    Patrol,
    Flee,
    Dead,
    Count
}

[DisallowMultipleComponent]
public sealed class AnimalStateController : MonoBehaviour
{
    private static readonly int BlendHash = Animator.StringToHash("Blend");
    private static readonly int EatHash = Animator.StringToHash("Eat");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    [SerializeField] private AnimalStateType currentState = AnimalStateType.Count;
    [SerializeField]
    private AnimalStateBase[] states =
        new AnimalStateBase[(int)AnimalStateType.Count];

    private Transform player;
    private HarvestMover mover;
    private Animator animator;
    private HarvestAnimalStat animalStat;

    public Transform Player => player;
    public HarvestMover Mover => mover;
    public Animator Animator => animator;
    public HarvestAnimalStat AnimalStat => animalStat;
    public bool IsRunning => GameManager.Instance?.Harvest?.IsRunning == true;

    public void Init(
        Transform player,
        HarvestMover mover,
        Animator animator,
        HarvestAnimalStat animalStat,
        RuntimeAnimatorController animatorController)
    {
        this.player = player;
        this.mover = mover;
        this.animator = animator;
        this.animalStat = animalStat;
        animator.runtimeAnimatorController = animatorController;

        currentState = AnimalStateType.Count;

        for (int i = 0; i < states.Length; i++)
        {
            states[i].Init(this);
        }

        SetState(AnimalStateType.Patrol);
    }

    public void SetState(AnimalStateType nextState)
    {
        if (currentState == nextState || currentState == AnimalStateType.Dead)
            return;

        if (currentState != AnimalStateType.Count)
        {
            AnimalStateBase previousState = states[(int)currentState];
            previousState.StateEnd();
            previousState.enabled = false;
        }

        currentState = nextState;
        AnimalStateBase currentStateComponent = states[(int)currentState];
        currentStateComponent.enabled = true;
        currentStateComponent.StateStart();
    }

    public bool IsPlayerWithin(float range)
    {
        Vector3 offset = player.position - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude <= range * range;
    }

    public void SetBlend(float value)
    {
        animator.SetFloat(BlendHash, value);
    }

    public void SetEating(bool value)
    {
        animator.SetBool(EatHash, value);
    }

    public void PlayHit()
    {
        if (currentState == AnimalStateType.Dead)
            return;

        animator.SetTrigger(HitHash);
    }

    public void PlayDeath()
    {
        animator.SetTrigger(DeathHash);
    }

    public void CompleteDeath()
    {
        gameObject.SetActive(false);
    }
}
