using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GroceryView : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image iconImage;

    [Header("Gain")]
    [SerializeField, Min(0f)] private float gainPopScale = 1.25f;
    [SerializeField, Min(0f)] private float gainPopDuration = 0.12f;
    [SerializeField, Min(0f)] private float gainSettleDuration = 0.08f;
    [SerializeField, Min(0f)] private float gainIconJumpHeight = 8f;

    private GroceryType groceryType = GroceryType.Count;
    private Tween gainTween;
    private Vector3 amountBaseScale;
    private Vector2 iconBaseAnchoredPosition;

    private void Awake()
    {
        amountBaseScale = amountText.rectTransform.localScale;
        iconBaseAnchoredPosition = iconImage.rectTransform.anchoredPosition;
    }

    public void Initialize(GroceryType type)
    {
        StopGainTween();
        amountText.rectTransform.localScale = amountBaseScale;
        iconImage.rectTransform.anchoredPosition = iconBaseAnchoredPosition;

        groceryType = type;
        iconImage.sprite = GroceryDataDB.GetData(groceryType).Icon;
    }

    public void SetAmount(long amount)
    {
        amountText.text = amount.ToString();
    }

    public void PlayGain()
    {
        StopGainTween();

        RectTransform amountRect = amountText.rectTransform;
        RectTransform iconRect = iconImage.rectTransform;
        amountRect.localScale = amountBaseScale;
        iconRect.anchoredPosition = iconBaseAnchoredPosition;

        GameManager.Instance.Utility.Audio.PlaySFX(
            SFXType.Harvest_Collect);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            amountRect
                .DOScale(amountBaseScale * gainPopScale, gainPopDuration)
                .SetEase(Ease.OutBack));
        sequence.Join(
            iconRect
                .DOAnchorPosY(
                    iconBaseAnchoredPosition.y + gainIconJumpHeight,
                    gainPopDuration)
                .SetEase(Ease.OutQuad));
        sequence.Append(
            amountRect
                .DOScale(amountBaseScale, gainSettleDuration)
                .SetEase(Ease.OutQuad));
        sequence.Join(
            iconRect
                .DOAnchorPosY(
                    iconBaseAnchoredPosition.y,
                    gainSettleDuration)
                .SetEase(Ease.InQuad));

        gainTween = sequence;
    }

    public void Refresh()
    {
        if (groceryType == GroceryType.Count || amountText == null)
            return;

        IReadOnlyList<GroceryAmount> groceries =
            GameManager.Instance?.StockManager?.StockData?.Groceries;

        if (groceries == null)
            return;

        long amount = 0;

        for (int i = 0; i < groceries.Count; i++)
        {
            GroceryAmount grocery = groceries[i];

            if (grocery != null && grocery.grocery == groceryType)
                amount += grocery.amount;
        }

        SetAmount(amount);
    }

    private void StopGainTween()
    {
        if (gainTween != null)
        {
            gainTween.Kill();
            gainTween = null;
        }
    }

    private void OnDestroy()
    {
        StopGainTween();
    }
}
