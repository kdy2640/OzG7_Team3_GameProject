using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(UI_EventHandler))]
public sealed class UI_EscapeClickButton : MonoBehaviour
{
    private Button button;
    private int enabledFrame;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        enabledFrame = Time.frameCount;
    }

    private void Update()
    {
        if (Keyboard.current == null
            || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (Time.frameCount == enabledFrame)
            return;

        Transform activePopupTransform = GetActivePopupTransform();

        if (activePopupTransform != null
            && !transform.IsChildOf(activePopupTransform))
            return;

        if (!button.IsInteractable())
            return;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left
        };

        ExecuteEvents.Execute(
            gameObject,
            eventData,
            ExecuteEvents.pointerClickHandler);
    }

    private static Transform GetActivePopupTransform()
    {
        TutorialPopup activeTutorialPopup =
            FindFirstObjectByType<TutorialPopup>();

        if (activeTutorialPopup != null)
            return activeTutorialPopup.transform;

        SettingsPopup activeSettingsPopup =
            FindFirstObjectByType<SettingsPopup>();

        if (activeSettingsPopup != null)
            return activeSettingsPopup.transform;

        UI_MenuUpgradePanel activeMenuUpgradePanel =
            FindFirstObjectByType<UI_MenuUpgradePanel>();

        if (activeMenuUpgradePanel != null)
            return activeMenuUpgradePanel.transform;

        return null;
    }
}
