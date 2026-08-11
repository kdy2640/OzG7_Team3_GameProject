using System;
using UnityEngine;

[Serializable]
public sealed class LevelMissionChecker
{
    [SerializeField] private LevelMissionGroupSO missionGroup;

    public LevelMissionGroupSO MissionGroup => missionGroup;

    public int CurrentStage
    {
        get
        {
            if (missionGroup == null || missionGroup.Missions == null)
                return 0;

            for (int i = 0; i < missionGroup.Missions.Count; i++)
            {
                LevelMissionInfo mission = missionGroup.Missions[i];

                if (mission?.Condition == null || !mission.Condition.IsSatisfied())
                    return i;
            }

            return missionGroup.Missions.Count;
        }
    }

    public void SetMissionGroup(LevelMissionGroupSO missionGroup)
    {
        this.missionGroup = missionGroup;
    }
}
