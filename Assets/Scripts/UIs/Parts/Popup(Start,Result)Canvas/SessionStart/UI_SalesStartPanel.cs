using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SalesStartPanel : MonoBehaviour
{
    [Header("Level Slots")]
    [SerializeField] private Image[] levelSlots;

    [Header("Slot Colors")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;

    [Header("Sales Goal")]
    [SerializeField] private TMP_Text salesGoalText;

    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, Min(0f)] private float fadeDuration = 0.25f;

    private const int MinLevel = 1;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("[UI_SalesStartPanel] CanvasGroup이 연결되지 않았습니다.", this);
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        if (GameManager.Instance == null
            || GameManager.Instance.Market == null
            || GameManager.Instance.Upgrade == null)
        {
            Debug.LogError("[UI_SalesStartPanel] 표시 데이터를 가져올 수 없습니다.", this);
            return;
        }

        MarketData marketData = GameManager.Instance.Market.MarketData;

        if (marketData == null)
            return;

        int currentLevel = Mathf.Clamp(
            marketData.CurrentLevel,
            MinLevel,
            levelSlots.Length);

        int customerCount = Mathf.RoundToInt(
            GameManager.Instance.Upgrade.RuntimeStat.Service
                .Get(ServiceStatType.CustomerCount));

        RefreshLevelSlots(currentLevel);

        if (salesGoalText != null)
        {
            salesGoalText.text =
                $"오늘의 영업 목표 : 손님 {customerCount:N0}명 응대";
        }
    }

    public IEnumerator Show()
    {
        if (canvasGroup == null)
            yield break;

        gameObject.SetActive(true);
        Refresh();

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return canvasGroup
            .DOFade(1f, fadeDuration)
            .WaitForCompletion();
    }

    public IEnumerator Hide()
    {
        if (canvasGroup == null || !gameObject.activeSelf)
            yield break;

        canvasGroup.DOKill();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return canvasGroup
            .DOFade(0f, fadeDuration)
            .WaitForCompletion();

        gameObject.SetActive(false);
    }

    private void RefreshLevelSlots(int currentLevel)
    {
        for (int i = 0; i < levelSlots.Length; i++)
        {
            if (levelSlots[i] == null)
                continue;

            int slotLevel = i + 1;
            levelSlots[i].color =
                slotLevel <= currentLevel ? activeColor : inactiveColor;
        }
    }
}
