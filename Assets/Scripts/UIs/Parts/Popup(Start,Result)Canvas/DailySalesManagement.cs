using System;
using System.Collections.Generic;
using UnityEngine;

//기존 MarketManager를 읽기만 하고 값을 변경하지 않음
public class DailySalesManagement : MonoBehaviour
{
    public const int MaxHistoryCount = 5;

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

    // 영업일 시작 시점의 누적 매출
    private int dayStartTotalIncome;

    // 오늘 손님 데이터
    private DailyCustomerData todayCustomerData = new DailyCustomerData();

    // 최근 5일 매출 기록
    private readonly List<DailySalesData> salesHistory =
        new List<DailySalesData>();

    public int TodayVisitorCount => todayCustomerData.visitorCount;

    public int TodayServedCustomerCount => todayCustomerData.servedCustomerCount;

    public int TodaySales
    {
        get
        {
            if (marketManager == null) return 0;

            return Mathf.Max
                (0, marketManager.MarketData.TotalIncome - dayStartTotalIncome);
        }
    }

    public IReadOnlyList<DailySalesData> SalesHistory => salesHistory;

    public event Action OnDailySalesChanged;

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
        MarketData marketData = marketManager.MarketData;

        currentBusinessDay = marketData.CurrentBusinessDay;

        dayStartTotalIncome = marketData.TotalIncome;

        todayCustomerData = new DailyCustomerData();

        NotifyChanged();
    }

    private void OnMarketDataChanged()
    {
        if (marketManager == null) return;

        MarketData marketData = marketManager.MarketData;

        CheckBusinessDayChanged(marketData);

        NotifyChanged();
    }

    private void CheckBusinessDayChanged(MarketData marketData)
    {
        if (marketData.CurrentBusinessDay == currentBusinessDay)
        {
            return;
        }

        // 이전 영업일 기록
        if (currentBusinessDay >= 0)
        {
            SavePreviousDay(marketData);
        }

        // 새로운 영업일 시작
        currentBusinessDay = marketData.CurrentBusinessDay;

        dayStartTotalIncome = marketData.TotalIncome;

        todayCustomerData = new DailyCustomerData();
    }

    private void SavePreviousDay(MarketData marketData)
    {
        int previousDaySales = Mathf.Max
            (0, marketData.TotalIncome - dayStartTotalIncome);

        salesHistory.Add
            (new DailySalesData
            {
                dateLabel = $"{currentBusinessDay}일차",

                salesAmount = previousDaySales
            });

        while (salesHistory.Count > MaxHistoryCount)
        {
            salesHistory.RemoveAt(0);
        }
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

    public List<DailySalesData> GetGraphData()
    {
        List<DailySalesData> result = new List<DailySalesData>(salesHistory);

        // 오늘 데이터 추가
        result.Add(new DailySalesData
            {
                dateLabel = $"{currentBusinessDay}일차",

                salesAmount = TodaySales
            });

        while (result.Count > MaxHistoryCount)
        {
            result.RemoveAt(0);
        }

        return result;
    }

    private void NotifyChanged()
    {
        OnDailySalesChanged?.Invoke();
    }
}