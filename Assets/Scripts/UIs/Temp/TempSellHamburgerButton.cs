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
        if (!DishDataDB.TryGetData(DishType.MeatOnigiri, out DishDataSO data))
            return;

        StockManager stockManager = GameManager.Instance.StockManager;

        if (!stockManager.TryConsumeDish(new DishAmount(DishType.MeatOnigiri, 1)))
            return;

        stockManager.AddCurrency(data.Cost);
    }
}
