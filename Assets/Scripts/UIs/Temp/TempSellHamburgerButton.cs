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
        if (!DishDataDB.TryGetData(DishType.Hamburger, out DishDataSO data))
            return;

        StockManager stockManager = GameManager.Instance.StockManager;

        if (!stockManager.TryConsumeDish(new DishAmount(DishType.Hamburger, 1)))
            return;

        stockManager.AddCurrency(data.Cost);
    }
}
