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
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button confirmButton;

    [Header("Animation")]
    [SerializeField] private PanelAnimator panelAnimator;

    private TutorialManager tutorialManager;
    private TutorialDataSO currentTutorialData;

    private Coroutine animationCoroutine;

    private void Awake()
    {
        tutorialManager = FindFirstObjectByType<TutorialManager>();

        if (tutorialManager == null)
        {
            Debug.LogError("[TutorialPopup] TutorialManager를 찾을 수 없습니다.");
        }

        if (panelAnimator == null) panelAnimator = GetComponent<PanelAnimator>();

        if (closeButton != null) closeButton.onClick.AddListener(Close);

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
        if (closeButton != null) closeButton.onClick.RemoveListener(Close);

        if (confirmButton != null) confirmButton.onClick.RemoveListener(Close);
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

        if (panelAnimator != null) animationCoroutine = StartCoroutine(ShowAnimation());
    }

    public void Close()
    {
        if (!gameObject.activeSelf)
            return;

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

        if (tutorialImage != null)
        {
            tutorialImage.sprite = currentTutorialData.image;
            tutorialImage.gameObject.SetActive(
                currentTutorialData.image != null);
        }
    }

    private IEnumerator ShowAnimation()
    {
        yield return panelAnimator.Show();
        animationCoroutine = null;
    }

    private IEnumerator HideAnimation()
    {
        yield return panelAnimator.Hide();

        animationCoroutine = null;
        gameObject.SetActive(false);
    }
}