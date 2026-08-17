using UnityEngine;
using UnityEngine.UI;

public class UI_SalesStartPanel : MonoBehaviour
{
    [Header("Level Slots")]
    [SerializeField] private Image[] levelSlots;

    [Header("Slot Colors")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;

    private const int MinLevel = 1; 
    private const int MaxLevel = 4;

    private MarketData marketData;

    public void Initialize(MarketData data)
    {
        if (data == null)
        {
            Debug.LogError("[UI_MarketLevelDisplay] MarketData가 없습니다.");
            return;
        }

        if (marketData != null)
        {
            marketData.OnMarketDataChanged -= HandleMarketDataChanged;
        }

        marketData = data;
        marketData.OnMarketDataChanged += HandleMarketDataChanged;

        RefreshLevelSlots();
    }

    private void HandleMarketDataChanged()
    {
        RefreshLevelSlots();
    }

    private void RefreshLevelSlots()
    {
        if (marketData == null) return;

        int currentLevel = Mathf.Clamp
            (marketData.CurrentLevel, MinLevel, MaxLevel);

        for (int i = 0; i < levelSlots.Length; i++)
        {
            if (levelSlots[i] == null) continue;

            int slotLevel = i + 1;

            levelSlots[i].color =
                slotLevel <= currentLevel ? activeColor : inactiveColor;
        }
    }

    private void OnDestroy()
    {
        if (marketData != null)
        {
            marketData.OnMarketDataChanged -= HandleMarketDataChanged;
        }
    }
}
