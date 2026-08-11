using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 카드 한 장의 데이터 조회, UI 표시, 클릭 전달을 담당합니다.
public sealed class UI_StaffDevelopCard : MonoBehaviour
{
    private enum StaffCardState
    {
        Locked,
        Normal,
        CanRecruit,
        CanUpgrade
    }

    [SerializeField] private EmployeeType employeeType = EmployeeType.Count;

    [Header("UI")]
    [SerializeField] private Image outlineImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject recruitReadyText;
    [SerializeField] private GameObject upgradeArrow;
    [SerializeField] private GameObject lockOverlay;

    [Header("Color")]
    [SerializeField] private Color normalColor = new(1f, 1f, 1f, 0f);
    [SerializeField] private Color availableColor = new(1f, .78f, .1f, 1f);
    [SerializeField] private Color lockedColor = new(.35f, .35f, .35f, .85f);

    public EmployeeType EmployeeType => employeeType;

    private Action<EmployeeType> onSelected;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    public void Initialize(Action<EmployeeType> callback)
    {
        onSelected = callback;
    }

    // ListPanel이 호출합니다.
    // EmployeeData가 있으면 true, 없으면 false를 반환합니다.
    public bool Refresh()
    {
        if (!CreateInfoData(out int level, out StaffCardState state))
            return false;

        ApplyView(level, state);
        return true;
    }

    // 카드 표시용 최소 정보만 직접 생성합니다.
    private bool CreateInfoData(out int level, out StaffCardState state)
    {
        level = 0;
        state = StaffCardState.Locked;

        if (employeeType == EmployeeType.Count)
            return false;

        if (!EmployeeDataDB.TryGetData(employeeType, out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"EmployeeData가 없습니다: {employeeType}");
            return false;
        }

        UpgradeManager upgrade = GameManager.Instance.Upgrade;
        StockManager stockManager = GameManager.Instance.StockManager;

        level = upgrade.GetLevel(employeeType);

        EmployeeUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(employeeData.Id) as EmployeeUpgradeDataSO;

        bool canPay = false;

        if (upgradeData != null && level < employeeData.MaxLevel)
        {
            int cost = upgradeData.GetCosts(level);
            canPay = stockManager.CanConsumeCurrency(cost);
        }

        if (level == 0)
        {
            state = canPay
                ? StaffCardState.CanRecruit : StaffCardState.Locked;
        }
        else if (level >= employeeData.MaxLevel)
        {
            state = StaffCardState.Normal;
        }
        else
        {
            state = canPay
                ? StaffCardState.CanUpgrade : StaffCardState.Normal;
        }

        return true;
    }

    private void ApplyView(int level, StaffCardState state)
    {
        bool canRecruit = state == StaffCardState.CanRecruit;
        bool canUpgrade = state == StaffCardState.CanUpgrade;
        bool isLocked = state == StaffCardState.Locked;

        levelText.text = $"Lv.{level}";
        levelText.gameObject.SetActive(!canRecruit);

        if (recruitReadyText != null) recruitReadyText.SetActive(canRecruit);

        if (upgradeArrow != null) upgradeArrow.SetActive(canUpgrade);

        if (lockOverlay != null) lockOverlay.SetActive(isLocked);

        if (outlineImage != null)
        {
            outlineImage.color = canRecruit || canUpgrade
                ? availableColor : isLocked ? lockedColor : normalColor;
        }
    }

    private void OnClick()
    {
        onSelected?.Invoke(employeeType);
    }
}