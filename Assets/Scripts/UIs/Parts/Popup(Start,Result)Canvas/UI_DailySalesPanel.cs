using TMPro;
using UnityEngine;
using static MarketSalesViewData;

public class UI_DailySalesPanel : MonoBehaviour
{
    [Header("Today's Performance")]
    [SerializeField] private TMP_Text todaySalesText;
    [SerializeField] private TMP_Text customerCountText;

    [Header("Sales Difference")]
    [SerializeField] private TMP_Text salesDifferenceText;
    [SerializeField] private GameObject increaseIcon;
    [SerializeField] private GameObject decreaseIcon;

    [Header("Menu Sales")]
    [SerializeField] private Transform menuSalesContent;
    [SerializeField] private UI_MenuSalesRow menuSalesRowPrefab;

    [Header("Tip")]
    [SerializeField] private TMP_Text tipSalesText;

    private DailySalesManagement dailySalesManagement;

    private void OnEnable()
    {
        dailySalesManagement =
            FindFirstObjectByType<DailySalesManagement>();

        if (dailySalesManagement == null)
            return;

        dailySalesManagement.OnDailySalesChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (dailySalesManagement != null)
        {
            dailySalesManagement.OnDailySalesChanged -= Refresh;
        }

        dailySalesManagement = null;
    }

    private void Refresh()
    {
        if (dailySalesManagement == null)
            return;

        MarketSalesViewData data =
            dailySalesManagement.GetData();

        if (data == null)
            return;

        RefreshTodaySales(data);
        RefreshSalesDifference(data);
        RefreshMenuSales(data);
        RefreshTipSales(data);
    }

    private void RefreshTodaySales(
        MarketSalesViewData data)
    {
        if (todaySalesText != null)
        {
            todaySalesText.text =
                $"{data.todaySales:N0}코인";
        }

        if (customerCountText != null)
        {
            customerCountText.text =
                $"{data.servedCustomerCount:N0}명";
        }
    }

    private void RefreshSalesDifference(
        MarketSalesViewData data)
    {
        int difference =
            data.todaySales - data.yesterdaySales;

        if (salesDifferenceText != null)
        {
            if (difference > 0)
            {
                salesDifferenceText.text =
                    $"+{difference:N0}코인";
            }
            else
            {
                salesDifferenceText.text =
                    $"{difference:N0}코인";
            }
        }

        if (increaseIcon != null)
        {
            increaseIcon.SetActive(
                difference > 0);
        }

        if (decreaseIcon != null)
        {
            decreaseIcon.SetActive(
                difference < 0);
        }
    }

    private void RefreshMenuSales(
        MarketSalesViewData data)
    {
        if (menuSalesContent == null ||
            menuSalesRowPrefab == null)
        {
            return;
        }

        for (int i = menuSalesContent.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                menuSalesContent.GetChild(i).gameObject);
        }

        if (data.menuSales == null)
            return;

        foreach (MenuSalesViewData menuData
                 in data.menuSales)
        {
            UI_MenuSalesRow row =
                Instantiate(
                    menuSalesRowPrefab,
                    menuSalesContent);

            row.SetData(menuData);
        }
    }

    private void RefreshTipSales(
        MarketSalesViewData data)
    {
        if (tipSalesText == null)
            return;

        tipSalesText.text =
            $"{data.tipSales:N0}코인";
    }
}