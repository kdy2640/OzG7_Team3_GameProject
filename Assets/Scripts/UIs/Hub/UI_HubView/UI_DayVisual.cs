using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DayVisual : MonoBehaviour
{
    private static readonly TasteType[] NoTastes = new TasteType[0];

    [SerializeField] private TMP_Text dayText;
    [SerializeField] private Transform tasteCardContainer;
    [SerializeField] private UI_TasteCard tasteCardPrefab;

    private readonly List<UI_TasteCard> tasteCards = new();
    private readonly TasteType[] currentTastes = new TasteType[1];
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

        TasteType nowTaste = market.FestivalCalendar.GetNowTaste(currentBusinessDay);

        if (nowTaste == TasteType.Count)
        {
            RefreshTasteCards(NoTastes);
            return;
        }

        currentTastes[0] = nowTaste;
        RefreshTasteCards(currentTastes);
    }

    private void RefreshTasteCards(IReadOnlyList<TasteType> tastes)
    {
        while (tasteCards.Count < tastes.Count)
        {
            UI_TasteCard tasteCard = Instantiate(tasteCardPrefab, tasteCardContainer);
            tasteCards.Add(tasteCard);
        }

        for (int i = 0; i < tasteCards.Count; i++)
        {
            bool isActive = i < tastes.Count;
            tasteCards[i].gameObject.SetActive(isActive);

            if (isActive)
                tasteCards[i].SetData(tastes[i]);
        }
    }
}
