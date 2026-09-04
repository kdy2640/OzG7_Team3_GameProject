using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pauser : MonoBehaviour
{
    [SerializeField] UI_EventHandler openSettingButton;
    [SerializeField] UI_EventHandler closeSettingButton;

    private Coroutine pauseCoroutine;

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
        if (pauseCoroutine != null)
            StopCoroutine(pauseCoroutine);

        pauseCoroutine = StartCoroutine(PauseCo());
    }

    private void OnCloseSetting(PointerEventData data)
    {
        Resume();
    }

    public void Resume()
    {
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
            pauseCoroutine = null;
        }

        Time.timeScale = 1f;
    }

    private IEnumerator PauseCo()
    {
        yield return new WaitForSeconds(0.5f);

        SceneType currentSceneType = GameManager.Instance.Scene.CurrentSceneType;
        bool hasSessionEnded =
            currentSceneType == SceneType.Service
                && !GameManager.Instance.Service.IsRunning
            || currentSceneType == SceneType.Harvest
                && !GameManager.Instance.Harvest.IsRunning;

        if (hasSessionEnded)
        {
            pauseCoroutine = null;
            yield break;
        }

        closeSettingButton = GameObject.Find("Close_Btn").GetComponent<UI_EventHandler>();
        closeSettingButton.AddUIEvent(OnCloseSetting);
        Time.timeScale = 0f;
        pauseCoroutine = null;
    }
}
