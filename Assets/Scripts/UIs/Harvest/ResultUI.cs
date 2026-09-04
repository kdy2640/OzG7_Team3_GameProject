using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private Transform rewardArea;

    [SerializeField] private Button hubButton;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    private UI_GroceryViewPanel[] rewardPanels;
    private CanvasGroup[] rewardCanvasGroups;
    private RectTransform[] rewardRects;
    private bool isInitialized;

    private void Awake()
    {
        rewardPanels = rewardArea.GetComponentsInChildren<UI_GroceryViewPanel>(true);
        rewardCanvasGroups = new CanvasGroup[rewardPanels.Length];
        rewardRects = new RectTransform[rewardPanels.Length];

        bool hasRewardAnimationReferences = true;

        for (int i = 0; i < rewardPanels.Length; i++)
        {
            rewardCanvasGroups[i] = rewardPanels[i].GetComponent<CanvasGroup>();
            rewardRects[i] = rewardPanels[i].GetComponent<RectTransform>();

            if (rewardCanvasGroups[i] == null || rewardRects[i] == null)
                hasRewardAnimationReferences = false;
        }

        isInitialized = rewardPanels.Length == (int)StageType.Count
            && canvasGroup != null
            && panel != null
            && hasRewardAnimationReferences;

        if (!isInitialized)
            Debug.LogError($"[{nameof(ResultUI)}] 초기화에 필요한 참조가 없습니다.", this);

        if (hubButton != null)
            hubButton.onClick.AddListener(HandleHubButtonClicked);

    }

    private void OnDestroy()
    {
        if (hubButton != null)
            hubButton.onClick.RemoveListener(HandleHubButtonClicked);
    }

    public void SetData(IReadOnlyList<GroceryAmount> _)
    {
        if (!isInitialized)
            return;

        for (int i = 0; i < rewardPanels.Length; i++)
            rewardPanels[i].Initialize(StageDataDB.GetData((StageType)i).RewardList);
    }

    public IEnumerator Show()
    {
        if (!isInitialized)
            yield break;

        GameManager.Instance.Utility.Audio.PlaySFX(
            SFXType.Harvest_ResultReveal);
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.8f;

        for (int i = 0; i < rewardCanvasGroups.Length; i++)
        {
            rewardCanvasGroups[i].DOKill();
            rewardRects[i].DOKill();
            rewardCanvasGroups[i].alpha = 0f;
            rewardCanvasGroups[i].blocksRaycasts = false;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.Join(canvasGroup.DOFade(1f, 0.4f));
        sequence.Join(panel.DOScale(1f, 0.45f).SetEase(Ease.OutBack));

        yield return sequence.WaitForCompletion();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rewardArea);

        Sequence rewardSequence = DOTween.Sequence();

        for (int i = 0; i < rewardPanels.Length; i++)
        {
            CanvasGroup rewardCanvasGroup = rewardCanvasGroups[i];
            RectTransform rewardRect = rewardRects[i];
            Vector2 endPosition = rewardRect.anchoredPosition;
            float startTime = i * 0.1f;

            rewardRect.anchoredPosition = endPosition + Vector2.down * 30f;

            rewardSequence.InsertCallback(startTime, () =>
                GameManager.Instance.Utility.Audio.PlaySFX(
                    SFXType.Global_PanelPopup));
            rewardSequence.Insert(
                startTime,
                rewardCanvasGroup.DOFade(1f, 0.16f));
            rewardSequence.Insert(
                startTime,
                rewardRect.DOAnchorPos(endPosition, 0.22f)
                    .SetEase(Ease.OutCubic));
        }

        rewardSequence.OnComplete(() =>
        {
            for (int i = 0; i < rewardCanvasGroups.Length; i++)
                rewardCanvasGroups[i].blocksRaycasts = true;
        });

        yield return rewardSequence.WaitForCompletion();
    }

    public void Hide()
    {
        if (!isInitialized)
            return;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(canvasGroup.DOFade(0f, 0.25f));
        sequence.Join(panel.DOScale(0.85f, 0.25f));
        sequence.OnComplete(() => gameObject.SetActive(false));
    }

    private void HandleHubButtonClicked()
    {
        if (hubButton != null)
            PlayButtonAnimation(hubButton);

        GameManager.Instance.Scene.ChangeScene(SceneType.Hub);
    }

    private static void PlayButtonAnimation(Button button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.DOKill();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOScale(0.9f, 0.09f));
        sequence.Append(rectTransform.DOScale(1f, 0.12f));
    }
}
