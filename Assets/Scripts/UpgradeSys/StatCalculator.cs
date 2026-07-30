using System.Collections.Generic;

public class StatCalculator
{
    public RuntimeStat Calculate(IReadOnlyList<UpgradeState> upgradeStates)
    {
        RuntimeStat calculatedStat = new RuntimeStat();

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

            if (state?.data?.statModifiers == null)
                continue;

            foreach (StatModifier modifier in state.data.statModifiers)
                calculatedStat.Apply(modifier, state.level);
        }
    }
}
