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
        GetComponentInParent<SettingsPopup>().Close();
        GameManager.Instance.Utility.Save.ResetSave();
        GameManager.Instance.Scene.ChangeScene(SceneType.Main, true);
    }
}
