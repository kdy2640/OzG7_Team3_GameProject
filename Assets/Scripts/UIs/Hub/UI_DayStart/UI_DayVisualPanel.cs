using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class UI_DayVisualPanel : MonoBehaviour
{
    private const int DayCardCount = 8;
    private const int PreviousDayCardIndex = 3;
    private const int CurrentDayCardIndex = 4;
    private const float AnimationDelay = 0.5f;
    private const float AnimationDuration = 0.35f;

    private readonly RectTransform[] dayCards = new RectTransform[DayCardCount];
    private readonly TMP_Text[] dayTexts = new TMP_Text[DayCardCount];
    private readonly TMP_Text[] dayShadowTexts = new TMP_Text[DayCardCount];
    private readonly Vector2[] initialCardPositions = new Vector2[DayCardCount];
    private readonly Vector2[] initialCardSizes = new Vector2[DayCardCount];
    private readonly Vector3[] initialCardScales = new Vector3[DayCardCount];
    private readonly Vector2[] initialDayTextPositions = new Vector2[DayCardCount];
    private readonly Vector3[] initialDayTextScales = new Vector3[DayCardCount];
    private readonly Color[] initialDayTextColors = new Color[DayCardCount];
    private readonly Vector2[] initialDayShadowPositions = new Vector2[DayCardCount];
    private readonly Vector3[] initialDayShadowScales = new Vector3[DayCardCount];
    private readonly Color[] initialDayShadowColors = new Color[DayCardCount];

    private UI_TodayDeco todayDeco;
    private Sequence animationSequence;
    private bool isInitialized;

    public void Init()
    {
        if (isInitialized)
            return;

        RectTransform grid = transform.Find("Grid") as RectTransform;

        for (int i = 0; i < DayCardCount; i++)
        {
            RectTransform dayCard = grid.Find($"UI_DayCard_{i + 1}") as RectTransform;
            TMP_Text dayText = dayCard.Find("Day").GetComponent<TMP_Text>();
            TMP_Text dayShadowText = dayCard.Find("Day_Shadow").GetComponent<TMP_Text>();

            dayCards[i] = dayCard;
            dayTexts[i] = dayText;
            dayShadowTexts[i] = dayShadowText;
            initialCardPositions[i] = dayCard.anchoredPosition;
            initialCardSizes[i] = dayCard.sizeDelta;
            initialCardScales[i] = dayCard.localScale;
            initialDayTextPositions[i] = dayText.rectTransform.anchoredPosition;
            initialDayTextScales[i] = dayText.rectTransform.localScale;
            initialDayTextColors[i] = dayText.color;
            initialDayShadowPositions[i] = dayShadowText.rectTransform.anchoredPosition;
            initialDayShadowScales[i] = dayShadowText.rectTransform.localScale;
            initialDayShadowColors[i] = dayShadowText.color;
        }

        todayDeco = transform.Find("Today_Deco").GetComponent<UI_TodayDeco>();
        todayDeco.Init();
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
        dayCards[DayCardCount - 1].gameObject.SetActive(true);

        Vector2 exitPosition = initialCardPositions[0]
            + initialCardPositions[0]
            - initialCardPositions[1];

        Sequence sequence = DOTween.Sequence()
            .SetUpdate(true)
            .AppendInterval(AnimationDelay)
            .Append(dayCards[0].DOAnchorPos(exitPosition, AnimationDuration)
                .SetEase(Ease.InOutSine))
            .Join(dayTexts[0].DOFade(0f, AnimationDuration));

        for (int i = 1; i < DayCardCount; i++)
        {
            int targetIndex = i - 1;

            sequence
                .Join(dayCards[i].DOAnchorPos(initialCardPositions[targetIndex], AnimationDuration)
                    .SetEase(Ease.InOutSine))
                .Join(dayCards[i].DOSizeDelta(initialCardSizes[targetIndex], AnimationDuration)
                    .SetEase(Ease.InOutSine))
                .Join(dayCards[i].DOScale(initialCardScales[targetIndex], AnimationDuration)
                    .SetEase(Ease.InOutSine))
                .Join(dayTexts[i].rectTransform
                    .DOAnchorPos(initialDayTextPositions[targetIndex], AnimationDuration)
                    .SetEase(Ease.InOutSine))
                .Join(dayTexts[i].rectTransform
                    .DOScale(initialDayTextScales[targetIndex], AnimationDuration)
                    .SetEase(Ease.InOutSine))
                .Join(dayTexts[i].DOColor(initialDayTextColors[targetIndex], AnimationDuration)
                    .SetEase(Ease.InOutSine));
        }

        animationSequence = sequence;
        yield return sequence.WaitForCompletion();

        if (animationSequence != sequence)
            yield break;

        animationSequence = null;
        dayCards[0].gameObject.SetActive(false);
        dayShadowTexts[PreviousDayCardIndex].gameObject.SetActive(false);
        dayShadowTexts[CurrentDayCardIndex].rectTransform.anchoredPosition =
            initialDayShadowPositions[PreviousDayCardIndex];
        dayShadowTexts[CurrentDayCardIndex].rectTransform.localScale =
            initialDayShadowScales[PreviousDayCardIndex];
        dayShadowTexts[CurrentDayCardIndex].color =
            initialDayShadowColors[PreviousDayCardIndex];
        dayShadowTexts[CurrentDayCardIndex].gameObject.SetActive(true);
        todayDeco.Show();
    }

    public void ShowTasteFestival(TasteType tasteType)
    {
        todayDeco.ShowTasteFestival(tasteType);
    }

    public void ShowCategoryFestival(CategoryType categoryType)
    {
        todayDeco.ShowCategoryFestival(categoryType);
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
            string day = isVisible ? businessDay.ToString() : string.Empty;

            SetDayCardVisible(i, isVisible);
            dayTexts[i].text = day;
            dayShadowTexts[i].text = day;
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
        for (int i = 0; i < DayCardCount; i++)
        {
            dayCards[i].gameObject.SetActive(i < DayCardCount - 1);
            dayCards[i].anchoredPosition = initialCardPositions[i];
            dayCards[i].sizeDelta = initialCardSizes[i];
            dayCards[i].localScale = initialCardScales[i];
            dayTexts[i].rectTransform.anchoredPosition = initialDayTextPositions[i];
            dayTexts[i].rectTransform.localScale = initialDayTextScales[i];
            dayTexts[i].color = initialDayTextColors[i];
            dayShadowTexts[i].rectTransform.anchoredPosition = initialDayShadowPositions[i];
            dayShadowTexts[i].rectTransform.localScale = initialDayShadowScales[i];
            dayShadowTexts[i].color = initialDayShadowColors[i];
            dayShadowTexts[i].gameObject.SetActive(
                i == PreviousDayCardIndex && !string.IsNullOrEmpty(dayTexts[i].text));
        }

        todayDeco.Hide();
    }
}
