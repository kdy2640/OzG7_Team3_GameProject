using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MarketSalesViewData;

public sealed class UI_MenuSalesRow : MonoBehaviour
{
    [SerializeField] private Image menuIcon;
    [SerializeField] private TMP_Text salesText;

    public void SetData(
        MenuSalesViewData data)
    {
        if (data == null) return;

        if (menuIcon != null) menuIcon.sprite = data.menuIcon;

        if (salesText != null)
        {
            salesText.text = $"{data.salesAmount:N0}ÄÚÀÎ";
        }
    }
}