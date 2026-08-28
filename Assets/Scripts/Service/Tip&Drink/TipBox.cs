using UnityEngine;

public class TipBox : MonoBehaviour
{
    [SerializeField] private int tipAmount;
    [SerializeField] private Transform tipSpot;
    private TipButton tipButton;

    public Transform TipSpot => tipSpot;

    private void OnEnable()
    {
        tipAmount = 0;
        tipButton = GetComponentInChildren<TipButton>();
        tipButton.OnClicked += ApplyTip;
        tipButton.gameObject.SetActive(false);
    }

    public void AddTip(int tip)
    {
        tipAmount += tip;
        GameManager.Instance.Service.ResultBuilder.RecordTip(tip);
        UpdateUI();
    }
    public void ApplyTip()
    {
        GameManager.Instance.StockManager.AddCurrency(tipAmount);
        GameManager.Instance.Service.ResultBuilder.RecordTip(tipAmount);
        tipAmount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(tipAmount <= 0)
        {
            tipButton.gameObject.SetActive(false);
            return;
        }
        else
        {
            if(!tipButton.gameObject.activeSelf)
                tipButton.gameObject.SetActive(true);

            tipButton.SetAmountText(tipAmount);

        }
    }

    private void OnDisable()
    {
        if(tipAmount > 0)
        {
            ApplyTip();
        }
        tipButton.OnClicked -= ApplyTip;
    }
}
