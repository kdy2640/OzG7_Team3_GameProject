using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct DailySalesData
{
    public string dateLabel;  // 예: "1일차", "8/18"
    public float salesAmount; // 매출액
}

[System.Serializable]
public class BarSlot
{
    public GameObject slotObject;          // DayBar_N 전체 오브젝트
    public RectTransform barFillRect;      // BarFill의 RectTransform
    public TextMeshProUGUI dateText;       // DateText
    public TextMeshProUGUI amountText;     // AmountText

    public Vector3 SetData(string dateStr, float currentAmount, float maxAmount, float maxBarHeight)
    {
        if (slotObject != null) slotObject.SetActive(true);

        if (dateText != null) dateText.text = dateStr;
        if (amountText != null) amountText.text = currentAmount > 0 ? $"G{currentAmount:N0}" : "0";

        float ratio = maxAmount > 0 ? Mathf.Clamp01(currentAmount / maxAmount) : 0f;
        float targetHeight = ratio * maxBarHeight;

        if (barFillRect != null)
        {
            barFillRect.sizeDelta = new Vector2(barFillRect.sizeDelta.x, targetHeight);
        }

        Vector3 localTop = new Vector3(0, targetHeight, 0);
        return barFillRect.TransformPoint(localTop);
    }

    public void SetEmpty()
    {
        if (slotObject != null)
            slotObject.SetActive(true);

        if (dateText != null)
            dateText.text = "";

        if (amountText != null)
            amountText.text = "";

        if (barFillRect != null)
            barFillRect.sizeDelta = new Vector2(barFillRect.sizeDelta.x, 0);
    }
}

public class DailySalesGraphManager : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private UILineDrawer lineDrawer;

    [Header("Graph References")]
    [SerializeField] private float maxBarHeight = 240f;
    [SerializeField] private List<BarSlot> barSlots = new List<BarSlot>();

    public void UpdateGraph(List<DailySalesData> recentSalesList)
    {
        if (recentSalesList == null) recentSalesList = new List<DailySalesData>();

        int dataCount = Mathf.Min(recentSalesList.Count, 5);

        float maxSales = 0f;
        if (dataCount > 0)
        {
            maxSales = recentSalesList.Take(dataCount).Max(x => x.salesAmount);
        }

        List<Vector2> lineLocalPoints = new List<Vector2>();

        for (int i = 0; i < barSlots.Count; i++)
        {
            if (i < dataCount)
            {
                var data = recentSalesList[i];
                Vector3 worldTopPos = barSlots[i].SetData(data.dateLabel, data.salesAmount, maxSales, maxBarHeight);

                if (lineDrawer != null)
                {
                    Vector2 localPos = lineDrawer.transform.InverseTransformPoint(worldTopPos);
                    lineLocalPoints.Add(localPos);
                }
            }
            else
            {
                barSlots[i].SetEmpty();
            }
        }

        if (lineDrawer != null)
        {
            lineDrawer.SetPoints(lineLocalPoints);
        }
    }
}