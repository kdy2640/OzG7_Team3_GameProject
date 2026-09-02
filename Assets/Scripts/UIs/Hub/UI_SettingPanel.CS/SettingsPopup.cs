using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] private Canvas SettingCanvas;
    [SerializeField] private RectTransform popupContainer;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button toggleButton;
    [SerializeField] private TMP_Text toggleText;

    [Header("Exit Buttons")]
    [SerializeField] private Button gameExitButton;
    [SerializeField] private Button serviceEndButton;

    [SerializeField] private AudioManager audioManager;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Volume % Text")]
    [SerializeField] private TextMeshProUGUI masterVolText;
    [SerializeField] private TextMeshProUGUI bgmVolText;
    [SerializeField] private TextMeshProUGUI sfxVolText;

    private bool isToggleOn = true;

    private float prevMaster = 1f;
    private float prevBGM = 1f;
    private float prevSFX = 1f;

    private void Awake()
    {
        SettingCanvas.renderMode = RenderMode.ScreenSpaceOverlay; 

        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        if (closeButton != null) closeButton.onClick.AddListener(Close);

        if (toggleButton != null) toggleButton.onClick.AddListener(Toggle);

        gameExitButton.onClick.AddListener(ExitGame);
        serviceEndButton.onClick.AddListener(EndSession);

        RefreshToggleText();
        UpdateVolumeTexts();
    }

    private void Start()
    {
        // 현재 프로젝트 구조:
        // GameManager -> Utility -> Audio
        if (audioManager == null && GameManager.Instance != null)
        {
            audioManager = GameManager.Instance.Utility.Audio;
        }

        SyncSlidersFromAudioManager();
    }

    private void OnDestroy()
    {
        if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(OnMasterChanged);

        if (bgmSlider != null) bgmSlider.onValueChanged.RemoveListener(OnBGMChanged);

        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);

        if (closeButton != null) closeButton.onClick.RemoveListener(Close);

        if (toggleButton != null) toggleButton.onClick.RemoveListener(Toggle);

        gameExitButton.onClick.RemoveListener(ExitGame);
        serviceEndButton.onClick.RemoveListener(EndSession);
    }

    private void UpdateVolumeTexts()
    {
        if (masterVolText != null) masterVolText.text = $"{masterSlider.value * 100f:f0}%";

        if (bgmVolText != null) bgmVolText.text = $"{bgmSlider.value * 100f:f0}%";

        if (sfxVolText != null) sfxVolText.text = $"{sfxSlider.value * 100f:f0}%";
    }

    private void SyncSlidersFromAudioManager()
    {
        if (audioManager == null) return;

        masterSlider.SetValueWithoutNotify(audioManager.MasterVolume);
        bgmSlider.SetValueWithoutNotify(audioManager.BGMVolume);
        sfxSlider.SetValueWithoutNotify(audioManager.SFXVolume);

        isToggleOn = masterSlider.value > 0f || bgmSlider.value > 0f || sfxSlider.value > 0f;

        if (isToggleOn)
        {
            prevMaster = masterSlider.value;
            prevBGM = bgmSlider.value;
            prevSFX = sfxSlider.value;
        }

        RefreshToggleText();
        UpdateVolumeTexts();
    }

    private void OnMasterChanged(float value)
    {
        if (audioManager == null) return;

        audioManager.SetMasterVolume(value);

        if (masterVolText != null) masterVolText.text = $"{value * 100f:f0}%";
    }

    private void OnBGMChanged(float value)
    {
        if (audioManager == null) return;

        audioManager.SetBGMVolume(value);

        if (bgmVolText != null) bgmVolText.text = $"{value * 100f:f0}%";
    }

    private void OnSFXChanged(float value)
    {
        if (audioManager == null) return;

        audioManager.SetSFXVolume(value);

        if (sfxVolText != null) sfxVolText.text = $"{value * 100f:f0}%";
    }

    public void Open()
    {
        popupContainer.DOKill();

        gameObject.SetActive(true);

        RefreshExitButtons();
        SyncSlidersFromAudioManager();

        popupContainer.localScale = Vector3.zero;

        popupContainer.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        popupContainer.DOKill();

        popupContainer.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void Toggle()
    {
        isToggleOn = !isToggleOn;

        if (!isToggleOn)
        {
            prevMaster = masterSlider.value;
            prevBGM = bgmSlider.value;
            prevSFX = sfxSlider.value;

            masterSlider.value = 0f;
            bgmSlider.value = 0f;
            sfxSlider.value = 0f;
        }
        else
        {
            masterSlider.value = prevMaster;
            bgmSlider.value = prevBGM;
            sfxSlider.value = prevSFX;
        }

        RefreshToggleText();
    }

    private void RefreshExitButtons()
    {
        SceneType currentSceneType =
            GameManager.Instance.Scene.CurrentSceneType;
        bool isServiceScene = currentSceneType == SceneType.Service;
        bool isHarvestScene = currentSceneType == SceneType.Harvest;

        gameExitButton.gameObject.SetActive(!isServiceScene && !isHarvestScene);
        serviceEndButton.gameObject.SetActive(isServiceScene || isHarvestScene);

        if (isServiceScene || isHarvestScene)
        {
            serviceEndButton.GetComponentInChildren<TMP_Text>().text =
                isServiceScene ? "영업 종료" : "수확 종료";
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void EndSession()
    {
        Time.timeScale = 1f;
        Close();

        switch (GameManager.Instance.Scene.CurrentSceneType)
        {
            case SceneType.Service:
                GameManager.Instance.Service.EndLoop();
                break;
            case SceneType.Harvest:
                GameManager.Instance.Harvest.EndLoop();
                break;
        }
    }

    private void RefreshToggleText()
    {
        if (toggleText == null) return;

        toggleText.text = isToggleOn ? "ON" : "OFF";
    }
}
