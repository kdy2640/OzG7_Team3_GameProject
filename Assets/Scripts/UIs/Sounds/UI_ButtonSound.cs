using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UI_EventHandler))]
public class UI_ButtonSound : MonoBehaviour
{
    UI_EventHandler eventHandler;

    private void Awake()
    {
        eventHandler = GetComponent<UI_EventHandler>();
    }
    private void OnEnable()
    {
        eventHandler.AddUIEvent(OnClick, UI_EventHandler.UIEvent.LClick);
    }
    private void OnDisable()
    {
        eventHandler.RemoveUIEvent(OnClick, UI_EventHandler.UIEvent.LClick);
    }

    private void OnClick(PointerEventData data)
    {

    }
    private void OnHover(PointerEventData data)
    {

    } 
}
