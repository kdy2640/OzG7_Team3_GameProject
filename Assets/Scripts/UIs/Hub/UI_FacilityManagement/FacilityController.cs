using System;
using UnityEngine;

public class FacilityController : MonoBehaviour
{
    [Header("Facility")]
    [SerializeField] private FacilityType facilityType = FacilityType.Count;

    [Header("View")]
    [SerializeField] private FacilityModelView modelView;
    [SerializeField] private FacilityWorldUI worldUI;

    private UpgradeManager SubscribedUpgradeManager => GameManager.Instance?.Upgrade;

    private FacilityUpgradeDataSO UpgradeData => UpgradeDataDB.GetData(facilityType);

    public FacilityType FacilityType => facilityType;

    public string FacilityName
    {
        get
        {
            FacilityDataSO data = FacilityDataDB.GetData(facilityType);

            return data != null ? data.DisplayName : facilityType.ToString();
        }
    }

    public int CurrentLevel => SubscribedUpgradeManager.RuntimeLevel.Get(facilityType); 
    public bool IsPurchased => CurrentLevel > 0;

    public int MaxLevel => UpgradeData != null ? UpgradeData.MaxLevel : 0;

    public event Action<FacilityController> StateChanged;

    internal void SetSelected(bool isSelected)
    {
        modelView.SetSelected(isSelected);
    }

    private void OnEnable()
    {
        SubscribedUpgradeManager.SubscribeUpgradeChanged(Refresh);
        Refresh();
    }

    private void OnDisable()
    {
        SubscribedUpgradeManager.UnsubscribeUpgradeChanged(Refresh);
    }

    #region Upgrade
      

    public bool CanUpgrade()
    {
        return GetUpgradeAvailability() == UpgradeAvailability.Available;
    }

    public UpgradeAvailability GetUpgradeAvailability()
    { 

        if (SubscribedUpgradeManager == null)
            return UpgradeAvailability.InvalidData;

        if (UpgradeData == null)
            return UpgradeAvailability.InvalidData;

        return SubscribedUpgradeManager.GetUpgradeAvailability(UpgradeData);
    }
     
    #endregion
     
     

    private void Refresh()
    {
        int level = CurrentLevel;

        modelView?.ShowLevel(level);

        worldUI?.Refresh(
            isPurchased: level > 0,
            currentLevel: level,
            canUpgrade: CanUpgrade()
        );

        StateChanged?.Invoke(this);
    }
     
}
