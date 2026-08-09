using System.Collections.Generic;
using UnityEngine;

public class UI_MenuContainer : MonoBehaviour
{
    private List<UI_MenuVisualCard> cards;

    public void SetCards(List<UI_MenuVisualCard> cards)
    {
        this.cards = cards;
    }
}
