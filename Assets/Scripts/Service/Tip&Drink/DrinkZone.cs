using UnityEngine;
using UnityEngine.UI;

public class DrinkZone : MonoBehaviour
{
    [SerializeField] private DrinkFillButton drinkFillButton;
    [SerializeField] private Image drinkFillImg;
    [SerializeField] private Transform drinkSpot;

    private float spendDrinkFillAmount = 1.0f;

    public Transform DrinkSpot => drinkSpot;

    private void Awake()
    {
        if ((GameManager.Instance.Upgrade.RuntimeLevel.Get(FacilityType.Decor_2) < 1))
            Destroy(this.gameObject);

        drinkFillButton.gameObject.SetActive(false);
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
        // 재료
        return true;
    }

    private void FillDrink()
    {
        drinkFillButton.gameObject.SetActive (true);
    }

    private void ApplyFill()
    {
        spendDrinkFillAmount = 1.0f;
        drinkFillImg.fillAmount = spendDrinkFillAmount;
    }

    private void OnDisable()
    {
        drinkFillButton.Filled -= ApplyFill;
    }
}
