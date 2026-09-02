using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pauser : MonoBehaviour
{
    [SerializeField] UI_EventHandler openSettingButton;
    [SerializeField] UI_EventHandler closeSettingButton;

    private void OnEnable()
    {
        openSettingButton.AddUIEvent(OnOpenSetting);
        
    }

    private void OnDisable()
    {
        openSettingButton.RemoveUIEvent(OnOpenSetting);
        closeSettingButton.RemoveUIEvent(OnCloseSetting);
    }

    private void OnOpenSetting(PointerEventData data)
    {
        StartCoroutine(PauseCo());
    }

    private void OnCloseSetting(PointerEventData data)
    {
        Time.timeScale = 1f;
    }

    private IEnumerator PauseCo()
    {
        yield return new WaitForSeconds(0.5f);
        closeSettingButton = GameObject.Find("Close_Btn").GetComponent<UI_EventHandler>();
        closeSettingButton.AddUIEvent(OnCloseSetting);
        Time.timeScale = 0f;
    }
}
