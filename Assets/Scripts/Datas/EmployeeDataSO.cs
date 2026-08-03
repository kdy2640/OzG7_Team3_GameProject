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
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private EmployeeType employeeType = EmployeeType.Count;
    [SerializeField] private WorkType workType = WorkType.Count;
    [SerializeField, Min(0)] private int cost;
    [SerializeField, Min(0f)] private float upgradeMultiplier = 1f;
    [SerializeField, Min(1)] private int maxLevel = 1;

    public string Id => id;
    public string DisplayName => displayName;
    public EmployeeType EmployeeType => employeeType;
    public WorkType WorkType => workType;
    public int Cost => cost;
    public float UpgradeMultiplier => upgradeMultiplier;
    public int MaxLevel => maxLevel;
}
