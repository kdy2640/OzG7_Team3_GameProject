using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UI_EventHandler))]
public class UI_ButtonSound : MonoBehaviour
{
    UI_EventHandler eventHandler;
    [SerializeField] SFXType type = SFXType.None;

    private void Awake()
    {
        eventHandler = GetComponent<UI_EventHandler>();
    }
    private void OnEnable()
    {
        eventHandler.AddUIEvent(OnClick, UI_EventHandler.UIEvent.LClick);
        eventHandler.AddUIEvent(OnHover, UI_EventHandler.UIEvent.Enter);
    }
    private void OnDisable()
    {
        eventHandler.RemoveUIEvent(OnClick, UI_EventHandler.UIEvent.LClick);
        eventHandler.RemoveUIEvent(OnHover, UI_EventHandler.UIEvent.Enter);
    }

    private void OnClick(PointerEventData data)
    {
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Global_ButtonClick);
        if(type != SFXType.None) GameManager.Instance.Utility.Audio.PlaySFX(type);
    }
    private void OnHover(PointerEventData data)
    { 
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Global_ButtonHover);
    } 
}
