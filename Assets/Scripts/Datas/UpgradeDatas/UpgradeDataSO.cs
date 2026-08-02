using UnityEngine;

/// <summary>
/// 업그레이드 노드가 공통으로 사용하는 데이터입니다.
/// </summary>
public abstract class UpgradeDataSO : ScriptableObject
{
    public string id;
    public string displayName;
    [field: SerializeField] public Sprite displayIcon;

    public int baseCost;
    public float costMultiplier = 1.2f;
    public int maxLevel = 1;

    public abstract void ApplyTo(RuntimeStat runtimeStat, int level);

    /// <summary>
    /// 다음 레벨에 필요한 재화량을 계산합니다.
    /// </summary>
    public int GetCosts(int level)
    {
        return Mathf.RoundToInt(
            baseCost * Mathf.Pow(costMultiplier, level));
    }
}
