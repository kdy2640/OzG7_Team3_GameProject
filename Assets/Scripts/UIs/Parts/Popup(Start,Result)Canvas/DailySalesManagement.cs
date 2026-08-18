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

    // 현재 영업일 시작 시점의 누적 매출
    private int dayStartTotalIncome;

    // 직전 영업일 매출
    private int yesterdaySales;

    // 오늘 손님 데이터
    private DailyCustomerData todayCustomerData =
        new DailyCustomerData();

    public int TodayVisitorCount =>
        todayCustomerData.visitorCount;

    public int TodayServedCustomerCount =>
        todayCustomerData.servedCustomerCount;

    public int TodaySales
    {
        get
        {
            if (marketManager == null)
                return 0;

            return Mathf.Max(
                0,
                marketManager.MarketData.TotalIncome
                    - dayStartTotalIncome);
        }
    }

    public int YesterdaySales => yesterdaySales;

    public event Action OnDailySalesChanged;

    private void OnEnable()
    {
        if (GameManager.Instance == null)
            return;

        marketManager = GameManager.Instance.Market;

        if (marketManager == null)
            return;

        marketManager.SubscribeMarketDataChanged(
            OnMarketDataChanged);

        Initialize();
    }

    private void OnDisable()
    {
        if (marketManager != null)
        {
            marketManager.UnsubscribeMarketDataChanged(
                OnMarketDataChanged);
        }

        marketManager = null;
    }

    private void Initialize()
    {
        MarketData marketData = marketManager.MarketData;

        currentBusinessDay =
            marketData.CurrentBusinessDay;

        dayStartTotalIncome =
            marketData.TotalIncome;

        yesterdaySales = 0;

        todayCustomerData =
            new DailyCustomerData();

        NotifyChanged();
    }

    private void OnMarketDataChanged()
    {
        if (marketManager == null)
            return;

        MarketData marketData =
            marketManager.MarketData;

        CheckBusinessDayChanged(marketData);

        NotifyChanged();
    }

    private void CheckBusinessDayChanged(
        MarketData marketData)
    {
        if (marketData.CurrentBusinessDay ==
            currentBusinessDay)
        {
            return;
        }

        if (currentBusinessDay >= 0)
        {
            SavePreviousDay(marketData);
        }

        currentBusinessDay =
            marketData.CurrentBusinessDay;

        dayStartTotalIncome =
            marketData.TotalIncome;

        todayCustomerData =
            new DailyCustomerData();
    }

    private void SavePreviousDay(
        MarketData marketData)
    {
        yesterdaySales = Mathf.Max(
            0,
            marketData.TotalIncome
                - dayStartTotalIncome);
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
        MarketSalesViewData data =
            new MarketSalesViewData();

        data.todaySales = TodaySales;
        data.yesterdaySales = YesterdaySales;
        data.servedCustomerCount =
            TodayServedCustomerCount;

        // 메뉴 / 팁 데이터는 여기에서
        // SaleResultData를 읽어 조합한다.
        //
        // 현재 SaleResultData의 실제 구조가
        // 아직 제공되지 않았으므로 이 부분은
        // 실제 필드명에 맞춰 연결한다.

        data.tipSales = 0;

        return data;
    }

    private void NotifyChanged()
    {
        OnDailySalesChanged?.Invoke();
    }
}