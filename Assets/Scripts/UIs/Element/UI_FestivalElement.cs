using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum FestivalElementState
{
    InProgress,
    Selectable,
    Unavailable
}

[RequireComponent(typeof(Button))]
public sealed class UI_FestivalElement : MonoBehaviour
{
    [SerializeField] private TMP_Text festivalNameText;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private GameObject lockedIndicator;
    [SerializeField] private GameObject progressIndicator;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private bool isTaste;
    [SerializeField] private int nowEnum;

    private Button button;
    private Action<UI_FestivalElement> onSelected;

    public bool IsTaste => isTaste;
    public int NowEnum => nowEnum;
    public FestivalElementState State { get; private set; } = FestivalElementState.Unavailable;

    private void Awake()
    {
        InitializeButton();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClicked);
    }

    public void SetTaste(TasteType tasteType, Action<UI_FestivalElement> onSelected)
    {
        InitializeButton();
        isTaste = true;
        nowEnum = (int)tasteType;
        this.onSelected = onSelected;
        festivalNameText.text = tasteType.ToString();
        Refresh();
    }

    public void SetCategory(CategoryType categoryType, Action<UI_FestivalElement> onSelected)
    {
        InitializeButton();
        isTaste = false;
        nowEnum = (int)categoryType;
        this.onSelected = onSelected;
        festivalNameText.text = categoryType.ToString();
        Refresh();
    }

    public void Refresh()
    {
        MarketManager market = GameManager.Instance?.Market;

        if (market == null)
        {
            ApplyState(FestivalElementState.Unavailable, 0);
            return;
        }

        int businessDay = market.MarketData.CurrentBusinessDay;

        if (isTaste)
        {
            TasteType tasteType = (TasteType)nowEnum;
            bool isInProgress = market.FestivalCalendar.GetNowTaste(businessDay) == tasteType;
            int progressDay = businessDay - market.FestivalCalendar.TasteStartBusinessDay + 1;

            ApplyState(
                isInProgress
                    ? FestivalElementState.InProgress
                    : market.CanStartTasteFestival(tasteType)
                        ? FestivalElementState.Selectable
                        : FestivalElementState.Unavailable,
                progressDay);
            return;
        }

        CategoryType categoryType = (CategoryType)nowEnum;
        bool isCategoryInProgress = market.FestivalCalendar.GetNowCategory(businessDay) == categoryType;
        int categoryProgressDay = businessDay - market.FestivalCalendar.CategoryStartBusinessDay + 1;

        ApplyState(
            isCategoryInProgress
                ? FestivalElementState.InProgress
                : market.CanStartCategoryFestival(categoryType)
                    ? FestivalElementState.Selectable
                    : FestivalElementState.Unavailable,
            categoryProgressDay);
    }

    private void ApplyState(FestivalElementState state, int progressDay)
    {
        State = state;

        bool isInProgress = state == FestivalElementState.InProgress;
        bool isUnavailable = state == FestivalElementState.Unavailable;

        button.interactable = !isUnavailable;
        lockedPanel.SetActive(isUnavailable);
        lockedIndicator.SetActive(isUnavailable);
        progressIndicator.SetActive(isInProgress);

        if (isInProgress)
            progressText.text = $"진행중!\n{progressDay}일차";
    }

    private void HandleClicked()
    {
        if (State != FestivalElementState.Unavailable)
            onSelected?.Invoke(this);
    }

    private void InitializeButton()
    {
        if (button != null)
            return;

        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClicked);
    }
}
