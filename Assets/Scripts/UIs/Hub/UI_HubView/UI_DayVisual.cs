using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DayVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private Transform festivalCardContainer;
    [SerializeField] private UI_FestivalCard festivalCardPrefab;

    private readonly List<UI_FestivalCard> festivalCards = new();
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

        dayText.text = $"Day {currentBusinessDay:D2}";

        FestivalCalendar festivalCalendar = market.FestivalCalendar;
        int festivalCount = 0;
        TasteType nowTaste = festivalCalendar.GetNowTaste(currentBusinessDay);

        if (nowTaste != TasteType.Count)
        {
            while (festivalCards.Count <= festivalCount)
            {
                UI_FestivalCard festivalCard = Instantiate(
                    festivalCardPrefab,
                    festivalCardContainer);
                festivalCards.Add(festivalCard);
            }

            int daysLeft = festivalCalendar.TasteEndBusinessDay
                - currentBusinessDay
                + 1;
            UI_FestivalCard tasteFestivalCard = festivalCards[festivalCount++];
            tasteFestivalCard.gameObject.SetActive(true);
            tasteFestivalCard.SetData($"{nowTaste} Festival", daysLeft);
        }

        CategoryType nowCategory = festivalCalendar.GetNowCategory(currentBusinessDay);

        if (nowCategory != CategoryType.Count)
        {
            while (festivalCards.Count <= festivalCount)
            {
                UI_FestivalCard festivalCard = Instantiate(
                    festivalCardPrefab,
                    festivalCardContainer);
                festivalCards.Add(festivalCard);
            }

            int daysLeft = festivalCalendar.CategoryEndBusinessDay
                - currentBusinessDay
                + 1;
            UI_FestivalCard categoryFestivalCard = festivalCards[festivalCount++];
            categoryFestivalCard.gameObject.SetActive(true);
            categoryFestivalCard.SetData($"{nowCategory} Festival", daysLeft);
        }

        for (int i = festivalCount; i < festivalCards.Count; i++)
        {
            festivalCards[i].gameObject.SetActive(false);
        }
    }
}
