using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Dish")]
public sealed class DishUpgradeDataSO : UpgradeDataSO
{
    [SerializeField] private DishType targetDish = DishType.Count;

    public DishType TargetDish => targetDish;

    public override void ApplyTo(RuntimeStat runtimeStat, int level)
    {
        if (runtimeStat == null)
            return;

        runtimeStat.Dish.Apply(targetDish, level);
    }
}
