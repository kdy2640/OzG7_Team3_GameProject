using System;
using UnityEngine;

public class DailySalesManagement : MonoBehaviour
{
    [Serializable]
    public class DailyCustomerData
    {
        public int visitorCount;
        public int servedCustomerCount;

        public DailyCustomerData()
        {
            visitorCount = 0;
            servedCustomerCount = 0;
        }
    }

    private MarketManager marketManager;

    // 현재 영업일
    private int currentBusinessDay = -1;

    // 오늘 누적 매출
    private int todaySales;

    // 전날 매출
    private int yesterdaySales;

    // 오늘 누적 서비스 결과
    private ServiceResultData todayServiceResult;

    // 오늘 손님 데이터
    private DailyCustomerData todayCustomerData = new DailyCustomerData();

    public int TodaySales => todaySales;

    public int YesterdaySales => yesterdaySales;

    public int TodayVisitorCount => todayCustomerData.visitorCount;

    public int TodayServedCustomerCount => todayCustomerData.servedCustomerCount;

    public event Action OnDailySalesChanged;

    private void Start()
    {
        TestSalesUI();
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;

        marketManager = GameManager.Instance.Market;

        if (marketManager == null) return;

        marketManager.SubscribeMarketDataChanged(OnMarketDataChanged);

        Initialize();
    }

    private void OnDisable()
    {
        if (marketManager != null)
        {
            marketManager.UnsubscribeMarketDataChanged(OnMarketDataChanged);
        }

        marketManager = null;
    }

    private void Initialize()
    {
        currentBusinessDay = marketManager.CurrentBusinessDay;

        todaySales = 0;
        yesterdaySales = 0;

        todayServiceResult = new ServiceResultData();

        todayCustomerData = new DailyCustomerData();

        NotifyChanged();
    }

    private void OnMarketDataChanged()
    {
        if (marketManager == null) return;

        int businessDay = marketManager.CurrentBusinessDay;

        if (businessDay != currentBusinessDay)
        {
            StartNewBusinessDay(businessDay);
            return;
        }

        NotifyChanged();
    }

    private void StartNewBusinessDay(int newBusinessDay)
    {
        // 오늘 매출을 전날 매출로 이동
        yesterdaySales = todaySales;

        // 새 영업일 초기화
        todaySales = 0;

        todayServiceResult = new ServiceResultData();

        todayCustomerData = new DailyCustomerData();

        currentBusinessDay = newBusinessDay;

        NotifyChanged();
    }

    /// <summary>
    /// 세션 완료 후 결과를 하루 데이터에 누적한다.
    /// todayTotalSales는 해당 시점의 '오늘 누적 매출'이다.
    /// </summary>
    public void ApplyServiceResult(int todayTotalSales,ServiceResultData result)
    {
        if (result == null) return;

        // 오늘 누적 매출은 외부 시스템에서 전달받는다.
        todaySales = Mathf.Max(0, todayTotalSales);

        if (todayServiceResult == null)
            todayServiceResult = new ServiceResultData();

        // 손님 수 누적
        todayServiceResult.customerReceived += Mathf.Max(0, result.customerReceived);

        todayServiceResult.customerMax += Mathf.Max(0, result.customerMax);

        // 팁 누적
        todayServiceResult.tipResult += Mathf.Max(0, result.tipResult);

        // 메뉴별 매출 누적
        if (result.menuSales != null)
        {
            foreach (MenuSalesResultData resultMenu in result.menuSales)
            {
                if (resultMenu == null) continue;

                AddOrAccumulateMenuSales(resultMenu);
            }
        }

        todayCustomerData.servedCustomerCount = todayServiceResult.customerReceived;

        NotifyChanged();
    }

    private void AddOrAccumulateMenuSales(MenuSalesResultData resultMenu)
    {
        MenuSalesResultData existingMenu =
            todayServiceResult.menuSales.Find(menu => menu != null && menu.dishType == resultMenu.dishType);

        if (existingMenu == null)
        {
            todayServiceResult.menuSales.Add(new MenuSalesResultData
                {
                    dishType = resultMenu.dishType,
                    menuName = resultMenu.menuName,
                    menuIcon = resultMenu.menuIcon,
                    salesAmount = Mathf.Max(0, resultMenu.salesAmount)
                });

            return;
        }

        existingMenu.salesAmount +=
            Mathf.Max(0, resultMenu.salesAmount);

        if (string.IsNullOrEmpty(existingMenu.menuName)) existingMenu.menuName = resultMenu.menuName;

        // 기존 아이콘이 비어 있고 새 결과에 아이콘이 있다면 보완
        if (existingMenu.menuIcon == null) existingMenu.menuIcon = resultMenu.menuIcon;
    }

    public void RecordVisitor()
    {
        todayCustomerData.visitorCount++;

        NotifyChanged();
    }

    public void RecordServedCustomer()
    {
        todayCustomerData.servedCustomerCount++;

        NotifyChanged();
    }

    public MarketSalesViewData GetData()
    {
        MarketSalesViewData data = new MarketSalesViewData
            {
                todaySales = todaySales,
                yesterdaySales = yesterdaySales,
                servedCustomerCount = 0,
                totalCustomerCount = 0,
                tipSales = 0
        };

        if (todayServiceResult == null)return data;

        data.servedCustomerCount = Mathf.Max(0,todayServiceResult.customerReceived);

        data.totalCustomerCount = Mathf.Max(0, todayServiceResult.customerMax);

        data.tipSales = Mathf.Max(0,todayServiceResult.tipResult);

        if (todayServiceResult.menuSales != null)
        {
            foreach (MenuSalesResultData menuResult
                     in todayServiceResult.menuSales)
            {
                if (menuResult == null)
                    continue;

                data.menuSales.Add(new MenuSalesViewData
                    {
                        dishType = menuResult.dishType,
                        menuName = menuResult.menuName,
                        menuIcon = menuResult.menuIcon,
                        salesAmount =Mathf.Max(0,menuResult.salesAmount)});
            }
        }

        return data;
    }

    private void NotifyChanged()
    {
        OnDailySalesChanged?.Invoke();
    }

    public void TestSalesUI()
    {
        ServiceResultData result = new ServiceResultData
        {
            customerReceived = 42,
            customerMax = 50,
            tipResult = 3500
        };

        result.menuSales.Add(new MenuSalesResultData
        {
            dishType = (DishType)0,
            menuName = "MENU A",
            menuIcon = null,
            salesAmount = 60000
        });

        result.menuSales.Add(new MenuSalesResultData
        {
            dishType = (DishType)1,
            menuName = "MENU B",
            menuIcon = null,
            salesAmount = 40000
        });

        result.menuSales.Add(new MenuSalesResultData
        {
            dishType = (DishType)2,
            menuName = "MENU C",
            menuIcon = null,
            salesAmount = 25000
        });

        ApplyServiceResult(125000, result);
    }
}