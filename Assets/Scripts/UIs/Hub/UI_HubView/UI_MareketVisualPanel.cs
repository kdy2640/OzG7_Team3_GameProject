using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_MareketVisualPanel : MonoBehaviour
{
    [SerializeField] private Image levelSlot01;
    [SerializeField] private Image levelSlot02;
    [SerializeField] private Image levelSlot03;
    [SerializeField] private Image levelSlot04;
    [SerializeField] private Slider salesSlider;
    [SerializeField] private TMP_Text salesAmountText;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color activeColor = Color.green;

    public void Refresh()
    {
        if (GameManager.Instance == null || GameManager.Instance.Market == null)
            return;

        MarketData marketData = GameManager.Instance.Market.MarketData;
        LevelData levelData = GameManager.Instance.Market.LevelData;

        int activeLevelCount = Mathf.Clamp(marketData.CurrentLevel + 1, 0, 4);
        SetLevelSlot(levelSlot01, activeLevelCount >= 1);
        SetLevelSlot(levelSlot02, activeLevelCount >= 2);
        SetLevelSlot(levelSlot03, activeLevelCount >= 3);
        SetLevelSlot(levelSlot04, activeLevelCount >= 4);

        int incomeGoal = Mathf.Max(1, levelData.IncomeGoal);
        int totalIncome = Mathf.Clamp(marketData.TotalIncome, 0, incomeGoal);

        if (salesSlider != null)
        {
            salesSlider.minValue = 0f;
            salesSlider.maxValue = incomeGoal;
            salesSlider.value = totalIncome;
        }

        if (salesAmountText != null)
            salesAmountText.text = $"{totalIncome:N0} / {incomeGoal:N0}";
    }

    private void SetLevelSlot(Image levelSlot, bool isActive)
    {
        if (levelSlot != null)
            levelSlot.color = isActive ? activeColor : inactiveColor;
    }
}
