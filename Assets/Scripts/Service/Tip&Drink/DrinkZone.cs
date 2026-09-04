using UnityEngine;
using UnityEngine.UI;

public class DrinkZone : MonoBehaviour
{
    [SerializeField] private DrinkFillButton drinkFillButton;
    [SerializeField] private Image drinkFillImg;
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Transform drinkSpot;

    private float spendDrinkFillAmount = 1.0f;
    private GroceryAmount groceryAmount = new();

    public Transform DrinkSpot => drinkSpot;

    private void Awake()
    {
        if ((GameManager.Instance.Upgrade.RuntimeLevel.Get(FacilityType.Decor_2) < 1))
            Destroy(this.gameObject);

        drinkFillButton.gameObject.SetActive(false);
        backGroundImage.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        drinkFillButton.Filled += ApplyFill;
        UpdateDrinkSpendUI();
    }

    private void UpdateDrinkSpendUI()
    {
        drinkFillImg.fillAmount = spendDrinkFillAmount;
    }

    public bool CanSpendDrink()
    {
        return spendDrinkFillAmount >= 0.25f;
    }

    public void SpendDrink()
    {
        spendDrinkFillAmount -= 0.25f;
        UpdateDrinkSpendUI();
        if (spendDrinkFillAmount <= 0.0f&&CanFillDrink())
        {
            FillDrink();
        }
    }

    private bool CanFillDrink()
    {
        groceryAmount.grocery = GroceryType.Grape;
        groceryAmount.amount = 1;
        if(GameManager.Instance.StockManager.CanConsumeGrocery(groceryAmount))
        {
            return true;
        }
        return false;
    }

    private void FillDrink()
    {
        groceryAmount.grocery = GroceryType.Grape;
        groceryAmount.amount = 1;
        if (GameManager.Instance.StockManager.TryConsumeGrocery(groceryAmount))
        {
            return;
        }
        drinkFillButton.gameObject.SetActive (true);
        backGroundImage.gameObject.SetActive (true);
    }

    private void ApplyFill()
    {
        spendDrinkFillAmount = 1.0f;
        drinkFillImg.fillAmount = spendDrinkFillAmount;
        backGroundImage.gameObject.SetActive(false);
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_DrinkRefill);
    }

    private void OnDisable()
    {
        drinkFillButton.Filled -= ApplyFill;
    }
}
