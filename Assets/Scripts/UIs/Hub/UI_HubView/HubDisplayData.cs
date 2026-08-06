using System;

[Serializable]
public class HubDisplayData
{
    // 현재 보유 재화
    public int Gold;

    // 1 ~ 4
    public int PlayerLevel;

    public string PlayerName;

    // 현재 누적 매출액
    public int CurrentSales;

    // 현재 승급 목표 매출액
    public int TargetSales;

    // 0 ~ 5
    public int PromotionStep;

    public string PromotionQuestTitle;

    public string PromotionQuestDescription;

    public string CurrentEffect;
}