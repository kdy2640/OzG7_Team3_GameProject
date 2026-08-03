using System;
using UnityEngine;

[Serializable]
public class FacilityBase
{
    [SerializeField] private FacilityDataSO dataSO;
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private int nowLevel;

    public string Id
    {
        get => id;
        set => id = value;
    }

    public string DisplayName
    {
        get => displayName;
        set => displayName = value;
    }

    public int NowLevel
    {
        get => nowLevel;
        set => nowLevel = value;
    }

    public FacilityBase(FacilityDataSO dataSO)
    {
        this.dataSO = dataSO;
        id = dataSO.Id;
        displayName = dataSO.DisplayName;
        nowLevel = 0;
    }
}
