using System.Collections.Generic;

public class StatCalculator
{
    public RuntimeStat Calculate(IReadOnlyList<UpgradeState> upgradeStates)
    {
        RuntimeStat calculatedStat = new RuntimeStat();

        calculatedStat.Harvest.Initialize();
        calculatedStat.Service.Initialize();

        ApplyStates(calculatedStat, upgradeStates);

        return calculatedStat;
    }

    private static void ApplyStates(
        RuntimeStat calculatedStat,
        IReadOnlyList<UpgradeState> upgradeStates)
    {
        if (upgradeStates == null)
            return;

        for (int i = 0; i < upgradeStates.Count; i++)
        {
            UpgradeState state = upgradeStates[i];

            if (state?.data == null || state.level <= 0)
                continue;

            switch (state.data)
            {
                case HarvestUpgradeDataSO harvestData:
                    calculatedStat.Harvest.Apply(
                        harvestData.StatModifiers,
                        state.level);
                    break;

                case FacilityUpgradeDataSO facilityData:
                    calculatedStat.Service.Apply(
                        facilityData.ServiceStatModifiers,
                        state.level);
                    break;
            }
        }
    }
}
