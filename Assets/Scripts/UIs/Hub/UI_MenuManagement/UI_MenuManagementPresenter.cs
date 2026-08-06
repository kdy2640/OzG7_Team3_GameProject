using System.Collections.Generic;
using UnityEngine;

public sealed class UI_MenuManagementPresenter : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private UI_SelectedMenuCard[] selectedMenuCards;
    [SerializeField] private UI_MenuDevelopCard[] developMenuCards;

    [Header("Detail")]
    [SerializeField] private UI_MenuManagementDisplay detailPanel;

    public void Initialize(
        List<MenuCardData> menuCards,
        List<MenuDevelopDetailData> menuDetails,
        List<int> todayMenuIndices)
    {
        UpdateTodayMenus(menuCards, menuDetails, todayMenuIndices);
        UpdateDevelopMenus(menuCards, menuDetails);

        if (menuDetails != null && menuDetails.Count > 0)
        {
            detailPanel.SetData(menuDetails[0]);
        }
    }

    private void UpdateTodayMenus(
        List<MenuCardData> menuCards,
        List<MenuDevelopDetailData> menuDetails,
        List<int> todayMenuIndices)
    {
        for (int i = 0; i < selectedMenuCards.Length; i++)
        {
            if (todayMenuIndices != null &&
                i < todayMenuIndices.Count)
            {
                int menuIndex = todayMenuIndices[i];

                selectedMenuCards[i].gameObject.SetActive(true);

                selectedMenuCards[i].SetData(menuCards[menuIndex]);

                selectedMenuCards[i].Bind(() =>
                {
                    detailPanel.SetData(menuDetails[menuIndex]);
                });
            }
            else
            {
                selectedMenuCards[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateDevelopMenus(
        List<MenuCardData> menuCards,
        List<MenuDevelopDetailData> menuDetails)
    {
        for (int i = 0; i < developMenuCards.Length; i++)
        {
            if (menuCards != null &&
                i < menuCards.Count)
            {
                int menuIndex = i;

                developMenuCards[i].gameObject.SetActive(true);

                developMenuCards[i].SetData(menuCards[menuIndex]);

                developMenuCards[i].Bind(() =>
                {
                    detailPanel.SetData(menuDetails[menuIndex]);
                });
            }
            else
            {
                developMenuCards[i].gameObject.SetActive(false);
            }
        }
    }
}