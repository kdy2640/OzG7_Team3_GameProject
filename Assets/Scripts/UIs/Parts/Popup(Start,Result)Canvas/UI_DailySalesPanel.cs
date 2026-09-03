using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DailySalesPanel : MonoBehaviour
{
    [Header("Today's Performance")]
    [SerializeField] private TMP_Text todaySalesText;
    [SerializeField] private TMP_Text customerCountText;

    [Header("Sales Difference")]
    [SerializeField] private TMP_Text salesDifferenceText;
    [SerializeField] private GameObject increaseIcon;
    [SerializeField] private GameObject decreaseIcon;
    [SerializeField] private Color decreasedSalesColor =
        new(0.9607843f, 0.5294118f, 0.5294118f, 1f);

    [Header("Menu Sales")]
    [SerializeField] private UI_MenuSalesRow[] menuSalesRows = new UI_MenuSalesRow[4];
    [SerializeField] private UI_DishIcon[] menuDishIcons = new UI_DishIcon[4];

    [Header("Tip")]
    [SerializeField] private TMP_Text tipSalesText;

    [Header("Market Progress")]
    [SerializeField] private Image[] restaurantLevelSlots = new Image[4];
    [SerializeField] private Color activeLevelColor = Color.white;
    [SerializeField] private Color inactiveLevelColor = Color.gray;
    [SerializeField] private Slider totalIncomeSlider;
    [SerializeField] private TMP_Text totalIncomeText;
    [SerializeField] private Image[] totalIncomeCheckIcons = new Image[2];

    [Header("Promotion Mission")]
    [SerializeField] private Image[] missionSlots = new Image[5];
    [SerializeField] private Sprite completedMissionSprite;
    [SerializeField] private Sprite currentMissionSprite;
    [SerializeField] private Sprite remainingMissionSprite;
    [SerializeField] private RectTransform currentMissionIndicator;
    [SerializeField] private float currentMissionSlotOffsetY = 15f;
    [SerializeField] private Image[] missionCheckIcons = new Image[2];

    [Header("Cheer")]
    [SerializeField] private TMP_Text cheerText;
    [SerializeField] private Color completedCheckColor = new(0.898f, 0.694f, 0.09f, 1f);
    [SerializeField] private Color incompleteCheckColor = new(0.819f, 0.819f, 0.819f, 1f);
    [SerializeField] private Color promotionIncompleteCheckColor = Color.white;

    [Header("Exit")]
    [SerializeField] private Button exitButton;

    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, Min(0f)] private float fadeDuration = 0.25f;

    private SalesResultData data;
    private Color defaultSalesDifferenceColor;

    public bool IsExitRequested { get; private set; }

    private void Awake()
    {
        defaultSalesDifferenceColor = salesDifferenceText.color;

        if (canvasGroup == null)
        {
            Debug.LogError("[UI_DailySalesPanel] CanvasGroup이 연결되지 않았습니다.", this);
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    public void SetData(SalesResultData data)
    {
        if (data == null)
            return;

        this.data = data;
        Refresh();
    }

    public void Refresh()
    {
        if (data == null)
            return;

        if (todaySalesText != null)
            todaySalesText.text = $"{data.todaySales:N0}";

        if (customerCountText != null)
        {
            int totalCustomerCount = Mathf.RoundToInt(
                GameManager.Instance.Upgrade.RuntimeStat.Service
                    .Get(ServiceStatType.CustomerCount));

            customerCountText.text =
                $"{data.customerReceived:N0} / {totalCustomerCount:N0}";
        }

        int difference = data.todaySales - data.yesterdaySales;

        if (salesDifferenceText != null)
        {
            salesDifferenceText.text =
                difference > 0 ? $"+{difference:N0}" : $"{difference:N0}";
            salesDifferenceText.color = difference < 0
                ? decreasedSalesColor
                : defaultSalesDifferenceColor;
        }

        if (increaseIcon != null)
            increaseIcon.SetActive(difference > 0);

        if (decreaseIcon != null)
            decreaseIcon.SetActive(difference < 0);

        RefreshMenuSales();

        if (tipSalesText != null)
            tipSalesText.text = $"{data.tipSales:N0}";

        RefreshPromotionProgress();
    }

    public IEnumerator Show()
    {
        if (canvasGroup == null)
            yield break;

        IsExitRequested = false;
        gameObject.SetActive(true);
        Refresh();

        if (exitButton != null)
            exitButton.interactable = true;

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return canvasGroup
            .DOFade(1f, fadeDuration)
            .WaitForCompletion();
    }

    public IEnumerator Hide()
    {
        if (canvasGroup == null || !gameObject.activeSelf)
            yield break;

        if (exitButton != null)
            exitButton.interactable = false;

        canvasGroup.DOKill();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return canvasGroup
            .DOFade(0f, fadeDuration)
            .WaitForCompletion();

        gameObject.SetActive(false);
    }

    public void RequestExit()
    {
        IsExitRequested = true;

        if (exitButton != null)
            exitButton.interactable = false;
    }

    private void RefreshMenuSales()
    {
        for (int i = 0; i < menuSalesRows.Length; i++)
        {
            UI_MenuSalesRow row = menuSalesRows[i];

            if (row == null)
                continue;

            if (data.menuSales != null && i < data.menuSales.Count)
            {
                menuDishIcons[i].gameObject.SetActive(true);
                menuDishIcons[i].SetData(data.menuSales[i].dishType);
                row.SetData(data.menuSales[i]);
            }
            else
            {
                menuDishIcons[i].gameObject.SetActive(false);
                row.Clear();
            }
        }
    }

    private void RefreshPromotionProgress()
    {
        MarketManager market = GameManager.Instance.Market;
        MarketData marketData = market.MarketData;
        LevelData levelData = market.LevelData;
        LevelMissionProgress missionProgress = market.LevelMissionProgress;

        int activeLevelCount = Mathf.Clamp(
            marketData.CurrentLevel,
            0,
            restaurantLevelSlots.Length);

        for (int i = 0; i < restaurantLevelSlots.Length; i++)
            restaurantLevelSlots[i].color =
                i < activeLevelCount ? activeLevelColor : inactiveLevelColor;

        int incomeGoal = Mathf.Max(1, levelData.IncomeGoal);
        int displayedIncome = Mathf.Clamp(marketData.TotalIncome, 0, incomeGoal);
        bool isIncomeGoalCompleted =
            levelData.IncomeGoal > 0
            && marketData.TotalIncome >= levelData.IncomeGoal;

        totalIncomeSlider.minValue = 0f;
        totalIncomeSlider.maxValue = incomeGoal;
        totalIncomeSlider.value = displayedIncome;
        totalIncomeText.text = $"{displayedIncome:N0} / {incomeGoal:N0}";

        for (int i = 0; i < totalIncomeCheckIcons.Length; i++)
        {
            totalIncomeCheckIcons[i].color = isIncomeGoalCompleted
                ? completedCheckColor
                : i == 0
                    ? incompleteCheckColor
                    : promotionIncompleteCheckColor;
        }

        LevelMissionGroupSO missionGroup = missionProgress.MissionGroup;
        int missionCount = Mathf.Clamp(
            missionGroup.Missions.Count,
            0,
            missionSlots.Length);
        int currentStage = Mathf.Clamp(
            missionProgress.CurrentStage,
            0,
            missionCount);

        for (int i = 0; i < missionSlots.Length; i++)
        {
            Image missionSlot = missionSlots[i];
            bool isVisible = i < missionCount;
            missionSlot.gameObject.SetActive(isVisible);

            if (!isVisible)
                continue;

            if (i < currentStage)
                missionSlot.sprite = completedMissionSprite;
            else if (i == currentStage)
                missionSlot.sprite = currentMissionSprite;
            else
                missionSlot.sprite = remainingMissionSprite;

            Vector2 slotPosition = missionSlot.rectTransform.anchoredPosition;
            slotPosition.y = i == currentStage ? currentMissionSlotOffsetY : 0f;
            missionSlot.rectTransform.anchoredPosition = slotPosition;
        }

        bool hasCurrentMission = currentStage < missionCount;
        currentMissionIndicator.gameObject.SetActive(hasCurrentMission);

        if (hasCurrentMission)
        {
            RectTransform currentSlot = missionSlots[currentStage].rectTransform;
            Vector3 indicatorPosition = currentMissionIndicator.position;
            indicatorPosition.x = currentSlot.TransformPoint(currentSlot.rect.center).x;
            currentMissionIndicator.position = indicatorPosition;
        }

        bool areMissionsCompleted = missionProgress.AreAllMissionsClaimed;

        for (int i = 0; i < missionCheckIcons.Length; i++)
        {
            missionCheckIcons[i].color = areMissionsCompleted
                ? completedCheckColor
                : i == 0
                    ? incompleteCheckColor
                    : promotionIncompleteCheckColor;
        }

        cheerText.text = market.CanPromote
            ? "승급할 수 있어요, 점장님!\n매장 등급을 올려봐요!"
            : "정말 훌륭해요, 점장님!\n다음 등급까지 힘내요!";
    }
}
