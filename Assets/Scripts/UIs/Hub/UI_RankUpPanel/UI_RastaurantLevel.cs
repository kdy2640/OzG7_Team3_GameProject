using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_RastaurantLevel : MonoBehaviour
{
    private const int SlotCount = 4;

    [SerializeField] private Color inactiveLevelColor = Color.gray;
    [SerializeField] private Color activeLevelColor = Color.green;

    private readonly Image[] levelSlots = new Image[SlotCount];
    private TextMeshProUGUI gradeText;
    private bool areSlotsCached;

    public void Refresh()
    {
        CacheUI();

        MarketManager marketManager = GameManager.Instance?.Market;

        if (marketManager == null)
        {
            Debug.LogError($"[{nameof(UI_RastaurantLevel)}] MarketManager를 찾을 수 없습니다.", this);
            return;
        }

        int currentLevel = marketManager.MarketData.CurrentLevel;

        if (gradeText != null)
            gradeText.text = $"{currentLevel}스타 식당";

        int activeLevelCount = Mathf.Clamp(currentLevel, 0, SlotCount);

        for (int i = 0; i < SlotCount; i++)
        {
            if (levelSlots[i] != null)
            {
                levelSlots[i].color =
                    i < activeLevelCount ? activeLevelColor : inactiveLevelColor;
            }
        }
    }

    private void CacheUI()
    {
        if (areSlotsCached)
            return;

        areSlotsCached = true;
        gradeText = transform.Find("GradeText")?.GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < SlotCount; i++)
        {
            levelSlots[i] = transform
                .Find($"LevelSlotContainer/RestaurantLevelSlot{i + 1:00}")?
                .GetComponent<Image>();
        }
    }
}
