
public static class DishPriceCalculator
{
    public static int BasicPriceCalculate(DishType dish)
    {
        int cost;
        UpgradeDataDB.GetData(dish).TryGetRequiredCost(
            GameManager.Instance.Upgrade.RuntimeLevel.Get(dish), 
            out cost
            );
        return cost;
    }

    // 피버 단계
}
