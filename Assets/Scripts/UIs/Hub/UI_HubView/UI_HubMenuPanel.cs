using UnityEngine;

public sealed class UI_HubMenuPanel : MonoBehaviour
{
    [SerializeField] private UI_HubStateButton facilityManagementButton;
    [SerializeField] private UI_HubStateButton menuManagementButton;
    [SerializeField] private UI_HubStateButton harvestUpgradeButton;
    [SerializeField] private UI_HubStateButton staffManagementButton;

    public void Init(HubCanvasController owner)
    {
        facilityManagementButton.Init(owner);
        menuManagementButton.Init(owner);
        harvestUpgradeButton.Init(owner);
        staffManagementButton.Init(owner);
    }
}
