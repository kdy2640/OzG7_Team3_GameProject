
public static class DishPriceCalculator
{
    public static int BasicPriceCalculate(DishType dish)
    {
        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(dish);

        UpgradeDataDB.GetData(dish).TryGetRequiredCost(
            level > 0 ? level : 1,
            out int cost);

        DishDataSO dishData = DishDataDB.GetData(dish);
        MarketManager market = GameManager.Instance.Market;
        int businessDay = market.MarketData.CurrentBusinessDay;
        TasteType eventTaste =
            market.FestivalCalendar.GetNowTaste(businessDay);
        CategoryType eventCategory =
            market.FestivalCalendar.GetNowCategory(businessDay);

        bool matchesTaste = eventTaste != TasteType.Count
            && dishData.Tastes == eventTaste;
        bool matchesCategory = eventCategory != CategoryType.Count
            && dishData.Category == eventCategory;

        if (matchesTaste || matchesCategory)
            cost += cost / 2;

        return cost;
    }

    // 피버 단계
}
