using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// StaffInfoPanel 프리팹 루트에 붙입니다.
/// UI_StaffDevelopCard가 ShowStaff()를 호출하면 선택한 직원의 정보를 표시합니다.
/// </summary>
public sealed class UI_StaffInfoPanel : MonoBehaviour
{
    [Header("상단 직원 정보")]
    [SerializeField] private Image roleIcon;
    [SerializeField] private TMP_Text staffNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image[] levelSlotImages = new Image[5];
    [SerializeField] private Color filledLevelSlotColor = new Color(1f, 0.78f, 0.2f);
    [SerializeField] private Color emptyLevelSlotColor = new Color(0.3f, 0.3f, 0.3f);

    [Header("1레벨 스킬 영역")]
    [SerializeField] private TMP_Text firstSkillNameText;
    [SerializeField] private TMP_Text firstSkillDescriptionText;
    [SerializeField] private TMP_Text firstSkillEffectText;
    [SerializeField] private TMP_Text nextLevelDescriptionText;
    [SerializeField] private TMP_Text nextLevelEffectText;

    [Header("스크롤 뷰 - 3레벨 스킬")]
    [SerializeField] private Image level3UnlockIcon;
    [SerializeField] private TMP_Text level3SkillNameText;
    [SerializeField] private TMP_Text level3LabelText;
    [SerializeField] private TMP_Text level3DescriptionText;

    [Header("스크롤 뷰 - 5레벨 스킬")]
    [SerializeField] private Image level5UnlockIcon;
    [SerializeField] private TMP_Text level5SkillNameText;
    [SerializeField] private TMP_Text level5LabelText;
    [SerializeField] private TMP_Text level5DescriptionText;

    [Header("모집 / 강화 버튼")]
    [SerializeField] private Button recruitUpgradeButton;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private Color availableButtonColor = new Color(0.48f, 0.83f, 0.56f);
    [SerializeField] private Color unavailableButtonColor = Color.gray;

    private EmployeeType selectedEmployeeType = EmployeeType.Count;
    private EmployeeDataSO selectedEmployeeData;
    private EmployeeUpgradeDataSO selectedUpgradeData;

    private void Awake()
    {
        recruitUpgradeButton.onClick.AddListener(OnClickRecruitOrUpgrade);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 직원 카드 Button의 OnClick에서 호출하거나, UI_StaffDevelopCard가 호출합니다.
    /// </summary>
    public void ShowStaff(EmployeeType employeeType)
    {
        selectedEmployeeType = employeeType;
        selectedEmployeeData = EmployeeDataDB.GetData(employeeType);
        selectedUpgradeData = UpgradeDataDB.GetData(employeeType);

        if (selectedEmployeeData == null)
            return;

        gameObject.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        if (selectedEmployeeData == null || GameManager.Instance == null)
            return;

        int currentLevel = GameManager.Instance.Upgrade.RuntimeLevel.Get(selectedEmployeeType);

        SetTopInfo(currentLevel);
        SetSkills(currentLevel);
        SetRecruitUpgradeButton(currentLevel);
    }

    private void SetTopInfo(int currentLevel)
    {
        // RoleIcon과 스킬 데이터는 아래 EmployeeDataSO 추가 코드에서 제공합니다.
        roleIcon.sprite = selectedEmployeeData.RoleIcon;
        staffNameText.text = selectedEmployeeData.DisplayName;
        levelText.text = $"Lv.{currentLevel}";

        for (int i = 0; i < levelSlotImages.Length; i++)
        {
            if (levelSlotImages[i] == null)
                continue;

            levelSlotImages[i].color = i < currentLevel
                ? filledLevelSlotColor
                : emptyLevelSlotColor;
        }
    }

    private void SetSkills(int currentLevel)
    {
        int maxLevel = selectedUpgradeData == null ? 0 : selectedUpgradeData.MaxLevel;
        EmployeeSkillInfo skill1 = selectedEmployeeData.GetSkill(1);
        EmployeeSkillInfo skill3 = selectedEmployeeData.GetSkill(3);
        EmployeeSkillInfo skill5 = selectedEmployeeData.GetSkill(5);

        // 1레벨 스킬은 고용과 동시에 해금됩니다.
        firstSkillNameText.text = skill1.Name;
        firstSkillDescriptionText.text = currentLevel >= 1
            ? skill1.Description : "직원을 모집하면 해금됩니다.";
        firstSkillEffectText.text = currentLevel >= 1 ? skill1.Effect : "";

        // '다음 레벨'은 현재 레벨 다음 수치를 보여 줍니다.
        int nextLevel = Mathf.Min(currentLevel + 1, maxLevel);
        nextLevelDescriptionText.text = currentLevel >= maxLevel
            ? "최대 레벨입니다."
            : $"다음 레벨: Lv.{nextLevel}";
        nextLevelEffectText.text = currentLevel >= maxLevel
            ? ""
            : selectedEmployeeData.GetLevelEffect(nextLevel);

        SetLockedSkill(level3UnlockIcon, level3SkillNameText, level3LabelText,
            level3DescriptionText, skill3, 3, currentLevel);
        SetLockedSkill(level5UnlockIcon, level5SkillNameText, level5LabelText,
            level5DescriptionText, skill5, 5, currentLevel);
    }

    private void SetLockedSkill(Image unlockIcon, TMP_Text nameText, TMP_Text labelText,
        TMP_Text descriptionText, EmployeeSkillInfo skill, int unlockLevel, int currentLevel)
    {
        bool unlocked = currentLevel >= unlockLevel;

        unlockIcon.color = unlocked ? Color.white : new Color(1f, 1f, 1f, 0.35f);
        nameText.text = skill.Name;
        labelText.text = unlocked ? $"Lv.{unlockLevel} 해금" : $"Lv.{unlockLevel} 잠김";
        descriptionText.text = unlocked
            ? $"{skill.Description}\n{skill.Effect}"
            : $"Lv.{unlockLevel} 달성 시 해금됩니다.";
    }

    private void SetRecruitUpgradeButton(int currentLevel)
    {
        bool isMaxLevel = selectedUpgradeData == null
            || currentLevel >= selectedUpgradeData.MaxLevel;
        int cost = selectedUpgradeData == null ? 0 : selectedUpgradeData.GetCosts(currentLevel);
        bool canPay = !isMaxLevel
            && GameManager.Instance.StockManager.CanConsumeCurrency(cost);

        costText.text = isMaxLevel ? "-" : cost.ToString("N0");
        actionText.text = isMaxLevel
            ? "MAX"
            : currentLevel == 0 ? "Recruit" : $"Lv.{currentLevel + 1} Upgrade";
        recruitUpgradeButton.interactable = canPay;
        recruitUpgradeButton.image.color = canPay
            ? availableButtonColor : unavailableButtonColor;
    }

    private void OnClickRecruitOrUpgrade()
    {
        if (selectedUpgradeData == null)
            return;

        // UpgradeManager가 비용 차감, 레벨 저장, RuntimeStat 재계산을 한 번에 처리합니다.
        if (GameManager.Instance.Upgrade.TryUpgrade(selectedUpgradeData))
        {
            Refresh();

            // StaffListPanel의 모든 카드도 갱신합니다.
            UI_StaffDevelopCard[] cards = FindObjectsByType<UI_StaffDevelopCard>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

            foreach (UI_StaffDevelopCard card in cards)
                card.Refresh();
        }
    }

}
