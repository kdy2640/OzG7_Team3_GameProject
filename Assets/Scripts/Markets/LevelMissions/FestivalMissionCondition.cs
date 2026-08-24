using System;

[Serializable]
public sealed class FestivalMissionCondition : MissionCondition
{
    public override bool IsSatisfied()
    {
        FestivalCalendar calendar = GameManager.Instance.Market.FestivalCalendar;

        return calendar.LatestTaste != TasteType.Count
            || calendar.LatestCategory != CategoryType.Count;
    }

    public override string ToString()
    {
        return $"{(IsSatisfied() ? 1 : 0)} / 1";
    }
}
