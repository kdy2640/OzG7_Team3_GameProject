using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_IngredientCard : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private GroceryType nowType;

    public void SetData(GroceryAmount ingredient, int ownedAmount)
    {
        if (ingredient == null)
            return;
        nowType = ingredient.grocery;
        countText.text = $"{ownedAmount} / {ingredient.amount}";

        countText.color = 
            ownedAmount >= ingredient.amount
            ? Color.white : Color.red;

        icon.sprite = GroceryDataDB.GetData(ingredient.grocery).Icon;
    }
}
