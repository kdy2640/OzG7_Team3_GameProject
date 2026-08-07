using TMPro;
using UnityEngine;

public class QueueSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text dishName;

    public void SetDish(DishType dish)
    {
        DishDataSO data = DishDataDB.GetData(dish);

        if(data == null)
        {
            return;
        }

        dishName.text = data.DisplayName;
    }

    public void Clear()
    {
        dishName.text = "";
    }
}
