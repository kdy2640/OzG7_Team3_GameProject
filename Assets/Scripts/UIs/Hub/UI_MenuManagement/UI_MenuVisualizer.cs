using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_MenuVisualizer : MonoBehaviour
{
    [SerializeField] private UI_MenuSlidePanel menuSlidePanel;

    [Header("Menu Information")]
    [SerializeField] private Image menuIcon;
    [SerializeField] private TMP_Text menuNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text sellValueText;
    [SerializeField] private TMP_Text cookValueText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Taste")]
    [SerializeField] private UI_TasteCard[] tasteCards;

    [Header("Ingredient")]
    [SerializeField] private TMP_Text ingredientTitleText;
    [SerializeField] private UI_IngredientCard[] ingredientCards;

    [Header("Status")]
    [SerializeField] private GameObject isDevelopedPanel;
    [SerializeField] private GameObject isFullUpgradedPanel;
    [SerializeField] private GameObject isNonDevelopedPanel;

    [Header("Buttons")]
    [SerializeField] private Button menuSelectionButton;
    [SerializeField] private TMP_Text menuSelectionButtonText;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Button fullUpgradedMenuSelectionButton;
    [SerializeField] private TMP_Text fullUpgradedMenuSelectionButtonText;
    [SerializeField] private Button menuDevelopButton;

    private DishType currentDishType = DishType.None;
    private bool isMarketSubscribed;

    private void Awake()
    {
        menuSelectionButton.onClick.AddListener(OnSelectionButtonClicked);
        fullUpgradedMenuSelectionButton.onClick.AddListener(OnSelectionButtonClicked);
        levelUpButton.onClick.AddListener(OnUpgradeButtonClicked);
        menuDevelopButton.onClick.AddListener(OnUpgradeButtonClicked);

        if (menuSlidePanel == null)
        {
            Debug.LogError($"[{nameof(UI_MenuVisualizer)}] UI_MenuSlidePanel is required.", this);
            return;
        }

        menuSlidePanel.SubscribeCardClicked(SetData);
    }

    private void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.Market == null)
            return;

        GameManager.Instance.Market.SubscribeMarketDataChanged(OnMarketDataChanged);
        isMarketSubscribed = true;
        UpdateSelectionButtons();
    }

    private void OnDestroy()
    {
        menuSelectionButton.onClick.RemoveListener(OnSelectionButtonClicked);
        fullUpgradedMenuSelectionButton.onClick.RemoveListener(OnSelectionButtonClicked);
        levelUpButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        menuDevelopButton.onClick.RemoveListener(OnUpgradeButtonClicked);

        if (menuSlidePanel != null)
            menuSlidePanel.UnsubscribeCardClicked(SetData);

        if (isMarketSubscribed && GameManager.Instance != null)
            GameManager.Instance.Market?.UnsubscribeMarketDataChanged(OnMarketDataChanged);
    }

    public void SetData(DishType dishType)
    {
        if (dishType == DishType.None)
        {
            ResetData();
            return;
        }

        DishDataSO data = DishDataDB.GetData(dishType);
        if (data == null)
            return;

        currentDishType = dishType;

        int level = GameManager.Instance.Upgrade.GetLevel(dishType);
        DishUpgradeDataSO upgradeData = UpgradeDataDB.GetData(dishType);

        menuIcon.gameObject.SetActive(true);
        menuIcon.sprite = data.Icon;
        menuNameText.text = data.DisplayName;
        levelText.text = $"LV.{level}";
        sellValueText.text = $"G {data.Cost:N0}";
        cookValueText.text = $"C {GameManager.Instance.CookingManager.CalculateCookableAmount(dishType):N0}";
        descriptionText.text = data.Description;

        ingredientTitleText.text = level <= 0
            ? "Ingredients for Develop"
            : "Ingredients for Cooking";

        UpdateTasteCards(data.Tastes);
        UpdateIngredientCards(data.Ingredients);
        UpdateStatusPanels(level, upgradeData);
        UpdateSelectionButtons();
    }

    private void ResetData()
    {
        currentDishType = DishType.None;

        menuIcon.sprite = null;
        menuIcon.gameObject.SetActive(false);

        menuNameText.text = string.Empty;
        levelText.text = string.Empty;
        sellValueText.text = string.Empty;
        cookValueText.text = string.Empty;
        descriptionText.text = string.Empty;
        ingredientTitleText.text = string.Empty;

        for (int i = 0; i < tasteCards.Length; i++)
        {
            tasteCards[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < ingredientCards.Length; i++)
        {
            ingredientCards[i].gameObject.SetActive(false);
        }

        isDevelopedPanel.SetActive(false);
        isFullUpgradedPanel.SetActive(false);
        isNonDevelopedPanel.SetActive(false);

        UpdateSelectionButtons();
    }

    private void UpdateTasteCards(List<TasteType> tastes)
    {
        for (int i = 0; i < tasteCards.Length; i++)
        {
            bool hasTaste = tastes != null
                && i < tastes.Count
                && tastes[i] != TasteType.Count;

            tasteCards[i].gameObject.SetActive(hasTaste);

            if (hasTaste)
                tasteCards[i].SetData(tastes[i]);
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

    private void UpdateStatusPanels(int level, DishUpgradeDataSO upgradeData)
    {
        bool isNonDeveloped = level <= 0;
        bool isFullUpgraded = upgradeData != null && level >= upgradeData.MaxLevel;

        isNonDevelopedPanel.SetActive(isNonDeveloped);
        isFullUpgradedPanel.SetActive(!isNonDeveloped && isFullUpgraded);
        isDevelopedPanel.SetActive(!isNonDeveloped && !isFullUpgraded);
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentDishType == DishType.None)
            return;

        DishUpgradeDataSO upgradeData = UpgradeDataDB.GetData(currentDishType);
        if (upgradeData == null)
            return;

        if (GameManager.Instance.Upgrade.TryUpgrade(upgradeData))
            SetData(currentDishType);
    }

    private void OnSelectionButtonClicked()
    {
        if (currentDishType == DishType.None)
            return;

        MarketData marketData = GameManager.Instance.Market.MarketData;

        if (IsCurrentDishSelected(marketData.SelectedDishes))
        {
            marketData.DeselectDish(currentDishType);
            return;
        }

        if (marketData.SelectedDishes.Count
            >= GameManager.Instance.Market.LevelData.MaxDishLimit)
        {
            return;
        }

        marketData.SelectDish(currentDishType);
    }

    private void OnMarketDataChanged()
    {
        UpdateSelectionButtons();
    }

    private void UpdateSelectionButtons()
    {
        bool hasDish = currentDishType != DishType.None;
        IReadOnlyList<DishType> selectedDishes = GameManager.Instance?.Market?.MarketData?.SelectedDishes;
        bool isSelected = hasDish && IsCurrentDishSelected(selectedDishes);
        bool canSelect = selectedDishes != null
            && selectedDishes.Count < GameManager.Instance.Market.LevelData.MaxDishLimit;
        string buttonText = isSelected ? "Deselect" : "Select";

        menuSelectionButtonText.text = buttonText;
        fullUpgradedMenuSelectionButtonText.text = buttonText;

        menuSelectionButton.interactable = hasDish && (isSelected || canSelect);
        fullUpgradedMenuSelectionButton.interactable = hasDish && (isSelected || canSelect);
    }

    private bool IsCurrentDishSelected(IReadOnlyList<DishType> selectedDishes)
    {
        if (selectedDishes == null)
            return false;

        for (int i = 0; i < selectedDishes.Count; i++)
        {
            if (selectedDishes[i] == currentDishType)
                return true;
        }

        return false;
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
