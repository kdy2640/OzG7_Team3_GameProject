using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public List<UpgradeSaveData> upgrades = new();
    public List<TutorialSaveData> tutorials = new();
    public AudioSaveData audio = new();
    public StockSaveData stock = new();
    public MarketSaveData market = new();
}

[Serializable]
public class UpgradeSaveData
{
    public string id;
    public int level;

    public UpgradeSaveData(string id, int level)
    {
        this.id = id;
        this.level = level;
    }
}

[Serializable]
public class TutorialSaveData
{
    public string id;
    public bool flag;

    public TutorialSaveData(string id, bool flag)
    {
        this.id = id;
        this.flag = flag;
    }
}

[Serializable]
public class AudioSaveData
{
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public AudioSaveData()
    {
    }

    public AudioSaveData(float masterVolume, float bgmVolume, float sfxVolume)
    {
        this.masterVolume = masterVolume;
        this.bgmVolume = bgmVolume;
        this.sfxVolume = sfxVolume;
    }
}

[Serializable]
public class StockSaveData
{
    public int currency;
    public List<GroceryAmount> groceries = new();
    public List<DishAmount> dishes = new();
}

[Serializable]
public class MarketSaveData : ISerializationCallbackReceiver
{
    public int currentBusinessDay;
    public MarketPhase currentPhase;
    public int currentLevel;
    public int totalIncome;
    public int claimedMissionCount;
    public int festivalStateVersion;
    public TasteType latestFestivalTaste;
    public int tasteFestivalStartBusinessDay;
    public CategoryType latestFestivalCategory;
    public int categoryFestivalStartBusinessDay;
    public List<DishType> selectedDishes = new();

    [SerializeField, HideInInspector] private int currentEXP;

    public MarketSaveData()
    {
    }

    public MarketSaveData(int currentBusinessDay)
    {
        this.currentBusinessDay = currentBusinessDay;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (totalIncome == 0 && currentEXP > 0)
            totalIncome = currentEXP;
    }
}
