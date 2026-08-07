using System.Collections.Generic;
using UnityEngine;

public class UI_SelectedMenuPanel : MonoBehaviour
{
    [SerializeField] private List<UI_SelectedMenuCard> cards = new();

    private void Start()
    {
        GameManager.Instance.Market.SubscribeMarketDataChanged(Refresh);
    }
    private void OnDestroy()
    {
        GameManager.Instance.Market.UnsubscribeMarketDataChanged(Refresh);
    }
    public void Refresh()
    {
        int maxDish = GameManager.Instance.Market.LevelData.MaxDishLimit;
        IReadOnlyList<DishType> selectedDish = GameManager.Instance.Market.Data.SelectedDishes;
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetData(i < selectedDish.Count ? selectedDish[i] : DishType.Count);
            cards[i].SetCover(i >= maxDish);
        }
    }

}
