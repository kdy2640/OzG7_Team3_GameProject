using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Dish")]
public sealed class DishUpgradeDataSO : UpgradeDataSO
{
    [SerializeField] private DishType targetDish = DishType.Count;

    public DishType TargetDish => targetDish;
}
