using TMPro;
using UnityEngine;

public class UI_DailySalesPanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DailySalesManagement dailySalesManagement;

    [Header("Today's Performance")]
    [SerializeField] private TMP_Text todaySalesText;
    [SerializeField] private TMP_Text customerCountText;

    [Header("Sales Difference")]
    [SerializeField] private TMP_Text salesDifferenceText;
    [SerializeField] private GameObject increaseIcon;
    [SerializeField] private GameObject decreaseIcon;

    [Header("Menu Sales")]
    [SerializeField] private UI_MenuSalesRow[] menuSalesRows = new UI_MenuSalesRow[3];

    [Header("Tip")]
    [SerializeField] private TMP_Text tipSalesText;

    private bool isSubscribed;

    private void Start()
    {
        Subscribe();
        Refresh();
    }

    private void OnEnable()
    {
        // Start 이전에 활성화될 경우를 대비해서
        if (Application.isPlaying) Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (isSubscribed) return;

        if (dailySalesManagement == null)
        {
            Debug.LogError("UI_DailySalesPanel: DailySalesManagement 참조가 없습니다.", this);
            return;
        }

        dailySalesManagement.OnDailySalesChanged += Refresh;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed) return;

        if (dailySalesManagement != null) dailySalesManagement.OnDailySalesChanged -= Refresh;

        isSubscribed = false;
    }

    private void Refresh()
    {
        if (dailySalesManagement == null) return;

        MarketSalesViewData data =
            dailySalesManagement.GetData();

        if (data == null) return;

        if (todaySalesText != null)
            todaySalesText.text = $"{data.todaySales:N0}코인";

        if (customerCountText != null)
            customerCountText.text = 
                $"{data.servedCustomerCount:N0} / {data.totalCustomerCount:N0}명";

        int difference = data.todaySales - data.yesterdaySales;

        if (salesDifferenceText != null)
        {
            salesDifferenceText.text =
                difference > 0 ? $"+{difference:N0}코인" : $"{difference:N0}코인";
        }

        if (increaseIcon != null) increaseIcon.SetActive(difference > 0);

        if (decreaseIcon != null) decreaseIcon.SetActive(difference < 0);

        RefreshMenuSales(data);

        if (tipSalesText != null)
            tipSalesText.text =
                $"{data.tipSales:N0}코인";
    }

    private void RefreshMenuSales(MarketSalesViewData data)
    {
        for (int i = 0; i < menuSalesRows.Length; i++)
        {
            UI_MenuSalesRow row = menuSalesRows[i];

            if (row == null)
                continue;

            if (data.menuSales != null &&
                i < data.menuSales.Count)
            {
                row.SetData(data.menuSales[i]);
            }
            else
            {
                row.Clear();
            }
        }
    }
}