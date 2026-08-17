using System.Collections.Generic;
using UnityEngine;

public sealed class StageResolver
{
    private readonly List<StageEntry> entries = new();

    private sealed class StageEntry
    {
        public readonly StageDataSO StageData;
        public readonly List<HarvestType> StaticTypes = new();
        public readonly List<HarvestType> MovableTypes = new();

        public StageEntry(StageDataSO stageData)
        {
            StageData = stageData;
        }
    }

    public void Initialize()
    {
        entries.Clear();

        foreach (StageDataSO stageData in StageDataDB.GetAllData())
        {
            StageEntry entry = new(stageData);

            foreach (HarvestType harvestType in stageData.HarvestList)
            {
                if (!HarvestDataDB.TryGetData(
                        harvestType,
                        out HarvestDataSO harvestData))
                {
                    Debug.LogWarning(
                        $"There is no HarvestDataSO. harvestType : {harvestType}");
                    continue;
                }

                if (harvestData.IsMove)
                {
                    entry.MovableTypes.Add(harvestType);
                }
                else
                {
                    entry.StaticTypes.Add(harvestType);
                }
            }

            entries.Add(entry);
        }

        entries.Sort((a, b) =>
            a.StageData.ZStart.CompareTo(b.StageData.ZStart));
    }

    public bool TryGetStaticType(float localZ, out HarvestType type)
    {
        return TryGetType(localZ, false, out type);
    }

    public bool TryGetMovableType(float localZ, out HarvestType type)
    {
        return TryGetType(localZ, true, out type);
    }

    private bool TryGetType(
        float localZ,
        bool isMovable,
        out HarvestType type)
    {
        StageEntry entry = GetEntry(localZ);
        List<HarvestType> types = isMovable
            ? entry?.MovableTypes
            : entry?.StaticTypes;

        if (types == null || types.Count == 0)
        {
            type = HarvestType.Count;
            return false;
        }

        type = types[Random.Range(0, types.Count)];
        return true;
    }

    private StageEntry GetEntry(float localZ)
    {
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            StageEntry entry = entries[i];

            if (localZ >= entry.StageData.ZStart)
            {
                return entry;
            }
        }

        return null;
    }
}
