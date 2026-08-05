using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class MarketData
{
    [SerializeField, Min(0)] private int currentBusinessDay;
    [SerializeField, Min(0)] private int dishSelectionLimit = 1;
    [SerializeField] private List<EmployeeType> unlockedEmployees = new();
    [SerializeField] private List<FacilityType> unlockedFacilities = new();
    [SerializeField] private List<DishType> selectedDishes = new();

    public int CurrentBusinessDay
    {
        get => currentBusinessDay;
        internal set => currentBusinessDay = value;
    }

    public int DishSelectionLimit
    {
        get => dishSelectionLimit;
        internal set => dishSelectionLimit = value;
    }

    public IReadOnlyList<EmployeeType> UnlockedEmployees => unlockedEmployees;
    public IReadOnlyList<FacilityType> UnlockedFacilities => unlockedFacilities;
    public List<DishType> SelectedDishes => selectedDishes;

    public MarketData()
    {
    }

    internal MarketData(
        int currentBusinessDay,
        int dishSelectionLimit,
        List<EmployeeType> unlockedEmployees,
        List<FacilityType> unlockedFacilities,
        List<DishType> selectedDishes)
    {
        this.currentBusinessDay = currentBusinessDay;
        this.dishSelectionLimit = dishSelectionLimit;
        this.unlockedEmployees = unlockedEmployees == null
            ? new List<EmployeeType>()
            : new List<EmployeeType>(unlockedEmployees);
        this.unlockedFacilities = unlockedFacilities == null
            ? new List<FacilityType>()
            : new List<FacilityType>(unlockedFacilities);
        this.selectedDishes = selectedDishes == null
            ? new List<DishType>()
            : new List<DishType>(selectedDishes);
    }
}
