using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 허브 화면 UI 표시 전용
public sealed class UI_HubDisplay : MonoBehaviour
{
    [Header("Top Resource")]
    [SerializeField] private TMP_Text goldText;

    [Header("Center Effect")]
    [SerializeField] private TMP_Text currentEffectText;

    [Header("Player Information")]
    [SerializeField] private Image[] levelImages = new Image[4];

    [SerializeField] private Sprite levelInactiveSprite;

    [SerializeField] private Sprite levelActiveSprite;

    [SerializeField] private TMP_Text playerNameText;

    [Header("Sales Progress")]
    [SerializeField] private Image salesIcon;

    [SerializeField] private TMP_Text salesTitleText;

    [SerializeField] private Slider salesSlider;

    [SerializeField] private TMP_Text salesProgressText;

    [Header("Promotion Quest")]
    [SerializeField] private TMP_Text promotionQuestTitleText;

    [SerializeField] private TMP_Text promotionQuestDescriptionText;

    [SerializeField] private Image[] promotionStepImages = new Image[5];

    [SerializeField] private Sprite missionInactiveSprite;

    [SerializeField] private Sprite missionActiveSprite;

    private void Awake()
    {
        if (salesTitleText != null) salesTitleText.text = "Total Sales";
    }

    public void SetData(HubDisplayData data)
    {
        if (data == null) return;

        goldText.text = data.Gold.ToString("N0");

        currentEffectText.text = data.CurrentEffect;

        playerNameText.text = data.PlayerName;

        promotionQuestTitleText.text = data.PromotionQuestTitle;

        promotionQuestDescriptionText.text = data.PromotionQuestDescription;

        RefreshLevel(data.PlayerLevel);

        RefreshSales(data.CurrentSales, data.TargetSales);

        RefreshPromotionStep(data.PromotionStep);
    }

    private void RefreshLevel(int level)
    {
        level = Mathf.Clamp(level, 1, levelImages.Length);

        for (int i = 0; i < levelImages.Length; i++)
        {
            if (levelImages[i] == null) continue;

            levelImages[i].sprite =
                (i < level) ? levelActiveSprite : levelInactiveSprite;
        }
    }
    private void RefreshSales(int currentSales, int targetSales)
    {
        targetSales = Mathf.Max(targetSales, 1);

        float progress =
            Mathf.Clamp01((float)currentSales / targetSales);

        if (salesSlider != null)
        {
            salesSlider.value = progress;
        }

        if (salesProgressText != null)
        {
            salesProgressText.text = $"{currentSales:N0} / {targetSales:N0}";
        }
    }
    private void RefreshPromotionStep(int step)
    {
        step = Mathf.Clamp(step, 0, promotionStepImages.Length);

        for (int i = 0; i < promotionStepImages.Length; i++)
        {
            if (promotionStepImages[i] == null) continue;

            promotionStepImages[i].sprite =
                (i < step) ? missionActiveSprite : missionInactiveSprite;
        }
    }

}