using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuSlidePanel : MonoBehaviour
{
    // TODO
    // - 카드 클릭 시 슬라이더 내부에 현재 선택 중인 DishType을 표시한다.
    // - 슬라이더 내부에 현재 선택 중인 DishType과
    //   UI_SelectedMenuPanel에 등록된 DishType을 함께 표시하고 상태를 구분한다.
    // - Refresh 후 선택 중인 DishType이 사라지거나 이동한 경우 선택 상태를 해제하거나 갱신한다.

    [SerializeField] private GameObject eventMenuContainer;
    [SerializeField] private Transform eventCardContainer;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private UI_MenuVisualCard cardPrefab;

    private readonly List<UI_MenuVisualCard> cards = new();

    private bool isInitialized;
    private event Action<DishType> onCardClicked;

    public void Init()
    {
        if (isInitialized)
            return;

        isInitialized = true;

        GameManager.Instance.Upgrade.SubscribeUpgradeChanged(OnUpgradeChanged);
        GameManager.Instance.StockManager.SubscribeStockDataChange(Refresh);
        GameManager.Instance.Market.SubscribeMarketDataChanged(Refresh);

        Refresh();
    }

    private void OnDestroy()
    {
        if (!isInitialized || GameManager.Instance == null)
            return;

        GameManager.Instance.Upgrade?.UnsubscribeUpgradeChanged(OnUpgradeChanged);
        GameManager.Instance.StockManager?.UnsubscribeStockDataChange(Refresh);
        GameManager.Instance.Market?.UnsubscribeMarketDataChanged(Refresh);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i]?.UnsubscribeClicked(NotifyCardClicked);
        }
    }

    public void Refresh()
    {
        MarketManager market = GameManager.Instance.Market;
        int marketLevel = market.MarketData.CurrentLevel;
        int currentBusinessDay = market.MarketData.CurrentBusinessDay;
        TasteType eventTaste = market.FestivalCalendar.GetNowTaste(currentBusinessDay);
        CategoryType eventCategory = market.FestivalCalendar.GetNowCategory(currentBusinessDay);
        IReadOnlyList<DishType> selectedDishes = market.MarketData.SelectedDishes;
        int eventCardCount = 0;

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            UI_MenuVisualCard card = cards[i];
            DishUpgradeDataSO upgradeData = UpgradeDataDB.GetData(card.DishType);
            bool isUnlocked = upgradeData != null
                && upgradeData.TryGetRequiredMarketLevel(1, out int requiredLevel)
                && requiredLevel <= marketLevel;

            if (isUnlocked)
                continue;

            card.UnsubscribeClicked(NotifyCardClicked);
            cards.RemoveAt(i);
            Destroy(card.gameObject);
        }

        for (int i = 0; i < (int)DishType.Count; i++)
        {
            DishType dishType = (DishType)i;
            DishUpgradeDataSO upgradeData = UpgradeDataDB.GetData(dishType);

            if (upgradeData == null
                || !upgradeData.TryGetRequiredMarketLevel(1, out int requiredLevel)
                || requiredLevel > marketLevel)
            {
                continue;
            }

            UI_MenuVisualCard card = null;

            for (int j = 0; j < cards.Count; j++)
            {
                if (cards[j].DishType == dishType)
                {
                    card = cards[j];
                    break;
                }
            }

            if (card == null)
            {
                card = Instantiate(cardPrefab, cardContainer);
                card.SubscribeClicked(NotifyCardClicked);
                cards.Add(card);
            }

            DishDataSO dishData = DishDataDB.GetData(dishType);
            bool isEventMenu = IsEventMenu(dishData, eventTaste, eventCategory);
            Transform targetContainer = isEventMenu
                ? eventCardContainer
                : cardContainer;

            card.transform.SetParent(targetContainer, false);
            card.transform.SetAsLastSibling();

            if (isEventMenu)
                eventCardCount++;

            int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(dishType);
            int selectedOrder = 0;

            for (int selectedIndex = 0;
                 selectedIndex < selectedDishes.Count;
                 selectedIndex++)
            {
                if (selectedDishes[selectedIndex] != dishType)
                    continue;

                selectedOrder = selectedIndex + 1;
                break;
            }

            card.SetData(dishType);
            card.SetStatus(GetStatus(upgradeData, level));
            card.SetSelectedOrder(selectedOrder);
        }

        eventMenuContainer.SetActive(eventCardCount > 0);
    }

    public void SubscribeCardClicked(Action<DishType> callback)
    {
        onCardClicked += callback;
    }

    public void UnsubscribeCardClicked(Action<DishType> callback)
    {
        onCardClicked -= callback;
    }

    public void NotifyCardClicked(DishType dishType)
    {
        if (dishType == DishType.None || dishType == DishType.Count)
            return;
        onCardClicked?.Invoke(dishType);
    }

    private MenuVisualStatus GetStatus(DishUpgradeDataSO upgradeData, int level)
    {
        if (level >= upgradeData.MaxLevel)
            return MenuVisualStatus.FullUpgraded;

        if (level > 0)
            return MenuVisualStatus.Opened;

        return GameManager.Instance.Upgrade.GetUpgradeAvailability(upgradeData)
            == UpgradeAvailability.Available
            ? MenuVisualStatus.CanOpen
            : MenuVisualStatus.Locked;
    }

    private static bool IsEventMenu(
        DishDataSO dishData,
        TasteType eventTaste,
        CategoryType eventCategory)
    {
        if (dishData == null)
            return false;

        bool matchesTaste = eventTaste != TasteType.Count
            && dishData.Tastes == eventTaste;
        bool matchesCategory = eventCategory != CategoryType.Count
            && dishData.Category == eventCategory;

        return matchesTaste || matchesCategory;
    }

    private void OnUpgradeChanged()
    {
        Refresh();
    }
}
