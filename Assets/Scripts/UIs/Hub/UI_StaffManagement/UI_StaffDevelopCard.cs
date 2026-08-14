using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ī�� �� ���� ������ ��ȸ, UI ǥ��, Ŭ�� ������ ����մϴ�.
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

    // ListPanel�� ȣ���մϴ�.
    // EmployeeData�� ������ true, ������ false�� ��ȯ�մϴ�.
    public bool Refresh()
    {
        if (!CreateInfoData(out int level, out StaffCardState state))
            return false;

        ApplyView(level, state);
        return true;
    }

    // ī�� ǥ�ÿ� �ּ� ������ ���� �����մϴ�.
    private bool CreateInfoData(out int level, out StaffCardState state)
    {
        level = 0;
        state = StaffCardState.Locked;

        if (employeeType == EmployeeType.Count)
            return false;

        if (!EmployeeDataDB.TryGetData(employeeType, out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"EmployeeData�� �����ϴ�: {employeeType}");
            return false;
        }

        UpgradeManager upgrade = GameManager.Instance.Upgrade;

        level = upgrade.RuntimeLevel.Get(employeeType);

        EmployeeUpgradeDataSO upgradeData = UpgradeDataDB.GetData(employeeType);

        bool canUpgrade = upgrade.CanUpgrade(upgradeData);

        if (level == 0)
        {
            state = canUpgrade
                ? StaffCardState.CanRecruit : StaffCardState.Locked;
        }
        else if (level >= employeeData.MaxLevel)
        {
            state = StaffCardState.Normal;
        }
        else
        {
            state = canUpgrade
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
