using TMPro;
using UnityEngine;

public class UI_DailySalesPanel : MonoBehaviour
{
    [Header("Daily Sales")]
    [SerializeField] private TMP_Text todaySalesText;

    [Header("Customer")]
    [SerializeField] private TMP_Text customerCountText;

    [Header("Graph")]
    [SerializeField] private DailySalesGraphManager graphManager;

    [Header("Data")]
    [SerializeField] private DailySalesManagement dailySalesManagement;

    private void OnEnable()
    {
        if (dailySalesManagement == null) return;

        dailySalesManagement.OnDailySalesChanged += Refresh;

        Refresh();
    }
    private void OnDisable()
    {
        if (dailySalesManagement == null) return;

        dailySalesManagement.OnDailySalesChanged -= Refresh;
    }
    private void Refresh()
    {
        if (dailySalesManagement == null)
            return;

        // 오늘 매출
        if (todaySalesText != null)
        {
            todaySalesText.text = $"G{dailySalesManagement.TodaySales:N0}";
        }

        // 응대한 손님 / 방문한 손님
        if (customerCountText != null)
        {
            customerCountText.text =
                $"{dailySalesManagement.TodayServedCustomerCount} / " +
                $"{dailySalesManagement.TodayVisitorCount}";
        }

        // 그래프
        if (graphManager != null)
        {
            graphManager.UpdateGraph( dailySalesManagement.GetGraphData());
        }
    }
}
