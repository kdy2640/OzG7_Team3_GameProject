using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// StaffInfoPanel에 붙입니다. 표시와 버튼 클릭 전달만 담당합니다.
public sealed class UI_StaffInfoPanel : MonoBehaviour
{
    [SerializeField] private Image roleIcon;
    [SerializeField] private TMP_Text staffNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image[] levelSlots = new Image[5];
    [SerializeField] private Color filledSlotColor = new Color(1, .78f, .2f);
    [SerializeField] private Color emptySlotColor = new Color(.3f, .3f, .3f);
    [SerializeField] private TMP_Text skill1Name, skill1Description, skill1Effect;
    [SerializeField] private TMP_Text nextLevelText, nextLevelEffect;
    [SerializeField] private Image skill3LockIcon, skill5LockIcon;
    [SerializeField] private TMP_Text skill3Name, skill3Label, skill3Description;
    [SerializeField] private TMP_Text skill5Name, skill5Label, skill5Description;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text costText, actionText;

    private Action<EmployeeType> onAction;
    private EmployeeType selectedType = EmployeeType.Count;

    private void Awake()
    {
        actionButton.onClick.AddListener(() => onAction?.Invoke(selectedType));
        gameObject.SetActive(false);
    }

    public void Initialize(Action<EmployeeType> callback) => onAction = callback;

    public void Show(StaffInfoUIData data)
    {
        selectedType = data.type;
        gameObject.SetActive(true);
        roleIcon.sprite = data.roleIcon;
        staffNameText.text = data.staffName;
        levelText.text = $"Lv.{data.level}";
        for (int i = 0; i < levelSlots.Length; i++)
            if (levelSlots[i] != null) levelSlots[i].color = i < data.level ? filledSlotColor : emptySlotColor;

        SetSkill1(data);
        SetLockedSkill(data.level, 3, data.level3Skill, skill3LockIcon, skill3Name, skill3Label, skill3Description);
        SetLockedSkill(data.level, 5, data.level5Skill, skill5LockIcon, skill5Name, skill5Label, skill5Description);
        nextLevelText.text = data.nextLevelText;
        nextLevelEffect.text = data.nextLevelEffect;
        costText.text = data.isMaxLevel ? "-" : data.cost.ToString("N0");
        actionText.text = data.isMaxLevel ? "MAX" : data.level == 0 ? "Recruit" : $"Lv.{data.level + 1} Upgrade";
        actionButton.interactable = data.canAction;
    }

    private void SetSkill1(StaffInfoUIData data)
    {
        bool unlocked = data.level >= 1;
        skill1Name.text = data.level1Skill.Name;
        skill1Description.text = unlocked ? data.level1Skill.Description : "직원을 모집하면 해금됩니다.";
        skill1Effect.text = unlocked ? data.level1Skill.Effect : "";
    }

    private void SetLockedSkill(int level, int unlockLevel, EmployeeSkillInfo skill, Image icon, TMP_Text name, TMP_Text label, TMP_Text description)
    {
        bool unlocked = level >= unlockLevel;
        icon.color = unlocked ? Color.white : new Color(1, 1, 1, .35f);
        name.text = skill.Name;
        label.text = unlocked ? $"Lv.{unlockLevel} 해금" : $"Lv.{unlockLevel} 잠김";
        description.text = unlocked ? $"{skill.Description}\n{skill.Effect}" : $"Lv.{unlockLevel} 달성 시 해금됩니다.";
    }
}
