using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MarketSalesViewData;

public sealed class UI_MenuSalesRow : MonoBehaviour
{
    [SerializeField] private TMP_Text menuNameText;
    [SerializeField] private Image menuIcon;
    [SerializeField] private TMP_Text salesText;

    public void SetData(
        MenuSalesViewData data)
    {
        if (data == null) return;

        if (menuNameText != null) menuNameText.text = data.menuName;

        if (menuIcon != null) menuIcon.sprite = data.menuIcon;

        if (salesText != null)
        {
            salesText.text = $"{data.salesAmount:N0}코인";
        }
    }
    public void Clear()
    {
        if (menuNameText != null) menuNameText.text = string.Empty;

        if (menuIcon != null) menuIcon.sprite = null;

        if (salesText != null) salesText.text = "0코인";
    }
}