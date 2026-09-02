using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class UI_StaffInfoPanel : MonoBehaviour
{
    [SerializeField] private Image staffPortrait;
    [SerializeField] private Image roleIcon;
    [SerializeField] private TMP_Text staffNameText;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] private GameObject[] filledLevelSlots = new GameObject[5];

    [SerializeField] private TMP_Text levelEffectLabel;
    [SerializeField] private TMP_Text currentLevelEffect;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text nextLevelEffect;

    [SerializeField] private Image skill1LockIcon;
    [SerializeField] private GameObject skill1UnlockedBackground;
    [SerializeField] private TMP_Text skill1Name;
    [SerializeField] private TMP_Text skill1Label;
    [SerializeField] private TMP_Text skill1Description;

    [SerializeField] private Image skill3LockIcon;
    [SerializeField] private GameObject skill3UnlockedBackground;
    [SerializeField] private TMP_Text skill3Name;
    [SerializeField] private TMP_Text skill3Label;
    [SerializeField] private TMP_Text skill3Description;

    [SerializeField] private Image skill5LockIcon;
    [SerializeField] private GameObject skill5UnlockedBackground;
    [SerializeField] private TMP_Text skill5Name;
    [SerializeField] private TMP_Text skill5Label;
    [SerializeField] private TMP_Text skill5Description;

    [SerializeField] private TMP_Text staffDescription;
    [SerializeField] private ScrollRect skillScrollRect;

    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private GameObject actionLockIcon;

    private Action<EmployeeType> onAction;
    private EmployeeType selectedType = EmployeeType.Count;
    private Color defaultCostTextColor;

    [SerializeField] private PanelAnimator panelAnimator;

    private void Awake()
    {
        defaultCostTextColor = costText.color;
        actionButton.onClick.AddListener(OnClickAction);
        gameObject.SetActive(false);
    }

    public void Initialize(Action<EmployeeType> callback)
    {
        onAction = callback;
    }

    public IEnumerator Show(EmployeeType type)
    {
        bool wasActive = gameObject.activeSelf;
        selectedType = type;

        if (!CreateInfoData())
        {
            gameObject.SetActive(false);
            yield break;
        }

        gameObject.SetActive(true);
        ResetSkillScrollPosition();

        if (!wasActive)
            yield return panelAnimator.Show();
    }

    public IEnumerator Hide()
    {
        if (!gameObject.activeSelf)
            yield break;

        selectedType = EmployeeType.Count;
        yield return panelAnimator.Hide();
        gameObject.SetActive(false);
    }

    private void ResetSkillScrollPosition()
    {
        Canvas.ForceUpdateCanvases();
        skillScrollRect.StopMovement();
        skillScrollRect.verticalNormalizedPosition = 1f;
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

        UpgradeAvailability availability =
            upgrade.GetUpgradeAvailability(upgradeData);
        bool canAction = availability == UpgradeAvailability.Available;
        bool isInsufficientCurrency =
            availability == UpgradeAvailability.InsufficientCurrency;

        staffPortrait.sprite = employeeData.StaffPortrait;
        roleIcon.sprite = employeeData.RoleIcon;
        staffNameText.text = employeeData.DisplayName;
        levelText.text = $"Lv.{level}";

        for (int i = 0; i < filledLevelSlots.Length; i++)
        {
            filledLevelSlots[i].SetActive(i < level);
        }

        levelEffectLabel.text = employeeData.LevelEffectLabel;
        currentLevelEffect.text = level <= 1
            ? "-"
            : employeeData.GetLevelEffect(level);

        SetSkill(
            level,
            1,
            employeeData.GetSkill(1),
            skill1LockIcon,
            skill1UnlockedBackground,
            skill1Name,
            skill1Label,
            skill1Description
        );

        SetSkill(
            level,
            3,
            employeeData.GetSkill(3),
            skill3LockIcon,
            skill3UnlockedBackground,
            skill3Name,
            skill3Label,
            skill3Description
        );

        SetSkill(
            level,
            5,
            employeeData.GetSkill(5),
            skill5LockIcon,
            skill5UnlockedBackground,
            skill5Name,
            skill5Label,
            skill5Description
        );

        nextLevelText.text = isMaxLevel
            ? "최대 레벨"
            : $"다음 레벨: Lv.{level + 1}";
        nextLevelEffect.text = isMaxLevel
            ? string.Empty
            : employeeData.GetLevelEffect(level + 1);

        staffDescription.text = employeeData.Description;

        costText.text = isMaxLevel ? "-" : cost.ToString("N0");
        costText.color = isInsufficientCurrency
            ? new Color32(255, 94, 94, 255)
            : defaultCostTextColor;

        actionText.text = isMaxLevel
            ? "최대"
            : availability == UpgradeAvailability.MarketLevelLocked
                ? "레벨 부족"
                : level == 0 ? "모집" : "업그레이드";

        actionButton.interactable = canAction;
        actionLockIcon.SetActive(
            availability == UpgradeAvailability.MarketLevelLocked ||
            isInsufficientCurrency);

        return true;
    }

    private void SetSkill(
        int level,
        int unlockLevel,
        EmployeeSkillInfo skill,
        Image icon,
        GameObject unlockedBackground,
        TMP_Text name,
        TMP_Text label,
        TMP_Text description)
    {
        bool unlocked = level >= unlockLevel;

        icon.gameObject.SetActive(!unlocked);
        unlockedBackground.SetActive(unlocked);

        name.text = skill.Name;
        label.gameObject.SetActive(!unlocked);
        label.text = $"Lv.{unlockLevel} 잠김";

        description.text = $"{skill.Description}\n{skill.Effect}";
    }

    private void OnClickAction()
    {
        onAction.Invoke(selectedType);
    }
}
