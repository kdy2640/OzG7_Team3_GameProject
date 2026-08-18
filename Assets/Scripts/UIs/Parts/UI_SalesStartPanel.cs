using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SalesStartPanel : MonoBehaviour
{
    [Header("Level Slots")]
    [SerializeField] private Image[] levelSlots;

    [Header("Slot Colors")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;

    [Header("Mission UI")]
    [SerializeField] private TMP_Text[] missionTitleTexts;
    [SerializeField] private TMP_Text[] missionDescriptionTexts;

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

        Refresh();
    }

    private void HandleMarketDataChanged()
    {
        Refresh();
    }
    private void Refresh()
    {
        if(marketData == null) return;

        int currentLevel = Mathf.Clamp(marketData.CurrentLevel, MinLevel, MaxLevel);

        RefreshLevelSlots(currentLevel);
        RefreshMissionTexts(currentLevel);
    }

    private void RefreshLevelSlots(int currentLevel)
    {
        if (marketData == null) return;

        for (int i = 0; i < levelSlots.Length; i++)
        {
            if (levelSlots[i] == null) continue;

            int slotLevel = i + 1;

            levelSlots[i].color =
                slotLevel <= currentLevel ? activeColor : inactiveColor;
        }
    }
    private void RefreshMissionTexts(int currentLevel)
    {
        LevelMissionGroupSO missionGroupSO = LevelMissionGroupDB.GetData(currentLevel);

        if (missionGroupSO == null)
        {
            ClearMissionTexts();
            return;
        }
        for (int i = 0; i < missionTitleTexts.Length; i++)
        {
            if (missionDescriptionTexts[i] == null) continue;

            if (i < missionGroupSO.Missions.Count)
            {
                LevelMissionInfo mission = missionGroupSO.Missions[i];

                missionTitleTexts[i].text = 
                    mission != null ? mission.Title : string.Empty;
            }
            else
            {
                missionTitleTexts[i].text = string.Empty;
            }
        }
        for (int i = 0; i < missionDescriptionTexts.Length; i++) 
        { 
            if (missionDescriptionTexts[i] == null) continue; 

            if (i < missionGroupSO.Missions.Count) 
            { LevelMissionInfo mission = missionGroupSO.Missions[i]; 

                missionDescriptionTexts[i].text = 
                    mission != null ? mission.Description : string.Empty; 
            } 

            else 
            { 
                missionDescriptionTexts[i].text = string.Empty; 
            } 
        }
    }
    private void ClearMissionTexts()
    {
        for(int i = 0; i < missionTitleTexts.Length; i++)
        {
            if (missionTitleTexts[i] != null)
                missionTitleTexts[i].text = string.Empty;
        }
        for(int i = 0; i < missionDescriptionTexts.Length; i++)
        {
            if (missionDescriptionTexts[i] != null)
                missionDescriptionTexts[i].text = string.Empty;
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
