using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 업그레이드 노드가 공통으로 사용하는 데이터입니다.
/// </summary>
public abstract class UpgradeDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite displayIcon;

    [SerializeField] private int baseCost;
    [SerializeField] private float costMultiplier = 1.2f;
    [SerializeField] private int maxLevel = 1;
    [SerializeField] private List<int> requiredMarketLevel = new() { 0, 0, 0, 0, 0 };

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite DisplayIcon => displayIcon;
    public int BaseCost => baseCost;
    public float CostMultiplier => costMultiplier;
    public int MaxLevel => maxLevel;
    public IReadOnlyList<int> RequiredMarketLevel => requiredMarketLevel;

    public bool TryGetRequiredMarketLevel(int targetUpgradeLevel, out int requiredLevel)
    {
        int index = targetUpgradeLevel - 1;

        if (index < 0 || index >= requiredMarketLevel.Count)
        {
            requiredLevel = 0;
            return false;
        }

        requiredLevel = Mathf.Max(0, requiredMarketLevel[index]);
        return true;
    }

    /// <summary>
    /// 다음 레벨에 필요한 재화량을 계산합니다.
    /// </summary>
    public int GetCosts(int level)
    {
        return Mathf.RoundToInt(
            baseCost * Mathf.Pow(costMultiplier, level));
    }
}
