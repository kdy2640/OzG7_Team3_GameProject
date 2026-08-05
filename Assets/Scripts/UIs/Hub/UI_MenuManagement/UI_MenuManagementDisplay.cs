using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_MenuManagementDisplay : MonoBehaviour
{
    [Header("Menu Information")]

    [SerializeField] private Image menuIcon;

    [SerializeField] private TMP_Text menuNameText;

    [SerializeField] private TMP_Text priceText;

    [SerializeField] private Button registerButton;

    [Header("Ingredient")]

    [SerializeField] private UI_IngredientCard[] ingredientCards;

    public void SetData(MenuDevelopDetailData data)
    {
        if (data == null) return;

        menuIcon.sprite = data.MenuIcon;
        menuNameText.text = data.MenuName;
        priceText.text = $"{data.Price:N0} G";

        registerButton.interactable = data.CanRegister;

        UpdateIngredientCards(data.RequiredIngredients);
    }

    private void UpdateIngredientCards(List<IngredientCardData> ingredients)
    {
        for (int i = 0; i < ingredientCards.Length; i++)
        {
            if (ingredients != null && i < ingredients.Count)
            {
                ingredientCards[i].gameObject.SetActive(true);
                ingredientCards[i].SetData(ingredients[i]);
            }
            else
            {
                ingredientCards[i].gameObject.SetActive(false);
            }
        }
    }
}

