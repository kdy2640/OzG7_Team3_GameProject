using TMPro; 
using UnityEngine;

public class UI_GoldElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldText;
    void Start()
    {
        GameManager.Instance.StockManager.SubscribeStockDataChange(UpdateGold);
        UpdateGold();
    }
     
    void OnDestroy()
    {
        GameManager.Instance.StockManager.UnsubscribeStockDataChange(UpdateGold); 
    }

    private void UpdateGold()
    {
        goldText.text = GameManager.Instance.StockManager.StockData.Currency.ToString();
    }

}
