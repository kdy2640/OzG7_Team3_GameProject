using System.Collections.Generic;
using UnityEngine;

public class UI_MenuContainer : MonoBehaviour
{
    private List<UI_MenuDevelopCard> cards;

    public void SetCards(List<UI_MenuDevelopCard> cards)
    {
        this.cards = cards;
    }
}
