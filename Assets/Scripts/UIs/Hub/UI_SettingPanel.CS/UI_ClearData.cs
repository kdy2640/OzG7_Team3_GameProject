using UnityEngine;
using UnityEngine.UI;

public class UI_ClearData : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] private SaveManager saveManager;


    void Start()
    {
        button.onClick.AddListener(OnClickButtonHandler);
    }

    private void OnDestroy()
    { 
        button.onClick.RemoveListener(OnClickButtonHandler);
    }
    private void OnClickButtonHandler()
    {
        if (saveManager == null)
        {
            Debug.LogWarning("SaveManager is not assigned.");
            return;
        }

        saveManager.ResetSave();
        GameManager.Instance.Scene.ChangeScene(SceneType.Main, true);
    }
}
