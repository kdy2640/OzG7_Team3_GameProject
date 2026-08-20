using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Mission Group")]
public sealed class LevelMissionGroupSO : ScriptableObject
{
    [SerializeField, Min(0)] private int level;
    [SerializeField] private List<LevelMissionInfo> missions = new();

    public int Level => level;
    public IReadOnlyList<LevelMissionInfo> Missions => missions;

    private void OnValidate()
    {
        if (missions == null)
            return;

        for (int i = 0; i < missions.Count; i++)
        {
            missions[i]?.SyncCondition();
            missions[i]?.SyncReward();
        }
    }
}
