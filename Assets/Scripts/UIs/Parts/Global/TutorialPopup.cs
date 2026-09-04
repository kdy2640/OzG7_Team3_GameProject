using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class TutorialPopup : MonoBehaviour
{
    [Header("Auto Open")]
    [SerializeField] private bool autoOpenOnStart = false;
    [SerializeField] private TutorialManager.TutorialType autoOpenTutorialType;

    [Header("UI")]
    [SerializeField] private RectTransform[] tutorialPanels;
    [SerializeField] private Button confirmButton;

    [Header("Animation")]
    [SerializeField] private PanelAnimator panelAnimator;

    private TutorialManager tutorialManager;
    private TutorialManager.TutorialType currentTutorialType;

    private Coroutine animationCoroutine;
    private float previousTimeScale;
    private bool isTimePaused;

    private void Awake()
    {
        tutorialManager = FindFirstObjectByType<TutorialManager>();

        if (tutorialManager == null)
        {
            Debug.LogError("[TutorialPopup] TutorialManager not found.");
        }

        if (panelAnimator == null) panelAnimator = GetComponent<PanelAnimator>();
         

        if (confirmButton != null) confirmButton.onClick.AddListener(Close);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!autoOpenOnStart) return;

        if (tutorialManager == null) return;

        if (tutorialManager.GetTutorialProgressed(
            autoOpenTutorialType))
        {
            return;
        }

        Open(autoOpenTutorialType);
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

    public void Open(TutorialManager.TutorialType tutorialType)
    {
        currentTutorialType = tutorialType;

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        for (int i = 0; i < tutorialPanels.Length; i++)
            tutorialPanels[i].gameObject.SetActive(i == (int)tutorialType);

        gameObject.SetActive(true);

        SceneType currentSceneType =
            GameManager.Instance.Scene.CurrentSceneType;
        bool shouldPauseTime =
            (tutorialType == TutorialManager.TutorialType.Sales
                && currentSceneType == SceneType.Service)
            || (tutorialType == TutorialManager.TutorialType.Harvest
                && currentSceneType == SceneType.Harvest);

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

        if (tutorialManager != null)
            tutorialManager.ResolveTutorial(currentTutorialType);

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        if (panelAnimator != null)
        {
            animationCoroutine = StartCoroutine(HideAnimation());
            return;
        }

        gameObject.SetActive(false);
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
