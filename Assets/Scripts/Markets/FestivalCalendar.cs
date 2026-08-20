using System;
using UnityEngine;

[Serializable]
public sealed class FestivalCalendar
{
    #region Fields & Properties

    public const int TasteFestivalDuration = 4;
    public const int CategoryFestivalDuration = 3;

    [SerializeField] private TasteType latestTaste = TasteType.Count;
    [SerializeField, Min(-1)] private int tasteStartBusinessDay = -1;
    [SerializeField] private CategoryType latestCategory = CategoryType.Count;
    [SerializeField, Min(-1)] private int categoryStartBusinessDay = -1;

    public TasteType LatestTaste => latestTaste;
    public CategoryType LatestCategory => latestCategory;
    public int TasteStartBusinessDay => tasteStartBusinessDay;
    public int CategoryStartBusinessDay => categoryStartBusinessDay;
    public int TasteEndBusinessDay => tasteStartBusinessDay < 0
        ? -1
        : tasteStartBusinessDay + TasteFestivalDuration - 1;
    public int CategoryEndBusinessDay => categoryStartBusinessDay < 0
        ? -1
        : categoryStartBusinessDay + CategoryFestivalDuration - 1;

    #endregion

    #region Taste Festival

    // 해당 영업일에 진행 중인 맛 축제를 반환한다.
    public TasteType GetNowTaste(int businessDay)
    {
        return IsTasteFestivalActive(businessDay)
            ? latestTaste
            : TasteType.Count;
    }

    // 해당 영업일에 맛 축제가 진행 중인지 확인한다.
    public bool IsTasteFestivalActive(int businessDay)
    {
        return IsValidTaste(latestTaste)
            && tasteStartBusinessDay >= 0
            && businessDay >= tasteStartBusinessDay
            && businessDay <= TasteEndBusinessDay;
    }

    // 해당 영업일에 선택한 맛으로 축제를 시작할 수 있는지 확인한다.
    public bool CanStartTasteFestival(TasteType taste, int businessDay)
    {
        return businessDay >= 0
            && IsValidTaste(taste)
            && !IsTasteFestivalActive(businessDay)
            && taste != latestTaste;
    }

    // 해당 영업일에 선택한 맛 축제의 시작을 시도한다.
    public bool TryStartTasteFestival(TasteType taste, int businessDay)
    {
        if (!CanStartTasteFestival(taste, businessDay))
            return false;

        latestTaste = taste;
        tasteStartBusinessDay = businessDay;
        return true;
    }

    #endregion

    #region Category Festival

    // 해당 영업일에 진행 중인 카테고리 축제를 반환한다.
    public CategoryType GetNowCategory(int businessDay)
    {
        return IsCategoryFestivalActive(businessDay)
            ? latestCategory
            : CategoryType.Count;
    }

    // 해당 영업일에 카테고리 축제가 진행 중인지 확인한다.
    public bool IsCategoryFestivalActive(int businessDay)
    {
        return IsValidCategory(latestCategory)
            && categoryStartBusinessDay >= 0
            && businessDay >= categoryStartBusinessDay
            && businessDay <= CategoryEndBusinessDay;
    }

    // 해당 영업일에 선택한 카테고리로 축제를 시작할 수 있는지 확인한다.
    public bool CanStartCategoryFestival(CategoryType category, int businessDay)
    {
        return businessDay >= 0
            && IsValidCategory(category)
            && !IsCategoryFestivalActive(businessDay)
            && category != latestCategory;
    }

    // 해당 영업일에 선택한 카테고리 축제의 시작을 시도한다.
    public bool TryStartCategoryFestival(CategoryType category, int businessDay)
    {
        if (!CanStartCategoryFestival(category, businessDay))
            return false;

        latestCategory = category;
        categoryStartBusinessDay = businessDay;
        return true;
    }

    #endregion

    #region Save Data

    // 저장된 맛 및 카테고리 축제 상태를 유효한 값만 복원한다.
    public void Load(
        TasteType savedTaste,
        int savedTasteStartBusinessDay,
        CategoryType savedCategory,
        int savedCategoryStartBusinessDay)
    {
        if (IsValidTaste(savedTaste) && savedTasteStartBusinessDay >= 0)
        {
            latestTaste = savedTaste;
            tasteStartBusinessDay = savedTasteStartBusinessDay;
        }
        else
        {
            latestTaste = TasteType.Count;
            tasteStartBusinessDay = -1;
        }

        if (IsValidCategory(savedCategory) && savedCategoryStartBusinessDay >= 0)
        {
            latestCategory = savedCategory;
            categoryStartBusinessDay = savedCategoryStartBusinessDay;
        }
        else
        {
            latestCategory = CategoryType.Count;
            categoryStartBusinessDay = -1;
        }
    }

    // 맛 및 카테고리 축제 상태를 초기값으로 되돌린다.
    public void Reset()
    {
        latestTaste = TasteType.Count;
        tasteStartBusinessDay = -1;
        latestCategory = CategoryType.Count;
        categoryStartBusinessDay = -1;
    }

    #endregion

    #region Validation

    // 맛 타입이 실제 선택 가능한 값인지 확인한다.
    private static bool IsValidTaste(TasteType taste)
    {
        return (int)taste >= 0 && (int)taste < (int)TasteType.Count;
    }

    // 카테고리 타입이 실제 선택 가능한 값인지 확인한다.
    private static bool IsValidCategory(CategoryType category)
    {
        return (int)category >= 0 && (int)category < (int)CategoryType.Count;
    }

    #endregion
}
