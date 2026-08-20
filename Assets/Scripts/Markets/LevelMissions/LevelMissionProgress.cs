using System;
using UnityEngine;

[Serializable]
public sealed class LevelMissionProgress
{
    [SerializeField] private LevelMissionGroupSO missionGroup;
    [SerializeField, Min(0)] private int claimedMissionCount;

    public LevelMissionGroupSO MissionGroup => missionGroup;
    public int ClaimedMissionCount => claimedMissionCount;
    public int CurrentStage => claimedMissionCount;

    public LevelMissionInfo CurrentMission
    {
        get
        {
            if (missionGroup == null || missionGroup.Missions == null)
                return null;

            return claimedMissionCount < missionGroup.Missions.Count
                ? missionGroup.Missions[claimedMissionCount]
                : null;
        }
    }

    public bool IsCurrentMissionSatisfied =>
        CurrentMission?.Condition != null
        && CurrentMission.Condition.IsSatisfied();

    public bool CanClaimCurrentReward =>
        IsCurrentMissionSatisfied
        && CurrentMission?.Reward != null;

    public bool AreAllMissionsClaimed =>
        missionGroup != null
        && missionGroup.Missions != null
        && missionGroup.Missions.Count > 0
        && claimedMissionCount >= missionGroup.Missions.Count;

    public bool TryClaimCurrentReward()
    {
        LevelMissionInfo currentMission = CurrentMission;

        if (currentMission?.Condition == null
            || !currentMission.Condition.IsSatisfied()
            || currentMission.Reward == null
            || !currentMission.Reward.TryGrant())
        {
            return false;
        }

        claimedMissionCount = Mathf.Min(
            claimedMissionCount + 1,
            missionGroup.Missions.Count);
        return true;
    }

    public void SetMissionGroup(LevelMissionGroupSO missionGroup)
    {
        this.missionGroup = missionGroup;
        ClampClaimedMissionCount();
    }

    public void LoadClaimedMissionCount(int count)
    {
        claimedMissionCount = Mathf.Max(0, count);
        ClampClaimedMissionCount();
    }

    private void ClampClaimedMissionCount()
    {
        int missionCount = missionGroup?.Missions?.Count ?? 0;
        claimedMissionCount = Mathf.Clamp(claimedMissionCount, 0, missionCount);
    }
}
