using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_IngredientCard : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private GroceryType nowType;
    [SerializeField] private Color defaultColor = Color.black;
    [SerializeField] private Color insufficientColor = Color.red;

    public void SetData(GroceryAmount ingredient, int ownedAmount)
    {
        if (ingredient == null)
            return;
        nowType = ingredient.grocery;
        countText.text = $"{ownedAmount} / {ingredient.amount}";

        countText.color = 
            ownedAmount >= ingredient.amount
            ? defaultColor : insufficientColor;

        icon.sprite = GroceryDataDB.GetData(ingredient.grocery).Icon;
    }
}
