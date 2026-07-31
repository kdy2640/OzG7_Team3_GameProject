using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Changes the active Hub canvas view when clicked.
/// Each concrete Hub state injects its HubCanvasController during initialization.
/// </summary>
public sealed class UI_HubStateButton : UI_EventHandler
{
    [SerializeField] private HubCanvasController.HubCanvasState targetState =
        HubCanvasController.HubCanvasState.None;
    [SerializeField] private TMP_Text label;

    private HubCanvasController hubController;
    private bool isInitialized;

    public HubCanvasController.HubCanvasState TargetState => targetState;

    public void Init(HubCanvasController controller)
    {
        if (controller == null)
        {
            Debug.LogError($"[{nameof(UI_HubStateButton)}] HubCanvasController is required.", this);
            return;
        }

        if (isInitialized)
        {
            if (hubController != controller)
            {
                Debug.LogError(
                    $"[{nameof(UI_HubStateButton)}] The button was initialized by another controller.",
                    this);
            }

            return;
        }

        hubController = controller;
        isInitialized = true;
        this.AddUIEvent(HandleClick, UIEvent.LClick);
        RefreshLabel();
    }

    private void HandleClick(PointerEventData _)
    {
        if (hubController == null)
        {
            Debug.LogError($"[{nameof(UI_HubStateButton)}] The controller has not been injected.", this);
            return;
        }

        if (targetState == HubCanvasController.HubCanvasState.None)
        {
            Debug.LogWarning($"[{nameof(UI_HubStateButton)}] Target state is None.", this);
            return;
        }

        hubController.RequestStateChange(targetState);
    }

    private void OnValidate()
    {
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (label != null)
        {
            label.text = targetState.ToString();
        }
    }
}
