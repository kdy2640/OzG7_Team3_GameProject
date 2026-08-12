using System;
using UnityEngine;

public class FacilityController : MonoBehaviour
{
    [Header("Facility Info")]
    [SerializeField] private string facilityName;
    [SerializeField] private int currentLevel;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] private bool isPurchased;
    [SerializeField] private string[] levelEffects;

    [Header("Views")]
    [SerializeField] private FacilityModelView modelView;
    [SerializeField] private FacilityWorldUI worldUI;

    public string FacilityName => facilityName;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;
    public bool IsPurchased => isPurchased;

    // 상세 패널이 열려 있는 동안 외부 데이터가 변경되었을 때 갱신용입니다.
    public event Action<FacilityController> StateChanged;

    private void Awake()
    {
        maxLevel = Mathf.Max(1, maxLevel);
        RefreshViews();
    }
    private void OnEnable()
    {
        RefreshViews();
    }

    // 외부 데이터 담당자가 호출하는 상태 반영 API입니다.
    public void SetState(bool purchased, int level)
    {
        isPurchased = purchased;
        currentLevel = purchased
            ? Mathf.Clamp(level, 1, maxLevel) : 0;

        RefreshViews();
        StateChanged?.Invoke(this);
    }

    // 현재는 레이아웃/테스트용 로컬 구매 처리입니다.
    public bool TryPurchase()
    {
        if (isPurchased) return false;

        isPurchased = true;
        currentLevel = 1;

        RefreshViews();
        modelView?.PlayUpgradeEffect();
        StateChanged?.Invoke(this);

        return true;
    }

    // 현재는 레이아웃/테스트용 로컬 강화 처리입니다.
    public bool TryUpgrade()
    {
        if (!CanUpgrade()) return false;

        currentLevel++;

        RefreshViews();
        modelView?.PlayUpgradeEffect();
        StateChanged?.Invoke(this);

        return true;
    }

    public bool CanUpgrade()
    {
        return isPurchased && currentLevel < maxLevel;
    }

    public string GetCurrentEffect()
    {
        if (!isPurchased) return GetEffect(0);

        return GetEffect(currentLevel - 1);
    }

    public string GetNextEffect()
    {
        if (!isPurchased) return GetEffect(0);

        if (currentLevel >= maxLevel) return "Max Level";

        return GetEffect(currentLevel);
    }

    private string GetEffect(int index)
    {
        if (levelEffects == null || index < 0 || index >= levelEffects.Length)
            return string.Empty;

        return levelEffects[index];
    }

    private void RefreshViews()
    {
        if (!isPurchased) modelView?.ShowLocked();
        
        else modelView?.ShowLevel(currentLevel);

        worldUI?.Refresh(isPurchased, currentLevel, CanUpgrade());
    }
}