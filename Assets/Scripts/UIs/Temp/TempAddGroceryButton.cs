using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TempAddGroceryButton : MonoBehaviour
{
    [SerializeField] private GroceryType groceryType;
    [SerializeField, Min(1)] private int amount = 1;

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
        GameManager.Instance.StockManager.AddGrocery(
            new GroceryAmount(groceryType, amount));
    }
}
