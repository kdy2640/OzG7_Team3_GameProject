using UnityEngine;
using UnityEngine.UI;

public sealed class UI_SelectMenuVisualCard : MonoBehaviour
{
    [SerializeField] private UI_MenuVisualCard menuVisualCard;
    [SerializeField] private Button deselectButton;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject addMenuOverlay;

    private DishType dishType = DishType.None;
    private HubCanvasController owner;
    private bool canDeselect;
    private bool isLocked;
    private bool isInitialized;

    private void Awake()
    {
        deselectButton.onClick.AddListener(OnDeselectButtonClicked);
        menuVisualCard.SubscribeClicked(OnVisualCardClicked);
    }

    private void OnDestroy()
    {
        deselectButton.onClick.RemoveListener(OnDeselectButtonClicked);
        menuVisualCard.UnsubscribeClicked(OnVisualCardClicked);
    }

    public void Init(HubCanvasController owner, bool canDeselect)
    {
        if (owner == null)
        {
            Debug.LogError($"[{nameof(UI_SelectMenuVisualCard)}] HubCanvasController is required.", this);
            return;
        }

        if (isInitialized && this.owner != owner)
        {
            Debug.LogError(
                $"[{nameof(UI_SelectMenuVisualCard)}] The card was initialized by another owner.",
                this);
            return;
        }

        this.owner = owner;
        this.canDeselect = canDeselect;
        isInitialized = true;
        ApplyInteractionState();
    }

    public void SetData(DishType dishType)
    {
        this.dishType = dishType;
        menuVisualCard.SetData(dishType);
        menuVisualCard.SetStatus(MenuVisualStatus.Opened);
        ApplyInteractionState();
    }

    public void SetLocked(bool isLocked)
    {
        this.isLocked = isLocked;
        lockedOverlay.SetActive(isLocked);
        ApplyInteractionState();
    }

    private void OnDeselectButtonClicked()
    {
        if (!canDeselect || isLocked || dishType == DishType.None)
            return;

        GameManager.Instance.Market.MarketData.DeselectDish(dishType);
    }

    private void OnVisualCardClicked(DishType _)
    {
        if (canDeselect || isLocked || owner == null)
            return;

        owner.RequestStateChange(HubCanvasController.HubCanvasState.MenuManagement);
    }

    private void ApplyInteractionState()
    {
        bool hasDish = dishType != DishType.None && dishType != DishType.Count;

        deselectButton.gameObject.SetActive(canDeselect && hasDish);
        deselectButton.interactable = canDeselect && hasDish && !isLocked;

        menuVisualCard.SetClickEnabled(!canDeselect && !hasDish && !isLocked);
        addMenuOverlay.SetActive(!canDeselect && !hasDish && !isLocked);
    }
}
