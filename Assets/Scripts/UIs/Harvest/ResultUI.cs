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
    private bool isInitialized;

    private void Awake()
    {
        rewardPanels = rewardArea.GetComponentsInChildren<UI_GroceryViewPanel>(true);

        isInitialized = rewardPanels.Length == (int)StageType.Count
            && canvasGroup != null
            && panel != null;

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

        Sequence sequence = DOTween.Sequence();
        sequence.Join(canvasGroup.DOFade(1f, 0.4f));
        sequence.Join(panel.DOScale(1f, 0.45f).SetEase(Ease.OutBack));

        yield return sequence.WaitForCompletion();
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
