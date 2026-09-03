using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_GoldElement : MonoBehaviour
{
    private const float GainScale = 1.15f;
    private const float ScaleUpDuration = 0.12f;
    private const float HoldDuration = 0.08f;
    private const float RestoreDuration = 0.18f;

    [SerializeField] TextMeshProUGUI goldText;

    private int displayedGold;
    private bool isInitialized;
    private Color originalColor;
    private Vector3 originalScale;
    private Sequence gainSequence;

    void Start()
    {
        originalColor = goldText.color;
        originalScale = goldText.rectTransform.localScale;

        GameManager.Instance.StockManager.SubscribeStockDataChange(UpdateGold);
        UpdateGold();
    }

    void OnDestroy()
    {
        if (gainSequence != null)
        {
            gainSequence.Kill();
        }

        GameManager.Instance.StockManager.UnsubscribeStockDataChange(UpdateGold);
    }

    private void UpdateGold()
    {
        int updatedGold = GameManager.Instance.StockManager.StockData.Currency;
        goldText.text = updatedGold.ToString();

        if (isInitialized && updatedGold > displayedGold)
        {
            PlayGainEffect();
        }

        displayedGold = updatedGold;
        isInitialized = true;
    }

    private void PlayGainEffect()
    {
        if (gainSequence != null)
        {
            gainSequence.Kill();
        }

        goldText.color = Color.yellow;
        goldText.rectTransform.localScale = originalScale;

        gainSequence = DOTween.Sequence()
            .Append(goldText.rectTransform.DOScale(originalScale * GainScale, ScaleUpDuration)
                .SetEase(Ease.OutBack))
            .AppendInterval(HoldDuration)
            .Append(goldText.rectTransform.DOScale(originalScale, RestoreDuration)
                .SetEase(Ease.OutCubic))
            .Join(goldText.DOColor(originalColor, RestoreDuration))
            .OnComplete(() => gainSequence = null);
    }
}
