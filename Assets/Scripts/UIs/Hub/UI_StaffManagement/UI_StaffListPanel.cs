using System;
using UnityEngine;

// 고정 배치된 직원 카드의 활성화, 갱신, 선택 전달을 담당합니다.
public sealed class UI_StaffListPanel : MonoBehaviour
{
    [SerializeField] private UI_StaffDevelopCard[] staffCards;

    private void Awake()
    {
        HideAllCards();
    }

    public void Initialize(Action<EmployeeType> onCardSelected)
    {
        foreach (UI_StaffDevelopCard card in staffCards)
        {
            if (card == null) continue;

            card.Initialize(onCardSelected);
        }
    }

    public void ShowCards()
    {
        foreach (UI_StaffDevelopCard card in staffCards)
        {
            if (card == null) continue;

            // 비활성 상태에서도 Refresh를 호출할 수 있도록 우선 활성화합니다.
            card.gameObject.SetActive(true);

            bool hasEmployeeData = card.Refresh();
            card.gameObject.SetActive(hasEmployeeData);
        }
    }

    // 모집/강화 이후 카드 상태만 다시 반영할 때 사용합니다.
    public void RefreshCards()
    {
        foreach (UI_StaffDevelopCard card in staffCards)
        {
            if (card == null || !card.gameObject.activeSelf) continue;

            card.Refresh();
        }
    }

    public void HideAllCards()
    {
        foreach (UI_StaffDevelopCard card in staffCards)
        {
            if (card != null) card.gameObject.SetActive(false);
        }
    }
}