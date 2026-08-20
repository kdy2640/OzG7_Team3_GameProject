using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResultGroceryItem : MonoBehaviour
{
    [FormerlySerializedAs("oreIcon")]
    [SerializeField] private Image groceryIcon;

    [FormerlySerializedAs("oreNameText")]
    [SerializeField] private TMP_Text groceryNameText;

    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text multiplierText;

    private void Awake()
    {
        if (groceryIcon != null)
            groceryIcon.gameObject.SetActive(false);

        if (multiplierText != null)
            multiplierText.gameObject.SetActive(false);
    }

    public void SetData(GroceryAmount groceryAmount)
    {
        if (groceryAmount == null)
            return;

        if (groceryNameText != null)
        {
            groceryNameText.text = GroceryDataDB.TryGetData(
                groceryAmount.grocery,
                out GroceryDataSO data)
                ? data.DisplayName
                : groceryAmount.grocery.ToString();
        }

        SetAmount(groceryAmount.amount);
    }

    public void SetAmount(int amount)
    {
        if (amountText != null)
            amountText.text = $"x{Mathf.Max(0, amount)}";
    }
}
