
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    [SerializeField] private Image backgroundImg;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text bonusText;

    private int comboCount = 0;
    private float bonusRate;

    public int ComboCount => comboCount;
    public void AddCount()
    {
        comboCount++;
        UpdateUI();
    }

    public void BreakCombo()
    {
        comboCount = 0;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if(comboCount <= 0)
        {
            backgroundImg.gameObject.SetActive(false);
        }
        if(comboCount > 0 )
        {
            backgroundImg.gameObject.SetActive(true);
        }
        bonusRate = (comboCount / 5) * 10f;
        countText.text = "combo : " + comboCount.ToString();
        bonusText.text = "+ bonus : " + bonusRate + "%";
    }
}
