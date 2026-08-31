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
    private SFXType voiceSFXType = SFXType.None;
    private SFXType hitSFXType = SFXType.None;
    private SFXType dieSFXType = SFXType.None;

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
        RuntimeAnimatorController animatorController,
        HarvestType harvestType)
    {
        this.player = player;
        this.mover = mover;
        this.animator = animator;
        this.animalStat = animalStat;
        animator.runtimeAnimatorController = animatorController;
        ConfigureSFXTypes(harvestType);

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

        if (nextState == AnimalStateType.Flee)
        {
            GameManager.Instance.Utility.Audio.PlaySFX(voiceSFXType);
        }
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

        GameManager.Instance.Utility.Audio.PlaySFX(hitSFXType);
        animator.SetTrigger(HitHash);
    }

    public void PlayDeath()
    {
        GameManager.Instance.Utility.Audio.PlaySFX(dieSFXType);
        animator.SetTrigger(DeathHash);
    }

    private void ConfigureSFXTypes(HarvestType harvestType)
    {
        switch (harvestType)
        {
            case HarvestType.Chicken:
                voiceSFXType = SFXType.Harvest_ChickenVoice;
                hitSFXType = SFXType.Harvest_ChickenHit;
                dieSFXType = SFXType.Harvest_ChickenDie;
                break;

            case HarvestType.Cow:
                voiceSFXType = SFXType.Harvest_CowVoice;
                hitSFXType = SFXType.Harvest_CowHit;
                dieSFXType = SFXType.Harvest_CowDie;
                break;

            case HarvestType.Sheep:
                voiceSFXType = SFXType.Harvest_SheepVoice;
                hitSFXType = SFXType.Harvest_SheepHit;
                dieSFXType = SFXType.Harvest_SheepDie;
                break;

            case HarvestType.Pig:
                voiceSFXType = SFXType.Harvest_PigVoice;
                hitSFXType = SFXType.Harvest_PigHit;
                dieSFXType = SFXType.Harvest_PigDie;
                break;

            default:
                Debug.LogError(
                    $"[AnimalStateController] 동물 SFX가 없는 수확 타입입니다: {harvestType}",
                    this);
                break;
        }
    }

    public void CompleteDeath()
    {
        gameObject.SetActive(false);
    }
}
