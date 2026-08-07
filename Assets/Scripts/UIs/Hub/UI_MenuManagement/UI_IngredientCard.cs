using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_IngredientCard : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    public void SetData(GroceryAmount ingredient, int ownedAmount)
    {
        if (ingredient == null)
            return;

        countText.text = $"{ownedAmount} / {ingredient.amount}";

        countText.color = 
            ownedAmount >= ingredient.amount
            ? Color.white : Color.red;
    }
}
