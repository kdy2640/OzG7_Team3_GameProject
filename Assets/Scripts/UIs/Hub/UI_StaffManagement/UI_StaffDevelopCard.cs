using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 카드 한 장은 UI 표시와 클릭 전달만 담당합니다. GameManager/DB를 참조하지 않습니다.
public sealed class UI_StaffDevelopCard : MonoBehaviour
{
    [SerializeField] private EmployeeType employeeType = EmployeeType.Count;
    [SerializeField] private Image outlineImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject recruitReadyText;
    [SerializeField] private GameObject upgradeArrow;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Color normalColor = new Color(1, 1, 1, 0);
    [SerializeField] private Color availableColor = new Color(1, .78f, .1f, 1);
    [SerializeField] private Color lockedColor = new Color(.35f, .35f, .35f, .85f);

    public EmployeeType EmployeeType => employeeType;
    private Action<EmployeeType> onSelected;
    private StaffCardState state;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        onSelected?.Invoke(employeeType);
    }
    public void Initialize(Action<EmployeeType> callback) => onSelected = callback;

    public void SetView(StaffCardUIData data)
    {
        state = data.state;
        bool recruit = state == StaffCardState.CanRecruit;
        bool upgrade = state == StaffCardState.CanUpgrade;
        levelText.text = $"Lv.{data.level}";
        levelText.gameObject.SetActive(!recruit);
        if (recruitReadyText != null) recruitReadyText.SetActive(recruit);
        if (upgradeArrow != null) upgradeArrow.SetActive(upgrade);
        if (lockOverlay != null) lockOverlay.SetActive(state == StaffCardState.Locked);
        if (outlineImage != null) outlineImage.color = (recruit || upgrade) ? availableColor : state == StaffCardState.Locked ? lockedColor : normalColor;
    }
}
