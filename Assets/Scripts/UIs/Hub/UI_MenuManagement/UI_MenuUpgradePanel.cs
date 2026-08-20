using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_MenuUpgradePanel : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private Image menuIcon;
    [SerializeField] private TMP_Text menuNameText;

    [Header("Current Level")]
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text currentSellPriceText;
    [SerializeField] private Image[] currentLevelSlots;

    [Header("Next Level")]
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text nextSellPriceText;
    [SerializeField] private Image[] nextLevelSlots;

    [Header("Level Slot Colors")]
    [SerializeField] private Color filledSlotColor = new(0.21f, 0.64f, 0.58f);
    [SerializeField] private Color nextFilledSlotColor = new(0.95f, 0.64f, 0.17f);
    [SerializeField] private Color emptySlotColor = new(0.88f, 0.82f, 0.65f);

    [Header("Ingredient")]
    [SerializeField] private UI_IngredientCard[] ingredientCards;

    [Header("Status")]
    [SerializeField] private TMP_Text warningText;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeButtonText;

    private DishType currentDishType = DishType.None;

    private void Awake()
    {
        cancelButton.onClick.AddListener(Hide);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        Hide();
    }

    private void OnDestroy()
    {
        cancelButton.onClick.RemoveListener(Hide);
        upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
    }

    public void Show(DishType dishType)
    {
        if (dishType == DishType.None || dishType == DishType.Count)
            return;

        DishDataSO dishData = DishDataDB.GetData(dishType);
        DishUpgradeDataSO upgradeData = UpgradeDataDB.GetData(dishType);

        if (dishData == null || upgradeData == null)
            return;

        currentDishType = dishType;
        gameObject.SetActive(true);
        Refresh(dishData, upgradeData);
    }

    public void Hide()
    {
        currentDishType = DishType.None;
        gameObject.SetActive(false);
    }

    private void Refresh(DishDataSO dishData, DishUpgradeDataSO upgradeData)
    {
        UpgradeManager upgradeManager = GameManager.Instance.Upgrade;
        int currentLevel = upgradeManager.RuntimeLevel.Get(currentDishType);
        int nextLevel = currentLevel + 1;
        bool isMaxLevel = currentLevel >= upgradeData.MaxLevel;

        menuIcon.sprite = dishData.Icon;
        menuNameText.text = dishData.DisplayName;

        currentLevelText.text = $"Lv.{currentLevel}";
        currentSellPriceText.text = $"G {GetSellPrice(upgradeData, currentLevel):N0}";

        nextLevelText.text = isMaxLevel ? "MAX" : $"Lv.{nextLevel}";
        nextSellPriceText.text = isMaxLevel
            ? "-"
            : $"G {GetSellPrice(upgradeData, nextLevel):N0}";

        UpdateLevelSlots(currentLevelSlots, currentLevel, filledSlotColor);
        UpdateLevelSlots(
            nextLevelSlots,
            isMaxLevel ? currentLevel : nextLevel,
            nextFilledSlotColor);

        List<GroceryAmount> requiredIngredients = null;
        if (!isMaxLevel)
        {
            upgradeData.TryGetRequiredIngredients(
                nextLevel,
                out requiredIngredients);
        }

        UpdateIngredientCards(requiredIngredients);

        UpgradeAvailability availability =
            upgradeManager.GetUpgradeAvailability(upgradeData);

        UpdateStatus(availability, upgradeData, currentLevel);
        upgradeButton.interactable =
            availability == UpgradeAvailability.Available;
        upgradeButtonText.text = isMaxLevel
            ? "MAX"
            : currentLevel <= 0 ? "Develop" : "Level Up";
    }

    private void UpdateLevelSlots(
        Image[] slots,
        int displayedLevel,
        Color filledColor)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].color = i < displayedLevel
                    ? filledColor
                    : emptySlotColor;
            }
        }
    }

    private void UpdateIngredientCards(List<GroceryAmount> ingredients)
    {
        for (int i = 0; i < ingredientCards.Length; i++)
        {
            bool hasIngredient = ingredients != null && i < ingredients.Count;
            ingredientCards[i].gameObject.SetActive(hasIngredient);

            if (hasIngredient)
            {
                ingredientCards[i].SetData(
                    ingredients[i],
                    GetOwnedAmount(ingredients[i].grocery));
            }
        }
    }

    private void UpdateStatus(
        UpgradeAvailability availability,
        DishUpgradeDataSO upgradeData,
        int currentLevel)
    {
        warningText.color = new Color(0.82f, 0.23f, 0.18f);

        switch (availability)
        {
            case UpgradeAvailability.Available:
                warningText.text = string.Empty;
                break;

            case UpgradeAvailability.InsufficientIngredients:
                warningText.text = "Not enough ingredients.";
                break;

            case UpgradeAvailability.MarketLevelLocked:
                if (upgradeData.TryGetRequiredMarketLevel(
                        currentLevel + 1,
                        out int requiredMarketLevel))
                {
                    warningText.text =
                        $"Market Lv.{requiredMarketLevel} required.";
                }
                else
                {
                    warningText.text = "Market level requirement unavailable.";
                }
                break;

            case UpgradeAvailability.MaxLevel:
                warningText.color = new Color(0.21f, 0.64f, 0.58f);
                warningText.text = "Max Level";
                break;

            default:
                warningText.text = "Upgrade data unavailable.";
                break;
        }
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentDishType == DishType.None)
            return;

        DishUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(currentDishType);

        if (upgradeData == null)
            return;

        if (GameManager.Instance.Upgrade.TryUpgrade(upgradeData))
        {
            Hide();
            return;
        }

        DishDataSO dishData = DishDataDB.GetData(currentDishType);
        if (dishData != null)
            Refresh(dishData, upgradeData);
    }

    private static int GetSellPrice(
        DishUpgradeDataSO upgradeData,
        int level)
    {
        if (level <= 0
            || !upgradeData.TryGetRequiredCost(level, out int sellPrice))
        {
            return 0;
        }

        return sellPrice;
    }

    private static int GetOwnedAmount(GroceryType groceryType)
    {
        IReadOnlyList<GroceryAmount> groceries =
            GameManager.Instance.StockManager.StockData.Groceries;
        int ownedAmount = 0;

        for (int i = 0; i < groceries.Count; i++)
        {
            GroceryAmount grocery = groceries[i];
            if (grocery != null && grocery.grocery == groceryType)
                ownedAmount += grocery.amount;
        }

        return ownedAmount;
    }
}
