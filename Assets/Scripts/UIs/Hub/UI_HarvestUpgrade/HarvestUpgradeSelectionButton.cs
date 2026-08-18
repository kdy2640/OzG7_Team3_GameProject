using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HarvestUpgradeSelectionButton : MonoBehaviour
{
    [SerializeField] private HarvestUpgradeType upgradeType = HarvestUpgradeType.Count;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private Color normalColor = new(0.18f, 0.22f, 0.25f, 0.95f);
    [SerializeField] private Color selectedColor = new(0.88f, 0.58f, 0.14f, 1f);

    private HarvestUpgradeListPanel owner;

    public HarvestUpgradeType UpgradeType => upgradeType;

    internal void Initialize(HarvestUpgradeListPanel listPanel)
    {
        owner = listPanel;

        if (button == null)
            button = GetComponent<Button>();

        if (labelText == null)
            labelText = GetComponentInChildren<TMP_Text>(true);

        HarvestUpgradeDataSO data = UpgradeDataDB.GetData(upgradeType);
        if (labelText != null)
            labelText.text = data != null ? data.DisplayName : upgradeType.ToString();

        button?.onClick.RemoveListener(OnClick);
        button?.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveListener(OnClick);
    }

    internal void SetSelected(bool isSelected)
    {
        if (button?.image != null)
            button.image.color = isSelected ? selectedColor : normalColor;
    }

    private void OnClick()
    {
        owner?.Select(upgradeType);
    }
}
