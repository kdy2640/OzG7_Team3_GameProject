using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TutorialPopup : MonoBehaviour
{
    [Header("Auto Open")]
    [SerializeField] private bool autoOpenOnStart = false;
    [SerializeField] private TutorialDataSO autoOpenTutorialData;

    [Header("UI")]
    [SerializeField] private TMP_Text titleText; 
    [SerializeField] private TMP_Text descriptionText; 
    [SerializeField] private Button confirmButton;

    [Header("Animation")]
    [SerializeField] private PanelAnimator panelAnimator;

    private TutorialManager tutorialManager;
    private TutorialDataSO currentTutorialData;

    private Coroutine animationCoroutine;
    private float previousTimeScale;
    private bool isTimePaused;

    private void Awake()
    {
        tutorialManager = FindFirstObjectByType<TutorialManager>();

        if (tutorialManager == null)
        {
            Debug.LogError("[TutorialPopup] TutorialManager를 찾을 수 없습니다.");
        }

        if (panelAnimator == null) panelAnimator = GetComponent<PanelAnimator>();
         

        if (confirmButton != null) confirmButton.onClick.AddListener(Close);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!autoOpenOnStart) return;

        if (autoOpenTutorialData == null)
        {
            Debug.LogWarning("[TutorialPopup] 자동 오픈용 TutorialDataSO가 없습니다.");
            return;
        }

        if (tutorialManager == null) return;

        if (tutorialManager.GetTutorialProgressed(
            autoOpenTutorialData.tutorialType))
        {
            return;
        }

        Open(autoOpenTutorialData);
    }

    private void OnDestroy()
    {
        ResumeTime();

        if (confirmButton != null) confirmButton.onClick.RemoveListener(Close);
    }

    private void OnDisable()
    {
        ResumeTime();
    }

    public void Open(TutorialDataSO data)
    {
        if (data == null)
        {
            Debug.LogWarning("[TutorialPopup] TutorialDataSO가 없습니다.");
            return;
        }

        currentTutorialData = data;

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        RefreshUI();

        gameObject.SetActive(true);

        bool shouldPauseTime =
            data.tutorialType == TutorialManager.TutorialType.Sales
            && GameManager.Instance.Scene.CurrentSceneType == SceneType.Service;

        if (panelAnimator != null)
        {
            animationCoroutine = StartCoroutine(
                ShowAnimation(shouldPauseTime));
            return;
        }

        if (shouldPauseTime)
            PauseTime();
    }

    public void Close()
    {
        if (!gameObject.activeSelf)
            return;

        ResumeTime();

        if (tutorialManager != null && currentTutorialData != null)
        {
            tutorialManager.ResolveTutorial(currentTutorialData.tutorialType);
        }

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        if (panelAnimator != null)
        {
            animationCoroutine = StartCoroutine(HideAnimation());
            return;
        }

        gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        if (currentTutorialData == null) return;

        if (titleText != null)
            titleText.text = currentTutorialData.title;

        if (descriptionText != null)
            descriptionText.text = currentTutorialData.description;
         
    }

    private IEnumerator ShowAnimation(bool shouldPauseTime)
    {
        yield return panelAnimator.Show();
        animationCoroutine = null;

        if (shouldPauseTime)
            PauseTime();
    }

    private IEnumerator HideAnimation()
    {
        yield return panelAnimator.Hide();

        animationCoroutine = null;
        gameObject.SetActive(false);
    }

    private void PauseTime()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        isTimePaused = true;
    }

    private void ResumeTime()
    {
        if (!isTimePaused)
            return;

        Time.timeScale = previousTimeScale;
        isTimePaused = false;
    }
}
