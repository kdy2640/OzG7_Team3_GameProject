using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ButtonClickSound : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler
{
    [SerializeField] private AudioManager audioManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioManager == null && GameManager.Instance != null)
            audioManager = GameManager.Instance.Utility.Audio;

        if (audioManager != null)
            audioManager.PlaySFX(SFXType.Global_ButtonClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 현재 프로젝트에는 UIHover 사운드가 없으므로 아무것도 하지 않음
    }
}