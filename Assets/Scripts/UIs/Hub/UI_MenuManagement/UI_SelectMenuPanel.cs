using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectMenuPanel : MonoBehaviour
{
    private const float MinCardHeight = 100f;
    private const float MaxCardHeight = 140f;

    [SerializeField] private List<UI_SelectMenuVisualCard> cards = new();
    [SerializeField] private RectTransform gridPanel;
    [SerializeField] private VerticalLayoutGroup gridLayout;

    private bool canDeselect;
    private bool isInitialized;

    public void SetCanDeselect(bool canDeselect)
    {
        this.canDeselect = canDeselect;
    }

    public void Init(HubCanvasController owner)
    {
        if (isInitialized)
            return;

        isInitialized = true;

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i]?.Init(owner, canDeselect);
        }

        GameManager.Instance.Market.SubscribeMarketDataChanged(Refresh);
        Refresh();
    }

    private void OnDestroy()
    {
        if (isInitialized && GameManager.Instance != null)
            GameManager.Instance.Market?.UnsubscribeMarketDataChanged(Refresh);
    }

    public void Refresh()
    {
        int maxDish = GameManager.Instance.Market.LevelData.MaxDishLimit;
        IReadOnlyList<DishType> selectedDish = GameManager.Instance.Market.MarketData.SelectedDishes;

        for (int i = 0; i < cards.Count; i++)
        {
            bool isAvailable = i < maxDish;
            cards[i].gameObject.SetActive(canDeselect || isAvailable);

            if (!canDeselect && !isAvailable)
                continue;

            cards[i].SetData(i < selectedDish.Count ? selectedDish[i] : DishType.None);
            cards[i].SetLocked(canDeselect && !isAvailable);
        }

        UpdateCardLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isInitialized || gridPanel == null || gridLayout == null)
            return;

        UpdateCardLayout();
    }

    private void UpdateCardLayout()
    {
        int activeCardCount = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].gameObject.activeSelf)
                activeCardCount++;
        }

        if (activeCardCount <= 0)
            return;

        float totalSpacing = gridLayout.spacing * (activeCardCount - 1);
        float availableHeight = gridPanel.rect.height
            - gridLayout.padding.vertical
            - totalSpacing;
        float cardHeight = Mathf.Clamp(
            availableHeight / activeCardCount,
            MinCardHeight,
            MaxCardHeight);

        for (int i = 0; i < cards.Count; i++)
        {
            if (!cards[i].gameObject.activeSelf)
                continue;

            LayoutElement layoutElement = cards[i].GetComponent<LayoutElement>();
            layoutElement.minHeight = MinCardHeight;
            layoutElement.preferredHeight = cardHeight;
            layoutElement.flexibleHeight = 0f;
        }

        LayoutRebuilder.MarkLayoutForRebuild(gridPanel);
    }
}
