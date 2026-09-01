using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Changes the active Hub canvas view when clicked.
/// Each concrete Hub state injects its HubCanvasController during initialization.
/// </summary>
public sealed class UI_HubStateButton : UI_EventHandler
{
    [SerializeField] private HubCanvasController.HubCanvasState targetState =
        HubCanvasController.HubCanvasState.None; 

    private HubCanvasController hubController;
    private Button button;
    private bool isInitialized;

    public HubCanvasController.HubCanvasState TargetState => targetState;

    public void SetTargetState(HubCanvasController.HubCanvasState state)
    {
        targetState = state;
    }

    private void Awake()
    {
        button = GetComponent<Button>();
    }

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
    }

    private void HandleClick(PointerEventData _)
    {
        if (button == null || !button.IsInteractable())
            return;

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
    }
     
}
