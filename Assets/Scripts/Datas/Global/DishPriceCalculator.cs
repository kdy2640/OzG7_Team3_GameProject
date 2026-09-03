
public static class DishPriceCalculator
{
    private const float TipRate = 0.2f;

    public static int BasicPriceCalculate(DishType dish)
    {
        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(dish);
        return BasicPriceCalculate(dish, level);
    }

    public static int BasicPriceCalculate(DishType dish, int level)
    {
        UpgradeDataDB.GetData(dish).TryGetRequiredCost(
            level,
            out int price);

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
            price += price / 2;

        return price;
    }

    public static int BasicPriceCalculate(DishType dish, float bonusRate)
    {
        int basicPrice = BasicPriceCalculate(dish);
        int bonusPrice = (int)(basicPrice * bonusRate / 100f);
        return basicPrice + bonusPrice;
    }

    public static int TipPriceCalculate(int paidDishPrice)
    {
        return (int)(paidDishPrice * TipRate);
    }

    // 피버 단계
}
