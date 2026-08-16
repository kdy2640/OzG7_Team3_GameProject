using UnityEngine;
using UnityEngine.UI;

public sealed class UI_PhaseButtonPanel : MonoBehaviour
{
    private const string HarvestButtonName = "UI_ToHarvestButton";
    private const string ServiceSelectionButtonName = "UI_ToServiceSelectionButton";
    private const string NextDayButtonName = "UI_NextDayButton";

    private HubCanvasController owner;
    private Button harvestButton;
    private Button serviceSelectionButton;
    private Button nextDayButton;
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
        owner.RequestStateChange(HubCanvasController.HubCanvasState.DayStart);
    }
}
