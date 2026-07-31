using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 포인터 이벤트 중계 컴포넌트입니다.
/// </summary>
public class UI_EventHandler : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerMoveHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IDeselectHandler
{
    public enum UIEvent
    {
        LClick,
        Enter,
        Exit,
        Hold,
        Deselect
    }

    public Action<PointerEventData> OnClickHandler;
    public Action<PointerEventData> OnPointerEnterHandler;
    public Action<PointerEventData> OnPointerMoveHandler;
    public Action<PointerEventData> OnPointerExitHandler;
    public Action<PointerEventData> OnPointerDownHandler;
    public Action<PointerEventData> OnPointerUpHandler;
    public Action<PointerEventData> OnHoldHandler;
    public Action<PointerEventData> OnDeselectHandler;

    [SerializeField, Min(0f)] private float holdThreshold = 0.5f;

    private Coroutine holdCoroutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterHandler?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitHandler?.Invoke(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        OnPointerMoveHandler?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownHandler?.Invoke(eventData);

        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
        }

        holdCoroutine = StartCoroutine(HoldCoroutine(eventData));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpHandler?.Invoke(eventData);
        StopHoldCoroutine();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselectHandler?.Invoke(eventData as PointerEventData);
    }

    private void OnDisable()
    {
        StopHoldCoroutine();
    }

    private IEnumerator HoldCoroutine(PointerEventData eventData)
    {
        yield return new WaitForSeconds(holdThreshold);
        holdCoroutine = null;
        OnHoldHandler?.Invoke(eventData);
    }

    private void StopHoldCoroutine()
    {
        if (holdCoroutine == null)
        {
            return;
        }

        StopCoroutine(holdCoroutine);
        holdCoroutine = null;
    }
}

public static class UIEventHandlerExtensions
{
    public static void AddUIEvent(
        this UI_EventHandler handler,
        Action<PointerEventData> action,
        UI_EventHandler.UIEvent eventType = UI_EventHandler.UIEvent.LClick)
    {
        if (handler == null || action == null)
        {
            return;
        }

        switch (eventType)
        {
            case UI_EventHandler.UIEvent.LClick:
                handler.OnClickHandler -= action;
                handler.OnClickHandler += action;
                break;
            case UI_EventHandler.UIEvent.Enter:
                handler.OnPointerEnterHandler -= action;
                handler.OnPointerEnterHandler += action;
                break;
            case UI_EventHandler.UIEvent.Exit:
                handler.OnPointerExitHandler -= action;
                handler.OnPointerExitHandler += action;
                break;
            case UI_EventHandler.UIEvent.Hold:
                handler.OnHoldHandler -= action;
                handler.OnHoldHandler += action;
                break;
            case UI_EventHandler.UIEvent.Deselect:
                handler.OnDeselectHandler -= action;
                handler.OnDeselectHandler += action;
                break;
        }
    }
}
