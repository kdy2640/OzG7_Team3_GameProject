using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DayVisual : MonoBehaviour
{
    [SerializeField] private GameObject festivalLayout;
    [SerializeField] private GameObject nonFestivalLayout;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text dayShadowText;
    [SerializeField] private TMP_Text nonFestivalDayText;
    [SerializeField] private TMP_Text nonFestivalDayShadowText;
    [SerializeField] private TMP_Text flavorText;
    [SerializeField] private TMP_Text themeText;
    [SerializeField] private List<GameObject> tasteFlavors;
    [SerializeField] private List<GameObject> categoryThemes;

    private MarketManager marketManager;

    private void OnEnable()
    {
        if (GameManager.Instance == null)
            return;

        marketManager = GameManager.Instance.Market;
        marketManager?.SubscribeMarketDataChanged(Refresh);
        Refresh();
    }

    private void OnDisable()
    {
        marketManager?.UnsubscribeMarketDataChanged(Refresh);
        marketManager = null;
    }

    public void Refresh()
    {
        if (GameManager.Instance == null || GameManager.Instance.Market == null)
            return;

        MarketManager market = GameManager.Instance.Market;
        int currentBusinessDay = market.MarketData.CurrentBusinessDay;
        FestivalCalendar festivalCalendar = market.FestivalCalendar;
        TasteType nowTaste = festivalCalendar.GetNowTaste(currentBusinessDay);
        CategoryType nowCategory = festivalCalendar.GetNowCategory(currentBusinessDay);
        bool hasFestival = nowTaste != TasteType.Count
            || nowCategory != CategoryType.Count;

        festivalLayout.SetActive(hasFestival);
        nonFestivalLayout.SetActive(!hasFestival);

        dayText.text = $"{currentBusinessDay:D2}";
        dayShadowText.text = $"{currentBusinessDay:D2}";
        nonFestivalDayText.text = $"{currentBusinessDay:D2}";
        nonFestivalDayShadowText.text = $"{currentBusinessDay:D2}";

        flavorText.text = nowTaste switch
        {
            TasteType.Salty => "짭짤한 맛 UP",
            TasteType.Clean => "담백한 맛 UP",
            TasteType.SpicyAndSour => "매콤새콤 맛 UP",
            _ => string.Empty
        };

        for (int i = 0; i < tasteFlavors.Count; i++)
        {
            tasteFlavors[i].SetActive(
                nowTaste != TasteType.Count
                && i == (int)nowTaste);
        }

        themeText.text = nowCategory switch
        {
            CategoryType.WesternDine => "웨스턴 다인 UP",
            CategoryType.AsianFood => "아시안 푸드 UP",
            CategoryType.StreetSnack => "스트릿 스낵 UP",
            _ => string.Empty
        };

        for (int i = 0; i < categoryThemes.Count; i++)
        {
            categoryThemes[i].SetActive(
                nowCategory != CategoryType.Count
                && i == (int)nowCategory);
        }
    }
}
