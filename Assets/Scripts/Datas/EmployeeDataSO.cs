using UnityEngine;

public enum EmployeeType
{
    Server_1,
    Server_2,
    Server_3,
    Server_4,
    Cooker_1,
    Cooker_2,
    Cooker_3,
    Cooker_4,
    Harvester_1,
    Harvester_2,
    Harvester_3,
    Harvester_4,
    Count
}
public enum WorkType
{
    Server,
    Cooker,
    Harvester,
    Count
}

[CreateAssetMenu(menuName = "Game/EmployeeDataSO")]
public sealed class EmployeeDataSO : ScriptableObject
{
    public string id;
    public string displayName;
    public EmployeeType employeeType = EmployeeType.Count;
    public WorkType workType = WorkType.Count;
    [Min(0)] public int cost;
    [Min(0f)] public float upgradeMultiplier = 1f;
    [Min(1)] public int maxLevel = 1;
}
