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
    [SerializeField] private Button button;

    [Header("Color")]
    [SerializeField] private Color normalColor = new(1f, 1f, 1f, 0f);
    [SerializeField] private Color availableColor = new(1f, .78f, .1f, 1f);
    [SerializeField] private Color lockedColor = new(.35f, .35f, .35f, .85f);

    public EmployeeType EmployeeType => employeeType;

    private Action<EmployeeType> onSelected;
    

    private void Awake()
    {
        if (button == null)
        {
            Debug.LogError($"[StaffCard] Button이 연결되지 않았습니다: {gameObject.name}",this);
            return;
        }
        button.onClick.AddListener(OnClick);
    }

    public void Initialize(Action<EmployeeType> callback)
    {
        onSelected = callback;
    }


    public bool Refresh()
    {
        if (!CreateInfoData(out int level, out StaffCardState state))
            return false;

        ApplyView(level, state);
        return true;
    }


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
            // 구매 전에는 재화가 충분해도 항상 Locked 상태
            state = StaffCardState.Locked;
        }
        else if (level >= upgradeData.MaxLevel)
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
        bool isPurchased = level > 0;
        bool canRecruit = state == StaffCardState.CanRecruit;
        bool canUpgrade = state == StaffCardState.CanUpgrade;

        // 구매 전이면 무조건 잠금 이미지 표시
        bool isLocked = !isPurchased;

        if (levelText != null)
        {
            levelText.text = $"Lv.{level}";
            levelText.gameObject.SetActive(isPurchased);
        }

        if (recruitReadyText != null) recruitReadyText.SetActive(canRecruit);

        if (upgradeArrow != null) upgradeArrow.SetActive(canUpgrade);

        if (lockOverlay != null) lockOverlay.SetActive(isLocked);

        if (outlineImage != null)
        {
            outlineImage.color =
                canRecruit || canUpgrade ? availableColor
                    : isLocked ? lockedColor : normalColor;
        }
    }

    private void OnClick()
    {
        onSelected?.Invoke(employeeType);
    }
}
