public sealed class KitchenSlotData
{
    public DishType DishType { get; }
    public float RemainTime { get; set; }

    public KitchenSlotData(DishType dishType, float remainTime)
    {
        DishType = dishType;
        RemainTime = remainTime;
    }
}
