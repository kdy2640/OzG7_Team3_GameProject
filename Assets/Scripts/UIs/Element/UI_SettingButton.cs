using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_SettingButton : MonoBehaviour
{
    [SerializeField] private SettingsPopup settingPanelPrefab;

    private Button button;
    private SettingsPopup settingPanel;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenSettingPanel);

        settingPanel = FindFirstObjectByType<SettingsPopup>(FindObjectsInactive.Include);
        if (settingPanel != null)
        {
            DontDestroyOnLoad(settingPanel.gameObject);
            settingPanel.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OpenSettingPanel);
    }

    private void OpenSettingPanel()
    {
        if (settingPanel == null)
        {
            settingPanel = FindFirstObjectByType<SettingsPopup>(FindObjectsInactive.Include);
        }

        if (settingPanel == null)
        {
            settingPanel = Instantiate(settingPanelPrefab);
            DontDestroyOnLoad(settingPanel.gameObject);
        }

        settingPanel.Open();
    }
}
