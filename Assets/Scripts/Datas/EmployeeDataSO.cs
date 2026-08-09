using System;
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

    [Header("Staff Info Panel")]
    [SerializeField] private Sprite roleIcon;
    [SerializeField] private EmployeeSkillInfo level1Skill;
    [SerializeField] private EmployeeSkillInfo level3Skill;
    [SerializeField] private EmployeeSkillInfo level5Skill;
    [SerializeField, TextArea] private string level2Effect;
    [SerializeField, TextArea] private string level3Effect;
    [SerializeField, TextArea] private string level4Effect;
    [SerializeField, TextArea] private string level5Effect;

    public Sprite RoleIcon => roleIcon;

    public EmployeeSkillInfo GetSkill(int unlockLevel)
    {
        return unlockLevel == 1 ? level1Skill
             : unlockLevel == 3 ? level3Skill
             : unlockLevel == 5 ? level5Skill
             : default;
    }

    public string GetLevelEffect(int level)
    {
        return level == 2 ? level2Effect
             : level == 3 ? level3Effect
             : level == 4 ? level4Effect
             : level == 5 ? level5Effect
             : string.Empty;
    }
}

[Serializable]
public struct EmployeeSkillInfo
{
    [SerializeField] private string skillName;
    [SerializeField, TextArea] private string description;
    [SerializeField, TextArea] private string effect;

    public string Name => skillName;
    public string Description => description;
    public string Effect => effect;
}






