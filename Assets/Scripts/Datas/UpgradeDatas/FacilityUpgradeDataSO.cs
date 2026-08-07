using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Facility")]
public sealed class FacilityUpgradeDataSO : UpgradeDataSO
{
    [SerializeField] private FacilityType targetFacility = FacilityType.Count;
    [SerializeField] private List<ServiceStatModifier> serviceStatModifiers = new();

    public FacilityType TargetFacility => targetFacility;
    public IReadOnlyList<ServiceStatModifier> ServiceStatModifiers => serviceStatModifiers;
}
