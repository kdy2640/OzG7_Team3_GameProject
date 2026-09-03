
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

    private void OnEnable()
    {
        UpdateUI();
    }

    public int ComboCount => comboCount;
    public float BonusRate => bonusRate;    
    public void AddCount()
    {
        comboCount++;

        if (comboCount % 5 == 0)
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_ComboSound);

        UpdateUI();
    }

    public void BreakCombo()
    {
        bool hadCombo = comboCount > 0;
        comboCount = 0;

        if (hadCombo)
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_ComboBreak);

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
        countText.text = comboCount.ToString();
        bonusText.text = "+ " + bonusRate + "%";
    }
}
