using System.Collections.Generic;
using UnityEngine;

public sealed class UI_DishDetailPanel : MonoBehaviour
{
    [SerializeField] private Transform dishIconContainer;
    [SerializeField] private UI_DishIcon dishIconPrefab;

    private readonly List<UI_DishIcon> dishIcons = new();
    private bool isInitialized;

    private void Awake()
    {
        InitializeIcons();
    }

    public void Refresh(TasteType tasteType)
    {
        if (!InitializeIcons())
            return;

        int visibleIconCount = 0;

        for (int i = 0; i < (int)DishType.Count; i++)
        {
            DishType dishType = (DishType)i;

            if (!DishDataDB.TryGetData(dishType, out DishDataSO dishData)
                || dishData.Tastes != tasteType)
            {
                continue;
            }

            SetIcon(visibleIconCount, dishType);
            visibleIconCount++;
        }

        HideUnusedIcons(visibleIconCount);
    }

    public void Refresh(CategoryType categoryType)
    {
        if (!InitializeIcons())
            return;

        int visibleIconCount = 0;

        for (int i = 0; i < (int)DishType.Count; i++)
        {
            DishType dishType = (DishType)i;

            if (!DishDataDB.TryGetData(dishType, out DishDataSO dishData)
                || dishData.Category != categoryType)
            {
                continue;
            }

            SetIcon(visibleIconCount, dishType);
            visibleIconCount++;
        }

        HideUnusedIcons(visibleIconCount);
    }

    private bool InitializeIcons()
    {
        if (isInitialized)
            return true;

        if (dishIconContainer == null || dishIconPrefab == null)
        {
            Debug.LogError($"[{nameof(UI_DishDetailPanel)}] 아이콘 컨테이너 또는 프리팹이 연결되지 않았습니다.", this);
            return false;
        }

        dishIconContainer.GetComponentsInChildren(true, dishIcons);

        for (int i = 0; i < dishIcons.Count; i++)
            dishIcons[i].gameObject.SetActive(false);

        isInitialized = true;
        return true;
    }

    private void SetIcon(int index, DishType dishType)
    {
        while (dishIcons.Count <= index)
            dishIcons.Add(Instantiate(dishIconPrefab, dishIconContainer));

        UI_DishIcon dishIcon = dishIcons[index];
        dishIcon.gameObject.SetActive(true);
        dishIcon.SetData(dishType);
    }

    private void HideUnusedIcons(int visibleIconCount)
    {
        for (int i = visibleIconCount; i < dishIcons.Count; i++)
            dishIcons[i].gameObject.SetActive(false);
    }
}
