using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuVisualStatus
{
    Opened,
    CanOpen,
    Locked
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

    [SerializeField] private GameObject lockOverlay;

    [SerializeField] private Color developedColor = Color.yellow;

    [SerializeField] private Color lockedColor = Color.gray;

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

        int level = GameManager.Instance.Upgrade.GetLevel(dishType);

        menuIcon.sprite = data.Icon;

        menuNameText.text = data.DisplayName;

        levelText.text = $"Lv.{level}";

        int cookableAmount = GameManager.Instance.CookingManager
            .CalculateCookableAmount(dishType);

        cookText.text = $"C {cookableAmount:N0}";
        priceText.text = $"G {data.Cost:N0}";
    }

    public void SetStatus(MenuVisualStatus status)
    {
        bool isOpened = status == MenuVisualStatus.Opened;

        canOpen.SetActive(status == MenuVisualStatus.CanOpen);
        lockOverlay.SetActive(status == MenuVisualStatus.Locked);

        backgroundImage.color = isOpened
            ? developedColor : lockedColor;
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
