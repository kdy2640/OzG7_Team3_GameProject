using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TempSellHamburgerButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        StockManager stockManager = GameManager.Instance.StockManager;

        if (!stockManager.TryConsumeDish(new DishAmount(DishType.CarrotSalad, 1)))
            return;

        stockManager.AddCurrency(
            DishPriceCalculator.BasicPriceCalculate(DishType.CarrotSalad));
    }
}
