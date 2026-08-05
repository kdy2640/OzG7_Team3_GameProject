using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    Stage_1,
    Stage_2,
    Stage_3,
    Count
}

[CreateAssetMenu(menuName = "Game/StageDataSO")]
public sealed class StageDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private StageType stageType = StageType.Count;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private List<GroceryType> rewardList = new();
    [SerializeField] private List<HarvestType> harvestList = new();
    [SerializeField] private Sprite icon;

    public string Id => id;
    public StageType StageType => stageType;
    public string DisplayName => displayName;
    public string Description => description;
    public List<GroceryType> RewardList => rewardList;
    public List<HarvestType> HarvestList => harvestList;
    public Sprite Icon => icon;
}
