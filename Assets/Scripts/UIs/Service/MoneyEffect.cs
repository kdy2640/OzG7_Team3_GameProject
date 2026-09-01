
using TMPro;
using UnityEngine;

public class MoneyEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    private float timer;

    private void OnEnable()
    {
        timer = 2.0f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SetAmount(int amount)
    {
        amountText.text = "+ " + amount.ToString() + "!";
    }
}
