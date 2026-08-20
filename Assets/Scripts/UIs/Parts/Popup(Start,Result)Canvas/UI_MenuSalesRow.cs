using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_MenuSalesRow : MonoBehaviour
{
    [SerializeField] private TMP_Text menuNameText;
    [SerializeField] private Image menuIcon;
    [SerializeField] private TMP_Text salesText;

    public void SetData(SalesResultData.MenuSalesData data)
    {
        if (data == null) return;

        if (!DishDataDB.TryGetData(data.dishType, out DishDataSO dishData))
        {
            Clear();
            return;
        }

        if (menuNameText != null) menuNameText.text = dishData.DisplayName;

        if (menuIcon != null) menuIcon.sprite = dishData.Icon;

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
