using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Employee")]
public sealed class EmployeeUpgradeDataSO : UpgradeDataSO
{
    [SerializeField] private EmployeeType targetEmployee = EmployeeType.Count;

    public EmployeeType TargetEmployee => targetEmployee;
}
