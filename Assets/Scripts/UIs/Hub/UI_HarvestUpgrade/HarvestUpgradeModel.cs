using UnityEngine;

public sealed class HarvestUpgradeModel : MonoBehaviour
{
    [SerializeField] private GameObject sawSizeHighlight;
    [SerializeField] private GameObject sawCountHighlight;
    [SerializeField] private GameObject sawSpeedHighlight;
    [SerializeField] private GameObject sawSharpnessHighlight;
    [SerializeField] private GameObject truckSpeedHighlight;
    [SerializeField] private GameObject truckCapacityHighlight;
    [SerializeField] private GameObject truckFuelHighlight;
    [SerializeField] private GameObject goldenPigRadarHighlight;

    private void Awake()
    {
        ClearHighlight();
    }

    public void ShowHighlight(HarvestUpgradeType upgradeType)
    {
        ClearHighlight();

        GameObject target = upgradeType switch
        {
            HarvestUpgradeType.SawSize => sawSizeHighlight, 
            HarvestUpgradeType.SawSpeed => sawSpeedHighlight,
            HarvestUpgradeType.SawSharpness => sawSharpnessHighlight,
            HarvestUpgradeType.TruckSpeed => truckSpeedHighlight,
            HarvestUpgradeType.TruckCapacity => truckCapacityHighlight,
            HarvestUpgradeType.TruckFuel => truckFuelHighlight,
            HarvestUpgradeType.GoldenPigRadar => goldenPigRadarHighlight,
            _ => null
        };

        target?.SetActive(true);
    }

    public void ClearHighlight()
    {
        sawSizeHighlight?.SetActive(false);
        sawCountHighlight?.SetActive(false);
        sawSpeedHighlight?.SetActive(false);
        sawSharpnessHighlight?.SetActive(false);
        truckSpeedHighlight?.SetActive(false);
        truckCapacityHighlight?.SetActive(false);
        truckFuelHighlight?.SetActive(false);
        goldenPigRadarHighlight.SetActive(false);
    }
}
