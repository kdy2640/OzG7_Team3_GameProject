using System.Collections;
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
    [SerializeField] private TMP_Text currentLevelTextShadow;
    [SerializeField] private TMP_Text currentSellPriceText;
    [SerializeField] private Image[] currentLevelSlots;

    [Header("Next Level")]
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text nextLevelTextShadow;
    [SerializeField] private TMP_Text nextSellPriceText;
    [SerializeField] private TMP_Text nextSellPriceTextShadow;
    [SerializeField] private Image[] nextLevelSlots;

    [Header("Level Slot Colors")]
    [SerializeField] private Color filledSlotColor = new(0.21f, 0.64f, 0.58f);
    [SerializeField] private Color nextFilledSlotColor = new(0.95f, 0.64f, 0.17f);
    [SerializeField] private Color emptySlotColor = new(0.88f, 0.82f, 0.65f);

    [Header("Ingredient")]
    [SerializeField] private UI_IngredientCard[] ingredientCards;

    [Header("Status")]
    [SerializeField] private GameObject warningBox;
    [SerializeField] private TMP_Text warningText;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeButtonText;
    [SerializeField] private GameObject upgradeLock;
    [SerializeField] private PanelAnimator panelAnimator;

    private DishType currentDishType = DishType.None;

    private void Awake()
    {
        cancelButton.onClick.AddListener(Close);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        Hide();
    }

    private void OnDestroy()
    {
        cancelButton.onClick.RemoveListener(Close);
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

        bool wasActive = gameObject.activeSelf;

        currentDishType = dishType;
        gameObject.SetActive(true);
        Refresh(dishData, upgradeData);

        if (!wasActive)
            StartCoroutine(panelAnimator.Show());
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

        string currentLevelLabel = $"Lv.{currentLevel}";
        currentLevelText.text = currentLevelLabel;
        currentLevelTextShadow.text = currentLevelLabel;
        currentSellPriceText.text =
            $"{GetSellPrice(currentDishType, currentLevel):N0}";

        string nextLevelLabel = isMaxLevel ? "MAX" : $"Lv.{nextLevel}";
        nextLevelText.text = nextLevelLabel;
        nextLevelTextShadow.text = nextLevelLabel;

        string nextSellPriceLabel = isMaxLevel
            ? "-"
            : $"{GetSellPrice(currentDishType, nextLevel):N0}";
        nextSellPriceText.text = nextSellPriceLabel;
        nextSellPriceTextShadow.text = nextSellPriceLabel;

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
        bool canUpgrade = availability == UpgradeAvailability.Available;
        upgradeButton.interactable = canUpgrade;
        upgradeLock.SetActive(!canUpgrade);
        upgradeButtonText.text = isMaxLevel
            ? "최대 레벨"
            : currentLevel <= 0 ? "메뉴 개발" : "레벨 업";
    }

    private void UpdateLevelSlots(
        Image[] slots,
        int displayedLevel,
        Color filledColor)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color = i < displayedLevel
                ? filledColor
                : emptySlotColor;
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
        warningBox.SetActive(availability != UpgradeAvailability.Available);
        warningText.color = new Color(0.82f, 0.23f, 0.18f);

        switch (availability)
        {
            case UpgradeAvailability.Available:
                warningText.text = string.Empty;
                break;

            case UpgradeAvailability.InsufficientIngredients:
                warningText.text = "재료가 충분하지 않습니다.";
                break;

            case UpgradeAvailability.MarketLevelLocked:
                if (upgradeData.TryGetRequiredMarketLevel(
                        currentLevel + 1,
                        out int requiredMarketLevel))
                {
                    warningText.text =
                        $"시장 Lv.{requiredMarketLevel} 달성이 필요합니다.";
                }
                else
                {
                    warningText.text = "필요 시장 레벨 정보를 확인할 수 없습니다.";
                }
                break;

            case UpgradeAvailability.MaxLevel:
                warningText.color = new Color(0.21f, 0.64f, 0.58f);
                warningText.text = "최대 레벨입니다.";
                break;

            default:
                warningText.text = "업그레이드 정보를 확인할 수 없습니다.";
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
            if (GameManager.Instance.Upgrade.CanUpgrade(upgradeData))
            {
                DishDataSO refreshedDishData = DishDataDB.GetData(currentDishType);
                Refresh(refreshedDishData, upgradeData);
                return;
            }

            Close();
            return;
        }

        DishDataSO dishData = DishDataDB.GetData(currentDishType);
        if (dishData != null)
            Refresh(dishData, upgradeData);
    }

    private void Close()
    {
        if (!gameObject.activeSelf)
            return;

        currentDishType = DishType.None;
        StartCoroutine(HideAnimated());
    }

    private IEnumerator HideAnimated()
    {
        yield return panelAnimator.Hide();
        gameObject.SetActive(false);
    }

    private static int GetSellPrice(
        DishType dishType,
        int level)
    {
        return DishPriceCalculator.BasicPriceCalculate(dishType, level);
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
