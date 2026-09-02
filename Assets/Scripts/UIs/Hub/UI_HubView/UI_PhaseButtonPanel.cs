using UnityEngine;
using UnityEngine.UI;

public sealed class UI_PhaseButtonPanel : MonoBehaviour
{
    private const string HarvestButtonName = "UI_ToHarvestButton";
    private const string ServiceSelectionButtonName = "UI_ToServiceSelectionButton";
    private const string NextDayButtonName = "UI_NextDayButton";

    [Header("Phase Highlight Colors")]
    [SerializeField] private Color morningBackgroundColor =
        new Color32(255, 215, 163, 255);
    [SerializeField] private Color morningOutlineColor =
        new Color32(242, 140, 40, 255);
    [SerializeField] private Color afternoonBackgroundColor =
        new Color32(217, 243, 181, 255);
    [SerializeField] private Color afternoonOutlineColor =
        new Color32(79, 157, 69, 255);
    [SerializeField] private Color nightBackgroundColor =
        new Color32(226, 206, 244, 255);
    [SerializeField] private Color nightOutlineColor =
        new Color32(139, 95, 191, 255);

    private HubCanvasController owner;
    private Button harvestButton;
    private Button serviceSelectionButton;
    private Button nextDayButton;
    private Outline harvestButtonOutline;
    private Outline serviceSelectionButtonOutline;
    private Outline nextDayButtonOutline;
    private Color harvestDefaultBackgroundColor;
    private Color harvestDefaultOutlineColor;
    private Color serviceSelectionDefaultBackgroundColor;
    private Color serviceSelectionDefaultOutlineColor;
    private Color nextDayDefaultBackgroundColor;
    private Color nextDayDefaultOutlineColor;
    private bool isInitialized;

    public void Init(HubCanvasController owner)
    {
        if (isInitialized)
            return;
        this.owner = owner;
        BindButtons();

        if (harvestButton == null || serviceSelectionButton == null || nextDayButton == null)
        {
            Debug.LogError($"[{nameof(UI_PhaseButtonPanel)}] Required phase buttons are missing.", this);
            return;
        }

        harvestButtonOutline = harvestButton.GetComponent<Outline>();
        serviceSelectionButtonOutline = serviceSelectionButton.GetComponent<Outline>();
        nextDayButtonOutline = nextDayButton.GetComponent<Outline>();

        harvestDefaultBackgroundColor = harvestButton.image.color;
        harvestDefaultOutlineColor = harvestButtonOutline.effectColor;
        serviceSelectionDefaultBackgroundColor = serviceSelectionButton.image.color;
        serviceSelectionDefaultOutlineColor = serviceSelectionButtonOutline.effectColor;
        nextDayDefaultBackgroundColor = nextDayButton.image.color;
        nextDayDefaultOutlineColor = nextDayButtonOutline.effectColor;

        harvestButton.GetComponent<UI_HubStateButton>()?.Init(owner);
        serviceSelectionButton.GetComponent<UI_HubStateButton>()?.Init(owner);
        nextDayButton.onClick.AddListener(HandleNextDayButtonClicked);

        GameManager.Instance.Market.SubscribeMarketDataChanged(Refresh);
        isInitialized = true;
        Refresh();
    }

    private void OnDestroy()
    {
        if (nextDayButton != null)
            nextDayButton.onClick.RemoveListener(HandleNextDayButtonClicked);

        if (isInitialized && GameManager.Instance != null)
            GameManager.Instance.Market?.UnsubscribeMarketDataChanged(Refresh);
    }

    public void Refresh()
    {
        if (!isInitialized || GameManager.Instance == null || GameManager.Instance.Market == null)
            return;

        MarketPhase currentPhase = GameManager.Instance.Market.MarketData.CurrentPhase;
        bool isMorning = currentPhase == MarketPhase.Morning;
        bool isAfternoon = currentPhase == MarketPhase.Afternoon;
        bool isNight = currentPhase == MarketPhase.Night;

        harvestButton.gameObject.SetActive(true);
        harvestButton.interactable = isMorning;

        serviceSelectionButton.gameObject.SetActive(!isNight);
        serviceSelectionButton.interactable = isAfternoon;

        nextDayButton.gameObject.SetActive(isNight);
        nextDayButton.interactable = isNight;

        harvestButton.image.color = isMorning
            ? morningBackgroundColor
            : harvestDefaultBackgroundColor;
        harvestButtonOutline.effectColor = isMorning
            ? morningOutlineColor
            : harvestDefaultOutlineColor;

        serviceSelectionButton.image.color = isAfternoon
            ? afternoonBackgroundColor
            : serviceSelectionDefaultBackgroundColor;
        serviceSelectionButtonOutline.effectColor = isAfternoon
            ? afternoonOutlineColor
            : serviceSelectionDefaultOutlineColor;

        nextDayButton.image.color = isNight
            ? nightBackgroundColor
            : nextDayDefaultBackgroundColor;
        nextDayButtonOutline.effectColor = isNight
            ? nightOutlineColor
            : nextDayDefaultOutlineColor;
    }

    private void BindButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            switch (buttons[i].name)
            {
                case HarvestButtonName:
                    harvestButton = buttons[i];
                    break;
                case ServiceSelectionButtonName:
                    serviceSelectionButton = buttons[i];
                    break;
                case NextDayButtonName:
                    nextDayButton = buttons[i];
                    break;
            }
        }
    }

    private void HandleNextDayButtonClicked()
    {
        if (GameManager.Instance.Market.MarketData.CurrentPhase != MarketPhase.Night)
            return;

        GameManager.Instance.Market.MoveToNextPhase();
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Hub_NextDay);
        owner.RequestStateChange(HubCanvasController.HubCanvasState.DayStart);
    }
}
