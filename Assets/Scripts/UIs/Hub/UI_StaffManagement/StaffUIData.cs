using UnityEngine;

// UI가 게임 데이터에 직접 접근하지 않도록, Controller가 만들어 전달하는 표시 전용 데이터입니다.
public enum StaffCardState { Normal, CanRecruit, CanUpgrade, Locked }

public sealed class StaffCardUIData
{
    public EmployeeType type;
    public int level;
    public StaffCardState state;
}

public sealed class StaffInfoUIData
{
    public EmployeeType type;
    public Sprite roleIcon;
    public string staffName;
    public int level;
    public int maxLevel;
    public EmployeeSkillInfo level1Skill;
    public EmployeeSkillInfo level3Skill;
    public EmployeeSkillInfo level5Skill;
    public string nextLevelText;
    public string nextLevelEffect;
    public int cost;
    public bool canAction;
    public bool isMaxLevel;
}
