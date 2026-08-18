using UnityEngine;
using UnityEngine.UI;

public sealed class HarvestUpgradePreviewPanel : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;

    private HarvestUpgradeListPanel subscribedListPanel;
    private HarvestUpgradeModel upgradeModel;

    public void Initialize(HarvestUpgradeListPanel listPanel)
    {
        if (subscribedListPanel == listPanel)
            return;

        if (subscribedListPanel != null)
            subscribedListPanel.OnSelected -= HandleSelected;

        subscribedListPanel = listPanel;

        if (subscribedListPanel != null)
            subscribedListPanel.OnSelected += HandleSelected;
    }

    private void OnEnable()
    {
        upgradeModel = FindFirstObjectByType<HarvestUpgradeModel>();
    }

    private void OnDisable()
    {
        upgradeModel?.ClearHighlight();
        upgradeModel = null;
    }

    private void OnDestroy()
    {
        if (subscribedListPanel != null)
            subscribedListPanel.OnSelected -= HandleSelected;
    }

    public void ClearHighlight()
    {
        if (upgradeModel == null)
            upgradeModel = FindFirstObjectByType<HarvestUpgradeModel>();

        upgradeModel?.ClearHighlight();
    }

    private void HandleSelected(HarvestUpgradeType upgradeType)
    {
        if (upgradeModel == null)
            upgradeModel = FindFirstObjectByType<HarvestUpgradeModel>();

        upgradeModel?.ShowHighlight(upgradeType);
    }
}
