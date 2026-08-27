using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuVisualStatus
{
    Opened,
    CanOpen,
    Locked,
    FullUpgraded
}

public class UI_MenuVisualCard : MonoBehaviour
{
    [SerializeField] private UI_EventHandler clickOverlay;

    [SerializeField] private Image backgroundImage;

    [SerializeField] private Image menuIcon;

    [SerializeField] private TMP_Text menuNameText;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private TMP_Text cookText;

    [SerializeField] private TMP_Text priceText;

    [SerializeField] private GameObject canOpen;

    [SerializeField] private TMP_Text developmentStateText;

    [SerializeField] private GameObject lockOverlay;

    [SerializeField] private Image selectVisual;

    [SerializeField] private TMP_Text selectedOrderText;

    [SerializeField] private Color developedColor = Color.yellow;

    [SerializeField] private Color lockedColor = Color.gray;

    [SerializeField] private Color selectedColor = new(0.54f, 0.33f, 0.18f, 1f);

    private event Action<DishType> onClicked;
    private DishType dishType = DishType.None;

    public DishType DishType => dishType;

    private void Awake()
    {
        if (clickOverlay == null)
        {
            Debug.LogError($"[{nameof(UI_MenuVisualCard)}] ClickOverlay is required.", this);
            return;
        }

        clickOverlay.AddUIEvent(HandleClick);
    }

    private void OnDestroy()
    {
        if (clickOverlay != null)
            clickOverlay.OnClickHandler -= HandleClick;
    }

    public void SubscribeClicked(Action<DishType> callback)
    {
        onClicked += callback;
    }

    public void UnsubscribeClicked(Action<DishType> callback)
    {
        onClicked -= callback;
    }

    public void SetClickEnabled(bool isEnabled)
    {
        clickOverlay.enabled = isEnabled;

        Button button = clickOverlay.GetComponent<Button>();
        if (button != null)
            button.enabled = isEnabled;
    }

    public void SetData(DishType dishType)
    {
        this.dishType = dishType;
        SetSelectedOrder(0);

        if (dishType == DishType.None || dishType == DishType.Count)
        {
            ResetData();
            return;
        }

        DishDataSO data = DishDataDB.GetData(dishType);
        if (data == null)
        {
            ResetData();
            return;
        }

        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(dishType);

        menuIcon.sprite = data.Icon;

        menuNameText.text = data.DisplayName;

        levelText.text = $"Lv.{level}";

        int cookableAmount = GameManager.Instance.CookingManager
            .CalculateCookableAmount(dishType);

        cookText.text = $"{cookableAmount:N0}";
        priceText.text = $"{data.Cost:N0}";
    }

    public void SetStatus(MenuVisualStatus status)
    {
        bool isLocked = status == MenuVisualStatus.Locked;
        bool canDevelop = status == MenuVisualStatus.CanOpen;
        bool isDeveloped = status == MenuVisualStatus.Opened
            || status == MenuVisualStatus.FullUpgraded;

        canOpen.SetActive(isLocked || canDevelop);
        lockOverlay.SetActive(isLocked || canDevelop);

        if (isLocked)
            developmentStateText.text = "개발 불가";
        else if (canDevelop)
            developmentStateText.text = "개발 가능!";

        backgroundImage.color = isDeveloped
            ? developedColor
            : lockedColor;
    }

    public void SetSelectedOrder(int order)
    {
        bool isSelected = order > 0;

        selectVisual.gameObject.SetActive(isSelected);

        if (!isSelected)
        {
            selectedOrderText.text = string.Empty;
            return;
        }

        backgroundImage.color = selectedColor;
        selectVisual.color = selectedColor;
        selectedOrderText.text = order.ToString();
    }

    private void HandleClick(PointerEventData _)
    {
        onClicked?.Invoke(dishType);
    }

    private void ResetData()
    {
        dishType = DishType.None;
        menuIcon.sprite = null;
        menuNameText.text = string.Empty;
        levelText.text = string.Empty;
        cookText.text = string.Empty;
        priceText.text = string.Empty;
    }
}
