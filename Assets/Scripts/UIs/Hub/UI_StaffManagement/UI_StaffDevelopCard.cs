using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StaffDevelopStatus
{
    Opened,      // 고용되어 있지만, 현재 보유 재화로는 강화 불가
    CanRecruit,  // 미고용 + 모집 비용 보유
    CanUpgrade,  // 고용됨 + 다음 레벨 비용 보유
    Locked       // 미고용 + 모집 비용 미보유
}

/// <summary>
/// StaffListPanel 안의 고정 배치 카드 하나에 붙입니다.
/// 카드 생성/배치는 하지 않으며, 연결된 EmployeeType의 상태만 표시합니다.
/// </summary>
public sealed class UI_StaffDevelopCard : MonoBehaviour
{
    [Header("Card UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image outlineImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject upgradeArrow;
    [SerializeField] private GameObject recruitReadyText;
    [SerializeField] private GameObject lockOverlay;

    [Header("Card state appearance")]
    [SerializeField] private Color normalOutlineColor = new Color(1f, 1f, 1f, 0f);
    [SerializeField] private Color availableOutlineColor = new Color(1f, 0.78f, 0.1f, 1f);
    [SerializeField] private Color lockedOutlineColor = new Color(0.35f, 0.35f, 0.35f, 0.85f);
    [SerializeField, Min(0f)] private float glowSpeed = 3f;

    [Header("Fixed card data")]
    [SerializeField] private EmployeeType employeeType = EmployeeType.Count;

    private StaffDevelopStatus status;
    private UI_StaffInfoPanel staffInfoPanel;
    private Button cardButton;

    private void Awake()
    {
        cardButton = GetComponent<Button>();
        cardButton.onClick.AddListener(OnClickCard);
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        // 모집/강화 가능 카드의 테두리만 약하게 점멸합니다.
        if (outlineImage == null) return;

        bool available = status == StaffDevelopStatus.CanRecruit
                         || status == StaffDevelopStatus.CanUpgrade;

        if (!available) return;

        Color color = availableOutlineColor;
        color.a = Mathf.Lerp(0.35f, 1f,
            (Mathf.Sin(Time.unscaledTime * glowSpeed) + 1f) * 0.5f);
        outlineImage.color = color;
    }

    /// <summary>
    /// UI_StaffManagement에서 각 고정 카드에 한 번 호출합니다.
    /// Inspector에서 employeeType을 지정했다면 호출하지 않아도 됩니다.
    /// </summary>
    public void SetData(EmployeeType value)
    {
        employeeType = value;
        Refresh();
    }

    // UI_StaffManagement이 각 고정 카드와 StaffInfoPanel을 연결할 때 사용합니다.
    public void Init(UI_StaffInfoPanel panel)
    {
        staffInfoPanel = panel;
    }

    /// <summary>
    /// 재화 변동, 모집, 강화가 끝난 뒤 호출하면 카드 표시를 갱신합니다.
    /// </summary>
    public void Refresh()
    {
        if (employeeType == EmployeeType.Count || GameManager.Instance == null)
            return;

        EmployeeDataSO employeeData = EmployeeDataDB.GetData(employeeType);
        EmployeeUpgradeDataSO upgradeData = UpgradeDataDB.GetData(employeeType);
        if (employeeData == null || upgradeData == null)
            return;

        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(employeeType);
        float currency = GameManager.Instance.StockManager.StockData.Currency;

        SetStatus(GetStatus(upgradeData, level, currency), level);
    }

    public void SetStatus(StaffDevelopStatus newStatus, int currentLevel)
    {
        status = newStatus;

        bool canRecruit = newStatus == StaffDevelopStatus.CanRecruit;
        bool canUpgrade = newStatus == StaffDevelopStatus.CanUpgrade;
        bool locked = newStatus == StaffDevelopStatus.Locked;

        // LevelText와 RecruitReadyText는 같은 자리에 겹쳐 둡니다.
        // 일반/강화 가능은 레벨만, 모집 가능은 '모집 대기'만 표시합니다.
        if (levelText != null)
        {
            levelText.text = $"Lv.{currentLevel}";
            levelText.gameObject.SetActive(!canRecruit);
        }
        if (upgradeArrow != null)
            upgradeArrow.SetActive(canUpgrade);
        if (recruitReadyText != null)
            recruitReadyText.SetActive(canRecruit);
        if (lockOverlay != null)
            lockOverlay.SetActive(locked);

        if (outlineImage == null)
            return;

        outlineImage.color = (canRecruit || canUpgrade)
            ? availableOutlineColor
            : locked ? lockedOutlineColor : normalOutlineColor;
    }

    private StaffDevelopStatus GetStatus(
        EmployeeUpgradeDataSO upgradeData,
        int level,
        float currency)
    {
        // Level 0은 미고용 상태입니다. 패널의 Recruit 버튼과 같은 강화 SO 비용을 사용합니다.
        if (level <= 0)
        {
            bool canRecruit = currency >= upgradeData.GetCosts(0);
            return canRecruit ? StaffDevelopStatus.CanRecruit : StaffDevelopStatus.Locked;
        }

        bool isMaxLevel = level >= upgradeData.MaxLevel;

        // 최대 레벨이면 일반 카드처럼 레벨만 표시합니다.
        if (isMaxLevel)
            return StaffDevelopStatus.Opened;

        int nextLevelCost = upgradeData.GetCosts(level);
        return currency >= nextLevelCost
            ? StaffDevelopStatus.CanUpgrade
            : StaffDevelopStatus.Opened;
    }

    private void OnClickCard()
    {
        if (staffInfoPanel != null)
            staffInfoPanel.ShowStaff(employeeType);
    }
}
