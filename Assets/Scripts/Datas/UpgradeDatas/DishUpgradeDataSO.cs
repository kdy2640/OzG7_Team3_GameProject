using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class GroceryRequirement
{
    [SerializeField] private List<GroceryAmount> requiredGroceries = new();

    public List<GroceryAmount> RequiredGroceries => requiredGroceries;
}


/// <summary>
/// Dish는 업그레이드를 Grocery로 하기 때문에
/// requiredCost는 판매 비용으로 사용한다.
/// </summary>
[CreateAssetMenu(menuName = "Game/Upgrade/Dish")]
public sealed class DishUpgradeDataSO : UpgradeDataSO
{
    [SerializeField] private DishType targetDish = DishType.Count;
    [SerializeField] private List<GroceryRequirement> requiredIngredients = new();

    public DishType TargetDish => targetDish;
    public IReadOnlyList<GroceryRequirement> RequiredIngredients => requiredIngredients;

    public bool TryGetRequiredIngredients(
        int targetUpgradeLevel,
        out List<GroceryAmount> requiredGroceries)
    {
        int index = targetUpgradeLevel - 1;

        if (index < 0 || index >= requiredIngredients.Count
            || requiredIngredients[index] == null)
        {
            requiredGroceries = null;
            return false;
        }

        requiredGroceries = requiredIngredients[index].RequiredGroceries;
        return requiredGroceries != null;
    }
}
