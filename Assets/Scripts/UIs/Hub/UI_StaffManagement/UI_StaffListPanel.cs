using System;
using System.Collections.Generic;
using UnityEngine;

// StaffListPanel에 붙입니다. 고정 카드의 활성화, 표시, 선택 이벤트만 관리합니다.
public sealed class UI_StaffListPanel : MonoBehaviour
{
    [SerializeField] private UI_StaffDevelopCard[] staffCards;

    private void Awake() => HideAllCards();

    public void Initialize(Action<EmployeeType> onCardSelected)
    {
        foreach (UI_StaffDevelopCard card in staffCards)
            card.Initialize(onCardSelected);
    }

    public void ShowCards(List<StaffCardUIData> dataList)
    {
        var map = new Dictionary<EmployeeType, StaffCardUIData>();
        foreach (StaffCardUIData data in dataList) map[data.type] = data;

        foreach (UI_StaffDevelopCard card in staffCards)
        {
            bool exists = map.TryGetValue(card.EmployeeType, out StaffCardUIData data);
            card.gameObject.SetActive(exists);
            if (exists) card.SetView(data);
        }
    }

    public void HideAllCards()
    {
        foreach (UI_StaffDevelopCard card in staffCards)
            card.gameObject.SetActive(false);
    }
}
