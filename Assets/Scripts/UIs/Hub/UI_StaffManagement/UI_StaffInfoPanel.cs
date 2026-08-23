using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class UI_StaffInfoPanel : MonoBehaviour
{
    [SerializeField] private Image roleIcon;
    [SerializeField] private TMP_Text staffNameText;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] private Image[] levelSlots = new Image[5];
    [SerializeField] private Color filledSlotColor = new(1f, .78f, .2f);
    [SerializeField] private Color emptySlotColor = new(.3f, .3f, .3f);

    [SerializeField] private TMP_Text skill1Name;
    [SerializeField] private TMP_Text skill1Description;
    [SerializeField] private TMP_Text skill1Effect;

    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text nextLevelEffect;

    [SerializeField] private Image skill3LockIcon;
    [SerializeField] private TMP_Text skill3Name;
    [SerializeField] private TMP_Text skill3Label;
    [SerializeField] private TMP_Text skill3Description;

    [SerializeField] private Image skill5LockIcon;
    [SerializeField] private TMP_Text skill5Name;
    [SerializeField] private TMP_Text skill5Label;
    [SerializeField] private TMP_Text skill5Description;

    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text actionText;

    private Action<EmployeeType> onAction;
    private EmployeeType selectedType = EmployeeType.Count;

    private void Awake()
    {
        actionButton.onClick.AddListener(OnClickAction);
        gameObject.SetActive(false);
    }

    public void Initialize(Action<EmployeeType> callback)
    {
        onAction = callback;
    }

    public void Show(EmployeeType type)
    {
        selectedType = type;

        if (!CreateInfoData())
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }

 
    private bool CreateInfoData()
    {
        if (!EmployeeDataDB.TryGetData(selectedType, out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"EmployeeData가 없습니다 : {selectedType}");
            return false;
        }

        UpgradeManager upgrade = GameManager.Instance.Upgrade;

        int level = upgrade.RuntimeLevel.Get(selectedType);
        bool isMaxLevel = level >= employeeData.MaxLevel;

        EmployeeUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(employeeData.EmployeeType);

        int cost = 0;

        if (!isMaxLevel && upgradeData != null)
            upgradeData.TryGetRequiredCost(level + 1, out cost);

        bool canAction = upgrade.CanUpgrade(upgradeData);

        roleIcon.sprite = employeeData.RoleIcon;
        staffNameText.text = employeeData.DisplayName;
        levelText.text = $"Lv.{level}";

        for (int i = 0; i < levelSlots.Length; i++)
        {
            if (levelSlots[i] != null)
                levelSlots[i].color = i < level
                    ? filledSlotColor : emptySlotColor;
        }

        SetSkill1(level, employeeData.GetSkill(1));

        SetLockedSkill(
            level,
            3,
            employeeData.GetSkill(3),
            skill3LockIcon,
            skill3Name,
            skill3Label,
            skill3Description
        );

        SetLockedSkill(
            level,
            5,
            employeeData.GetSkill(5),
            skill5LockIcon,
            skill5Name,
            skill5Label,
            skill5Description
        );

        nextLevelText.text = isMaxLevel ? "Max Level" : $"Next Level: Lv.{level + 1}";

        nextLevelEffect.text = isMaxLevel
            ? string.Empty : employeeData.GetLevelEffect(level + 1);

        costText.text = isMaxLevel ? "-" : cost.ToString("N0");

        actionText.text = isMaxLevel
            ? "MAX" : level == 0 ? "Recruit" : $"Lv.{level + 1} Upgrade";

        actionButton.interactable = canAction;

        return true;
    }

    private void SetSkill1(int level, EmployeeSkillInfo skill)
    {
        bool unlocked = level >= 1;

        skill1Name.text = skill.Name;
        skill1Description.text = unlocked
            ? skill.Description : "Unlock after Recruit";

        skill1Effect.text = unlocked
            ? skill.Effect : string.Empty;
    }

    private void SetLockedSkill(
        int level,
        int unlockLevel,
        EmployeeSkillInfo skill,
        Image icon,
        TMP_Text name,
        TMP_Text label,
        TMP_Text description)
    {
        bool unlocked = level >= unlockLevel;

        icon.color = unlocked
            ? Color.white
            : new Color(1f, 1f, 1f, .35f);

        name.text = skill.Name;
        label.text = unlocked
            ? $"Lv.{unlockLevel} Unlocked"
            : $"Lv.{unlockLevel} Locked";

        description.text = unlocked
            ? $"{skill.Description}\n{skill.Effect}"
            : $"Unlocks at Lv.{unlockLevel}.";
    }

    private void OnClickAction()
    {
        onAction?.Invoke(selectedType);
    }
}
