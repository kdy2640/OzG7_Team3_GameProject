using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_IngredientCard : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    public void SetData(IngredientCardData data)
    {
        if (data == null) return;

        icon.sprite = data.Icon;

        countText.text = $"{data.RequiredAmount} / {data.OwnedAmount}";

        countText.color = 
            data.OwnedAmount >= data.RequiredAmount 
            ? Color.white : Color.red;
    }
}