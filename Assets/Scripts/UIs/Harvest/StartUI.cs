using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StartUI : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private string message = "수확 시작!";
    [SerializeField] private float panelOpenDuration = 0.23f;
    [SerializeField] private float panelCloseDuration = 0.23f;
    [SerializeField] private float textOpenDuration = 0.35f;
    [SerializeField] private float textCloseDuration = 0.35f;
    [SerializeField] private float waveDuration = 1.2f;

    public UnityEvent onFinished;

    private bool isPlaying;

    private void Awake()
    {
        if (panel == null)
            Debug.LogError("[StartUI] Panel이 연결되지 않았습니다.", this);

        if (messageText == null)
            Debug.LogError("[StartUI] MessageText가 연결되지 않았습니다.", this);
    }

    public void Play()
    {
        if (isPlaying)
            return;

        gameObject.SetActive(true);
        StartCoroutine(PlayRoutine());
    }

    public IEnumerator PlayRoutine()
    {
        if (panel == null || messageText == null)
            yield break;

        isPlaying = true;
        panel.localScale = new Vector3(1f, 0f, 1f);
        messageText.rectTransform.localScale = Vector3.zero;
        messageText.text = message;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOScaleY(1f, panelOpenDuration).SetEase(Ease.OutBack));
        sequence.Join(messageText.rectTransform
            .DOScale(1f, textOpenDuration)
            .SetEase(Ease.OutBack));

        yield return sequence.WaitForCompletion();

        messageText.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1f);
        messageText.rectTransform.DOShakePosition(
            waveDuration,
            new Vector3(6f, 3f, 0f),
            20,
            90,
            false,
            true);
        messageText.rectTransform.DOShakeRotation(waveDuration, 2f, 20, 90, false);

        yield return new WaitForSeconds(waveDuration);

        sequence = DOTween.Sequence();
        sequence.Append(messageText.rectTransform.DOScale(0f, textCloseDuration));
        sequence.Join(panel.DOScaleY(0f, panelCloseDuration).SetEase(Ease.InBack));

        yield return sequence.WaitForCompletion();

        isPlaying = false;
        gameObject.SetActive(false);
        onFinished?.Invoke();
    }
}
