using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_StaffDevelopCard : MonoBehaviour
{
    private enum StaffCardState
    {
        Locked,
        Normal,
        CanUpgrade
    }

    [SerializeField]
    private EmployeeType employeeType =
        EmployeeType.Count;

    [Header("UI")]
    [SerializeField] private Image outlineImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject recruitReadyText;
    [SerializeField] private GameObject upgradeArrow;
    [SerializeField] private GameObject lockOverlay;

    [Header("Color")]
    [SerializeField]
    private Color normalColor =
        new(1f, 1f, 1f, 0f);

    [SerializeField]
    private Color availableColor = new(1f, .78f, .1f, 1f);

    [SerializeField]
    private Color lockedColor = new(.35f, .35f, .35f, .85f);

    public EmployeeType EmployeeType => employeeType;

    private Action<EmployeeType> onSelected;

    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null)
        {
            Debug.LogError($"[StaffCard] Button이 연결되지 않았습니다: {gameObject.name}");

            return;
        }

        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Initialize(Action<EmployeeType> callback)
    {
        onSelected = callback;
        Refresh();
    }

    public void Refresh()
    {
        if (!CreateInfoData(out int level, out StaffCardState state))
        {
            return;
        }

        ApplyView(level, state);
    }

    private bool CreateInfoData(out int level,out StaffCardState state)
    {
        level = 0;
        state = StaffCardState.Locked;

        if (employeeType == EmployeeType.Count)
            return false;

        if (!EmployeeDataDB.TryGetData(employeeType,out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"EmployeeData가 없습니다: {employeeType}");

            return false;
        }

        UpgradeManager upgrade = GameManager.Instance.Upgrade;

        level = upgrade.RuntimeLevel.Get(employeeType);

        // 구매 전
        if (level <= 0)
        {
            state = StaffCardState.Locked;
            return true;
        }

        // 최대 레벨
        EmployeeUpgradeDataSO upgradeData = UpgradeDataDB.GetData(employeeType);

        if (upgradeData == null)
        {
            Debug.LogWarning($"[StaffCard] {employeeType} UpgradeData가 없습니다.");

            state = StaffCardState.Normal;
            return true;
        }

        // 최대 레벨
        if (level >= upgradeData.MaxLevel)
        {
            state = StaffCardState.Normal;
            return true;
        }

        UpgradeAvailability availability = upgrade.GetUpgradeAvailability(upgradeData);

        bool canUpgrade = availability == UpgradeAvailability.Available;

        Debug.Log(
            $"[StaffCard] {employeeType} " +
            $"Lv={level} " +
            $"MaxLv={upgradeData.MaxLevel} " +
            $"Availability={availability} " +
            $"Cost={upgradeData.GetCosts(level)}"
        );

        state = canUpgrade
            ? StaffCardState.CanUpgrade : StaffCardState.Normal;

        return true;
    }

    private void ApplyView(int level, StaffCardState state)
    {
        bool canUpgrade =
            state == StaffCardState.CanUpgrade;

        bool isLocked =
            state == StaffCardState.Locked;

        if (levelText != null)
        {
            levelText.text = $"Lv.{level}";
            levelText.gameObject.SetActive(!isLocked);
        }

        if (recruitReadyText != null) recruitReadyText.SetActive(isLocked);

        if (upgradeArrow != null) upgradeArrow.SetActive(canUpgrade);

        if (lockOverlay != null) lockOverlay.SetActive(isLocked);

        if (outlineImage != null)
        {
            outlineImage.color =
                canUpgrade ? availableColor : 
                isLocked ? lockedColor : normalColor;
        }
    }

    private void OnClick()
    {
        onSelected?.Invoke(employeeType);
    }
}