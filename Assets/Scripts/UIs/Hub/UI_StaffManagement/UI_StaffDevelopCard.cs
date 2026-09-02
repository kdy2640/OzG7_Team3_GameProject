using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class UI_StaffDevelopCard : MonoBehaviour
{
    private const float SelectedScaleMultiplier = 1.05f;
    private const float SelectionTweenDuration = 0.15f;

    private enum StaffCardState
    {
        Locked,
        Normal,
        CanRecruit,
        CanUpgrade
    }

    [SerializeField] private EmployeeType employeeType = EmployeeType.Count;

    [Header("UI")]
    [SerializeField] private GameObject selectedCard;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text levelShadowText;
    [SerializeField] private GameObject recruitReadyText;
    [SerializeField] private GameObject upgradeArrow;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Button button;

    public EmployeeType EmployeeType => employeeType;

    private Action<EmployeeType> onSelected;
    private Vector3 defaultScale;
    private Tween selectionTween;

    private void Awake()
    {
        defaultScale = transform.localScale;

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

    public void SetSelected(bool isSelected)
    {
        selectedCard.SetActive(isSelected);

        selectionTween?.Kill();

        Vector3 targetScale = isSelected
            ? defaultScale * SelectedScaleMultiplier
            : defaultScale;

        selectionTween = transform
            .DOScale(targetScale, SelectionTweenDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .OnComplete(() => selectionTween = null);
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

        UpgradeAvailability availability =
            upgrade.GetUpgradeAvailability(upgradeData);

        if (level == 0)
        {
            // 구매 가능 조건을 만족하면 모집 가능 상태로 표시합니다.
            state = availability == UpgradeAvailability.Available
                ? StaffCardState.CanRecruit
                : StaffCardState.Locked;
        }
        else if (level >= upgradeData.MaxLevel)
        {
            state = StaffCardState.Normal;
        }
        else
        {
            state = availability == UpgradeAvailability.Available
                ? StaffCardState.CanUpgrade : StaffCardState.Normal;
        }

        return true;
    }

    private void ApplyView(int level, StaffCardState state)
    {
        bool isPurchased = level > 0;
        bool canRecruit = state == StaffCardState.CanRecruit;
        bool canUpgrade = state == StaffCardState.CanUpgrade;

        bool isLocked = !isPurchased;

        string levelLabel = $"Lv.{level}";
        levelText.text = levelLabel;
        levelShadowText.text = levelLabel;
        levelText.gameObject.SetActive(isPurchased);
        levelShadowText.gameObject.SetActive(isPurchased);

        if (recruitReadyText != null) recruitReadyText.SetActive(canRecruit);

        if (upgradeArrow != null) upgradeArrow.SetActive(canUpgrade);

        if (lockOverlay != null) lockOverlay.SetActive(isLocked);

    }

    private void OnClick()
    {
        onSelected.Invoke(employeeType);
    }

    private void OnDisable()
    {
        selectionTween?.Kill();
        selectionTween = null;
        transform.localScale = defaultScale;
    }
}
