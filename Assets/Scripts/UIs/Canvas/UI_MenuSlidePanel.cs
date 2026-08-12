using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuSlidePanel : MonoBehaviour
{
    // TODO
    // - UI_MenuDevelopCard에 ClickOverlay를 추가한다.
    // - 카드 클릭 시 슬라이더 내부에 현재 선택 중인 DishType을 표시하고,
    //   선택 변경 이벤트를 UI_MenuVisualizer로 전달한다.
    // - 슬라이더 내부에 현재 선택 중인 DishType과
    //   UI_SelectedMenuPanel에 등록된 DishType을 함께 표시하고 상태를 구분한다.
    // - 매장 레벨별 최대 DishType 수를 가져와 필요한 카드만 동적으로 생성하거나 회수한다.
    // - 이벤트 기간의 DishType은 별도의 UI_MenuContainer로 이동하고,
    //   이벤트 종료 시 원래 컨테이너로 복귀시킨다.
    // - ScrollRect와 Viewport Mask를 적용해 UI_MenuContainer의 양 끝이 잘리도록 만들고
    //   카드 목록이 슬라이더처럼 보이게 한다.
    // - Refresh 후 선택 중인 DishType이 사라지거나 이동한 경우 선택 상태를 해제하거나 갱신한다.
    // - 동적 생성한 카드의 클릭 이벤트는 카드 회수 및 파괴 시 해제한다.

    [SerializeField] private UI_MenuContainer menuContainer;
    [SerializeField] private List<UI_MenuVisualCard> cards = new();

    private bool isInitialized;
    private event Action<DishType> onCardClicked;

    public void Init()
    {
        if (isInitialized)
            return;

        isInitialized = true;
        menuContainer.SetCards(cards);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i]?.SubscribeClicked(NotifyCardClicked);
        }

        GameManager.Instance.Upgrade.SubscribeUpgradeChanged(OnUpgradeChanged);
        GameManager.Instance.StockManager.SubscribeStockDataChange(Refresh);

        Refresh();
    }

    private void OnDestroy()
    {
        if (!isInitialized || GameManager.Instance == null)
            return;

        GameManager.Instance.Upgrade?.UnsubscribeUpgradeChanged(OnUpgradeChanged);
        GameManager.Instance.StockManager?.UnsubscribeStockDataChange(Refresh);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i]?.UnsubscribeClicked(NotifyCardClicked);
        }
    }

    public void Refresh()
    {
        int cardCount = Mathf.Min(cards.Count, (int)DishType.Count);

        for (int i = 0; i < cardCount; i++)
        {
            UI_MenuVisualCard card = cards[i];
            DishType dishType = (DishType)i;
            DishUpgradeDataSO upgradeData = UpgradeDataDB.GetData(dishType);

            if (card == null || upgradeData == null)
                continue;

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

        return GameManager.Instance.StockManager.CanConsumeCurrency(upgradeData.GetCosts(0))
            ? MenuVisualStatus.CanOpen
            : MenuVisualStatus.Locked;
    }

    private void OnUpgradeChanged()
    {
        Refresh();
    }
}
