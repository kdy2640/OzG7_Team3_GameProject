using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_DayVisualPanel : MonoBehaviour
{
    private const int DayCardCount = 8;
    private const int PreviousDayCardIndex = 3;
    private const int CurrentDayCardIndex = 4;
    private const float AnimationDelay = 0.5f;
    private const float AnimationDuration = 0.35f;

    private readonly RectTransform[] dayCards = new RectTransform[DayCardCount];
    private readonly TMP_Text[] dayTexts = new TMP_Text[DayCardCount];

    private RectTransform grid;
    private HorizontalLayoutGroup gridLayout;
    private Vector2 initialGridPosition;
    private Vector2 normalCardSize;
    private Vector2 emphasizedCardSize;
    private Sequence animationSequence;
    private bool isInitialized;

    public void Init()
    {
        if (isInitialized)
            return;

        grid = transform.Find("Grid") as RectTransform;
        gridLayout = grid != null ? grid.GetComponent<HorizontalLayoutGroup>() : null;

        if (grid == null || gridLayout == null)
        {
            Debug.LogError($"[{nameof(UI_DayVisualPanel)}] Grid 또는 HorizontalLayoutGroup을 찾을 수 없습니다.", this);
            return;
        }

        for (int i = 0; i < DayCardCount; i++)
        {
            Transform cardTransform = grid.Find($"UI_DayCard_{i + 1}");
            Transform dayTextTransform = cardTransform?.Find("Day");

            dayCards[i] = cardTransform as RectTransform;
            dayTexts[i] = dayTextTransform?.GetComponent<TMP_Text>();

            if (dayCards[i] == null || dayTexts[i] == null)
            {
                Debug.LogError($"[{nameof(UI_DayVisualPanel)}] UI_DayCard_{i + 1} 또는 Day 텍스트를 찾을 수 없습니다.", this);
                return;
            }
        }

        initialGridPosition = grid.anchoredPosition;
        normalCardSize = dayCards[CurrentDayCardIndex].sizeDelta;
        emphasizedCardSize = dayCards[PreviousDayCardIndex].sizeDelta;
        isInitialized = true;
    }

    public IEnumerator SyncAndPlay(int currentBusinessDay)
    {
        if (!isInitialized)
        {
            Debug.LogError($"[{nameof(UI_DayVisualPanel)}] Init() 호출 전에 날짜 연출을 실행할 수 없습니다.", this);
            yield break;
        }

        animationSequence?.Kill();
        animationSequence = null;

        SyncDayTexts(currentBusinessDay);
        ResetVisualState();

        Vector2 targetGridPosition = initialGridPosition
            + Vector2.left * (normalCardSize.x + gridLayout.spacing);

        Sequence sequence = DOTween.Sequence()
            .SetUpdate(true)
            .AppendInterval(AnimationDelay)
            .Append(grid.DOAnchorPos(targetGridPosition, AnimationDuration)
                .SetEase(Ease.InOutSine))
            .Join(dayCards[PreviousDayCardIndex].DOSizeDelta(normalCardSize, AnimationDuration)
                .SetEase(Ease.InOutSine))
            .Join(dayCards[CurrentDayCardIndex].DOSizeDelta(emphasizedCardSize, AnimationDuration)
                .SetEase(Ease.InOutSine))
            .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(grid));

        animationSequence = sequence;
        yield return sequence.WaitForCompletion();

        if (animationSequence == sequence)
            animationSequence = null;
    }

    private void OnDisable()
    {
        animationSequence?.Kill();
        animationSequence = null;
    }

    private void SyncDayTexts(int currentBusinessDay)
    {
        for (int i = 0; i < DayCardCount; i++)
        {
            int businessDay = currentBusinessDay + i - CurrentDayCardIndex;
            bool isVisible = businessDay > 0;

            SetDayCardVisible(i, isVisible);
            dayTexts[i].text = isVisible ? businessDay.ToString() : string.Empty;
        }
    }

    private void SetDayCardVisible(int cardIndex, bool isVisible)
    {
        RectTransform dayCard = dayCards[cardIndex];

        for (int i = 0; i < dayCard.childCount; i++)
            dayCard.GetChild(i).gameObject.SetActive(isVisible);
    }

    private void ResetVisualState()
    {
        grid.anchoredPosition = initialGridPosition;
        dayCards[PreviousDayCardIndex].sizeDelta = emphasizedCardSize;
        dayCards[CurrentDayCardIndex].sizeDelta = normalCardSize;
        LayoutRebuilder.ForceRebuildLayoutImmediate(grid);
    }
}
