using TMPro;
using UnityEngine;

public class KitchenSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text timerText;

    public void SetCooking(DishType dish, float time)
    {
        DishDataSO data = DishDataDB.GetData(dish);

        dishName.text = data.DisplayName;
        timerText.text = time.ToString("F0") + "s";
    }

    public void Clear()
    {
        dishName.text = "";
        timerText.text = "";
    }
}
