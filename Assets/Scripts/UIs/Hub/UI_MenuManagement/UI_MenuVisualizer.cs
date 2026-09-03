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
    [SerializeField] private TMP_Text levelTextShadow;
    [SerializeField] private TMP_Text sellValueText;
    [SerializeField] private TMP_Text cookValueText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Taste & Category")]
    [SerializeField] private RectTransform[] tasteCards;
    [SerializeField] private RectTransform[] categoryCards;

    [Header("Level")]
    [SerializeField] private Image[] levelFrontSlots;

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
    [SerializeField] private Button menuDevelopButton;

    private DishType currentDishType = DishType.None;
    private bool isMarketSubscribed;
    private bool isUpgradeSubscribed;
    private UI_MenuUpgradePanel menuUpgradePanel;

    private void Awake()
    {
        menuSelectionButton.onClick.AddListener(OnSelectionButtonClicked);
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
        GameManager.Instance.Upgrade.SubscribeUpgradeChanged(OnUpgradeChanged);
        isUpgradeSubscribed = true;
        UpdateSelectionButtons();
    }

    private void OnDestroy()
    {
        menuSelectionButton.onClick.RemoveListener(OnSelectionButtonClicked);
        levelUpButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        menuDevelopButton.onClick.RemoveListener(OnUpgradeButtonClicked);

        if (menuSlidePanel != null)
            menuSlidePanel.UnsubscribeCardClicked(SetData);

        if (isMarketSubscribed && GameManager.Instance != null)
            GameManager.Instance.Market?.UnsubscribeMarketDataChanged(OnMarketDataChanged);

        if (isUpgradeSubscribed && GameManager.Instance != null)
            GameManager.Instance.Upgrade?.UnsubscribeUpgradeChanged(OnUpgradeChanged);
    }

    public void SetUpgradePanel(UI_MenuUpgradePanel upgradePanel)
    {
        menuUpgradePanel = upgradePanel;
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

        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(dishType);
        DishUpgradeDataSO upgradeData = UpgradeDataDB.GetData(dishType);
        string levelLabel = $"LV.{level}";

        menuIcon.gameObject.SetActive(true);
        menuIcon.sprite = data.Icon;
        menuNameText.text = data.DisplayName;
        levelText.text = levelLabel;
        levelTextShadow.text = levelLabel;
        sellValueText.text =
            $"{DishPriceCalculator.BasicPriceCalculate(dishType):N0}";
        cookValueText.text =
            $"{GameManager.Instance.CookingManager.CalculateCookableAmount(dishType):N0}";
        descriptionText.text = data.Description;

        bool isMaxLevel = upgradeData != null && level >= upgradeData.MaxLevel;
        List<GroceryAmount> displayedIngredients = null;

        if (isMaxLevel)
        {
            ingredientTitleText.text = "조리 필요 재료 목록";
            displayedIngredients = data.Ingredients;
        }
        else
        {
            ingredientTitleText.text = level <= 0
                ? "개발 필요 재료 목록"
                : "업그레이드 필요 재료 목록";

            upgradeData?.TryGetRequiredIngredients(
                level + 1,
                out displayedIngredients);
        }

        UpdateTasteCards(data.Tastes, data.Category);
        UpdateLevelSlots(level);
        UpdateIngredientCards(displayedIngredients);
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
        levelTextShadow.text = string.Empty;
        sellValueText.text = string.Empty;
        cookValueText.text = string.Empty;
        descriptionText.text = string.Empty;
        ingredientTitleText.text = string.Empty;

        for (int i = 0; i < tasteCards.Length; i++)
        {
            tasteCards[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < categoryCards.Length; i++)
        {
            categoryCards[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < ingredientCards.Length; i++)
        {
            ingredientCards[i].gameObject.SetActive(false);
        }

        UpdateLevelSlots(0);

        isDevelopedPanel.SetActive(false);
        isFullUpgradedPanel.SetActive(false);
        isNonDevelopedPanel.SetActive(false);

        UpdateSelectionButtons();
    }

    private void UpdateTasteCards(TasteType taste, CategoryType category)
    {
        for (int i = 0; i < tasteCards.Length; i++)
            tasteCards[i].gameObject.SetActive(false);

        for (int i = 0; i < categoryCards.Length; i++)
            categoryCards[i].gameObject.SetActive(false);

        if (taste != TasteType.Count)
            tasteCards[(int)taste].gameObject.SetActive(true);

        if (category != CategoryType.Count)
            categoryCards[(int)category].gameObject.SetActive(true);
    }

    private void UpdateLevelSlots(int level)
    {
        for (int i = 0; i < levelFrontSlots.Length; i++)
            levelFrontSlots[i].gameObject.SetActive(i < level);
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
        isDevelopedPanel.SetActive(!isNonDeveloped);
        levelUpButton.interactable = !isFullUpgraded;
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentDishType == DishType.None || menuUpgradePanel == null)
            return;

        menuUpgradePanel.Show(currentDishType);
    }

    private void OnSelectionButtonClicked()
    {
        if (currentDishType == DishType.None)
            return;

        MarketData marketData = GameManager.Instance.Market.MarketData;

        if (IsCurrentDishSelected(marketData.SelectedDishes))
        {
            if (!marketData.DeselectDish(currentDishType))
                return;

            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Hub_MenuDeselect);
            return;
        }

        if (marketData.SelectedDishes.Count
            >= GameManager.Instance.Market.LevelData.MaxDishLimit)
        {
            return;
        }

        int dishLevel = GameManager.Instance.Upgrade.RuntimeLevel.Get(currentDishType);

        if (!marketData.SelectDish(currentDishType, dishLevel))
            return;

        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Hub_MenuSelect);
    }

    private void OnMarketDataChanged()
    {
        UpdateSelectionButtons();
    }

    private void OnUpgradeChanged()
    {
        if (currentDishType != DishType.None)
            SetData(currentDishType);
    }

    private void UpdateSelectionButtons()
    {
        bool hasDish = currentDishType != DishType.None;
        IReadOnlyList<DishType> selectedDishes = GameManager.Instance?.Market?.MarketData?.SelectedDishes;
        bool isSelected = hasDish && IsCurrentDishSelected(selectedDishes);
        int dishLevel = hasDish
            ? GameManager.Instance.Upgrade.RuntimeLevel.Get(currentDishType)
            : 0;
        bool canSelect = selectedDishes != null
            && selectedDishes.Count < GameManager.Instance.Market.LevelData.MaxDishLimit
            && dishLevel > 0;
        string buttonText = isSelected ? "메뉴 해제" : "메뉴 추가";

        menuSelectionButtonText.text = buttonText;

        menuSelectionButton.interactable = hasDish && (isSelected || canSelect);
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
