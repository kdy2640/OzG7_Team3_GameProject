using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_RastaurantLevel : MonoBehaviour
{
    private const int SlotCount = 4;

    [SerializeField] private Image[] levelSlots = new Image[SlotCount];
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI descriptionShadowText;
    [SerializeField] private Color inactiveLevelColor = Color.gray;
    [SerializeField] private Color activeLevelColor = Color.green;

    public void Refresh()
    {
        MarketManager marketManager = GameManager.Instance.Market;

        int currentLevel = marketManager.MarketData.CurrentLevel;
        int activeLevelCount = Mathf.Clamp(currentLevel, 0, SlotCount);

        for (int i = 0; i < SlotCount; i++)
        {
            levelSlots[i].color =
                i < activeLevelCount ? activeLevelColor : inactiveLevelColor;
        }

        string description = marketManager.LevelMissionProgress
            .MissionGroup.MarketDescription;
        descriptionText.text = description;
        descriptionShadowText.text = description;
    }
}
