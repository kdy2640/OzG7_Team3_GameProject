using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_MenuVisualizer : MonoBehaviour
{
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

    [Header("Upgrade Data")]
    [SerializeField] private List<DishUpgradeDataSO> upgradeDatas = new();

    public void SetData(DishType dishType)
    {
        DishDataSO data = DishDataDB.GetData(dishType);
        if (data == null)
            return;

        int level = GameManager.Instance.Upgrade.RuntimeStat.Dish.GetLevel(dishType);
        DishUpgradeDataSO upgradeData = GetUpgradeData(dishType);

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
    }

    private DishUpgradeDataSO GetUpgradeData(DishType dishType)
    {
        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            DishUpgradeDataSO upgradeData = upgradeDatas[i];
            if (upgradeData != null && upgradeData.TargetDish == dishType)
                return upgradeData;
        }

        return null;
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
