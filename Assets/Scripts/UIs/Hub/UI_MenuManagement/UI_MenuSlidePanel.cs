using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuSlidePanel : MonoBehaviour
{
    // TODO
    // - 카드 클릭 시 슬라이더 내부에 현재 선택 중인 DishType을 표시한다.
    // - 슬라이더 내부에 현재 선택 중인 DishType과
    //   UI_SelectedMenuPanel에 등록된 DishType을 함께 표시하고 상태를 구분한다.
    // - 이벤트 기간의 DishType은 별도의 UI_MenuContainer로 이동하고,
    //   이벤트 종료 시 원래 컨테이너로 복귀시킨다.
    // - ScrollRect와 Viewport Mask를 적용해 UI_MenuContainer의 양 끝이 잘리도록 만들고
    //   카드 목록이 슬라이더처럼 보이게 한다.
    // - Refresh 후 선택 중인 DishType이 사라지거나 이동한 경우 선택 상태를 해제하거나 갱신한다.

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
        int marketLevel = GameManager.Instance.Market.MarketData.CurrentLevel;

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

            int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(dishType);

            card.SetData(dishType);
            card.SetStatus(GetStatus(upgradeData, level));
        }
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
        if (level > 0)
            return MenuVisualStatus.Opened;

        return GameManager.Instance.Upgrade.GetUpgradeAvailability(upgradeData)
            == UpgradeAvailability.Available
            ? MenuVisualStatus.CanOpen
            : MenuVisualStatus.Locked;
    }

    private void OnUpgradeChanged()
    {
        Refresh();
    }
}
