using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_FacilityLevelDisplay : MonoBehaviour
{
    public enum ColorMode
    {
        CurrentLevel,
        NextLevel
    }

    [SerializeField] private ColorMode colorMode;

    [Header("UI")]
    [SerializeField] private Image background;
    [SerializeField] private Image innerBackground;
    [SerializeField] private Image effectState;
    [SerializeField] private Image[] levelSlots = new Image[5];
    [SerializeField] private Outline[] levelSlotOutlines;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text currentEffectText;

    [Header("Level Slot Colors")]
    [SerializeField] private Color filledSlotColor =
        new(.68235296f, .8235294f, .43137255f, 1f);
    [SerializeField] private Color emptySlotColor =
        new(.7372549f, .7372549f, .7372549f, 1f);

    [Header("Current Level Colors")]
    [SerializeField] private Color currentBackgroundColor =
        new(.46f, .58f, .22f, 1f);
    [SerializeField] private Color currentInnerBackgroundColor =
        new(1f, .98f, .906f, 1f);
    [SerializeField] private Color currentEffectStateColor =
        new(.682f, .824f, .431f, 1f);
    [SerializeField] private Color currentSlotOutlineColor =
        new(.68235296f, .8235294f, .43137255f, 1f);

    [Header("Next Level Colors")]
    [SerializeField] private Color nextBackgroundColor =
        new(.91f, .47f, .08f, 1f);
    [SerializeField] private Color nextInnerBackgroundColor =
        new(1f, .788f, .459f, 1f);
    [SerializeField] private Color nextEffectStateColor =
        new(.91f, .47f, .08f, 1f);
    [SerializeField] private Color nextSlotOutlineColor = Color.white;

    private bool isMaxLevel;

    private void Awake()
    {
        ApplyColorMode();
    }

    private void OnValidate()
    {
        ApplyColorMode();
    }

    public void SetColorMode(ColorMode mode)
    {
        colorMode = mode;
        ApplyColorMode();
    }

    public void SetData(int level, string description)
    {
        levelText.text = $"Lv.{level}";
        descriptionText.text = description;

        for (int i = 0; i < levelSlots.Length; i++)
        {
            levelSlots[i].color = i < level
                ? filledSlotColor
                : emptySlotColor;
        }
    }

    public void SetMaxLevel(bool value)
    {
        isMaxLevel = value;
        ApplyEffectStateText();
    }

    public void SetInvalidData()
    {
        isMaxLevel = false;
        levelText.text = "Lv.-";
        descriptionText.text = "Data Error";
        ApplyEffectStateText();
    }

    private void ApplyColorMode()
    {
        bool isCurrentLevel = colorMode == ColorMode.CurrentLevel;

        background.color = isCurrentLevel
            ? currentBackgroundColor
            : nextBackgroundColor;
        innerBackground.color = isCurrentLevel
            ? currentInnerBackgroundColor
            : nextInnerBackgroundColor;
        effectState.color = isCurrentLevel
            ? currentEffectStateColor
            : nextEffectStateColor;
        ApplyEffectStateText();

        Color outlineColor = isCurrentLevel
            ? currentSlotOutlineColor
            : nextSlotOutlineColor;

        for (int i = 0; i < levelSlotOutlines.Length; i++)
            levelSlotOutlines[i].effectColor = outlineColor;
    }

    private void ApplyEffectStateText()
    {
        if (isMaxLevel)
        {
            currentEffectText.text = "만렙";
            return;
        }

        currentEffectText.text = colorMode == ColorMode.CurrentLevel
            ? "현재 효과"
            : "다음 효과";
    }
}
