using TMPro;
using UnityEngine;

public class DishSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text amountText;


    public void SetDish(DishAmount dishAmount)
    {
        DishDataSO data = DishDataDB.GetData(dishAmount.dish);

        if (data == null)
        {
            Clear();
            return;
        }
            

        dishName.text = data.DisplayName;
        amountText.text = $"x {dishAmount.amount}";
    }

    public void Clear()
    {
        dishName.text = "";
        amountText.text = "";
    }
}