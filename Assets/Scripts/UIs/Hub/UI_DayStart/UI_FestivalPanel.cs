using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_FestivalPanel : MonoBehaviour
{
    private readonly List<UI_FestivalElement> categoryElements = new();
    private readonly List<UI_FestivalElement> tasteElements = new();

    private UI_DishDetailPanel dishDetailPanel;
    private Button festivalStartButton;
    private UI_FestivalElement selectedElement;
    private bool isInitialized;

    public void Init(UI_DishDetailPanel dishDetailPanel, Button festivalStartButton)
    {
        if (isInitialized)
            return;

        if (dishDetailPanel == null || festivalStartButton == null)
        {
            Debug.LogError($"[{nameof(UI_FestivalPanel)}] Dish Detail Panel 또는 Festival Start Button이 필요합니다.", this);
            return;
        }

        Transform categoryGrid = transform.Find(
            "UI_FestivalContainer/UI_CategoryFestivalListPanel/Grid");
        Transform tasteGrid = transform.Find(
            "UI_FestivalContainer/UI_TasteFestivalListPanel/Grid");

        if (categoryGrid == null || tasteGrid == null)
        {
            Debug.LogError($"[{nameof(UI_FestivalPanel)}] Festival Element Grid를 찾을 수 없습니다.", this);
            return;
        }

        categoryGrid.GetComponentsInChildren(true, categoryElements);
        tasteGrid.GetComponentsInChildren(true, tasteElements);

        if (categoryElements.Count != (int)CategoryType.Count
            || tasteElements.Count != (int)TasteType.Count)
        {
            Debug.LogError($"[{nameof(UI_FestivalPanel)}] Festival Element 개수가 Enum 개수와 일치하지 않습니다.", this);
            return;
        }

        this.dishDetailPanel = dishDetailPanel;
        this.festivalStartButton = festivalStartButton;

        for (int i = 0; i < categoryElements.Count; i++)
            categoryElements[i].SetCategory((CategoryType)i, HandleElementSelected);

        for (int i = 0; i < tasteElements.Count; i++)
            tasteElements[i].SetTaste((TasteType)i, HandleElementSelected);

        festivalStartButton.onClick.AddListener(HandleFestivalStartClicked);
        festivalStartButton.interactable = false;
        isInitialized = true;
    }

    private void OnDestroy()
    {
        if (festivalStartButton != null)
            festivalStartButton.onClick.RemoveListener(HandleFestivalStartClicked);
    }

    public void Refresh()
    {
        if (!isInitialized)
            return;

        for (int i = 0; i < categoryElements.Count; i++)
            categoryElements[i].Refresh();

        for (int i = 0; i < tasteElements.Count; i++)
            tasteElements[i].Refresh();

        RefreshStartButton();
    }

    private void HandleElementSelected(UI_FestivalElement element)
    {
        selectedElement = element;

        if (element.IsTaste)
            dishDetailPanel.Refresh((TasteType)element.NowEnum);
        else
            dishDetailPanel.Refresh((CategoryType)element.NowEnum);

        RefreshStartButton();
    }

    private void HandleFestivalStartClicked()
    {
        if (selectedElement == null
            || selectedElement.State != FestivalElementState.Selectable
            || GameManager.Instance?.Market == null)
        {
            return;
        }

        bool didStart = selectedElement.IsTaste
            ? GameManager.Instance.Market.TryStartTasteFestival((TasteType)selectedElement.NowEnum)
            : GameManager.Instance.Market.TryStartCategoryFestival((CategoryType)selectedElement.NowEnum);

        if (didStart)
            Refresh();
    }

    private void RefreshStartButton()
    {
        festivalStartButton.interactable = selectedElement != null
            && selectedElement.State == FestivalElementState.Selectable;
    }
}
