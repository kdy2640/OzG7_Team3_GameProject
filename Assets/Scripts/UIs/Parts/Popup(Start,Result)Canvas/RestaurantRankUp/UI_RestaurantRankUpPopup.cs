using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_RestaurantRankUpPopup : MonoBehaviour
{
    [Header("Reward Data")]
    [SerializeField] private RestaurantRankRewardSO rewardSO;

    [Header("Popup")]
    [SerializeField] private GameObject popupRoot;

    [Header("Menu")]
    [SerializeField] private GameObject[] menuSlots;
    [SerializeField] private Image[] menuImages;

    [Header("Ingredient")]
    [SerializeField] private GameObject[] ingredientSlots;
    [SerializeField] private Image[] ingredientImages;

    [Header("Function")]
    [SerializeField] private GameObject[] functionSlots;
    [SerializeField] private TMP_Text[] functionDescriptions;

    [SerializeField] private Image[] levelSlots;

    [SerializeField] private Color inactiveLevelColor = Color.gray;
    [SerializeField] private Color activeLevelColor = Color.green;

    private MarketManager marketManager;
    private int previousLevel;

    private const int MaxSlotCount = 5;

    private void Awake()
    {
        if (popupRoot != null) popupRoot.SetActive(false);
    }
    
    private void OnEnable()
    {
        if (GameManager.Instance == null) return;

        marketManager = GameManager.Instance.Market;

        if (marketManager == null) return;

        previousLevel = marketManager.MarketData.CurrentLevel;

        marketManager.SubscribeMarketDataChanged(HandleMarketChanged);
    }
    private void OnDisable()
    {
        if (marketManager != null)
        {
            marketManager.UnsubscribeMarketDataChanged(HandleMarketChanged);
        }
    }
    private void HandleMarketChanged()
    {
        if (marketManager == null) return;

        int currentLevel = marketManager.MarketData.CurrentLevel;

        //레벨이 실제 상승시에만 팝업 활성화
        if (currentLevel > previousLevel) Show(currentLevel);

        previousLevel = currentLevel;
    }

    public void Show(int level)
    {
        if (rewardSO == null)
        {
            Debug.LogWarning("[UI_RestaurantRankUpPopup] RestaurantRankRewardSO가 연결되지 않았습니다.");
            return;
        }
        RestaurantRankRewardData reward = rewardSO.GetReward(level);

        if (reward == null)
        {
            Debug.LogWarning($"[UI_RestaurantRankUpPopup] {level}레벨 보상 데이터가 없습니다.");
            return;
        }
        RefreshLevelSlots(level);
        RefreshMenu(reward.NewMenus);
        RefreshIngredient(reward.NewIngredients);
        RefreshFunction(reward.NewFunctions);

        if (popupRoot != null) popupRoot.SetActive(true);


    }
    public void Hide()
    {
        if (popupRoot != null) popupRoot.SetActive(false);
    }
    private void RefreshLevelSlots(int currentLevel)
    {
        if (levelSlots == null) return;

        int activeLevelCount = Mathf.Clamp(currentLevel, 0, levelSlots.Length);

        for (int i = 0; i < levelSlots.Length; i++)
        {
            if (levelSlots[i] == null) continue;

            levelSlots[i].color =
                i < activeLevelCount ? activeLevelColor : inactiveLevelColor;
        }
    }

    private void RefreshMenu(IReadOnlyList<Sprite> sprites)
    {
        RefreshImageSlots(menuSlots, menuImages, sprites);
    }
    private void RefreshIngredient(IReadOnlyList<Sprite> sprites)
    {
        RefreshImageSlots(ingredientSlots,ingredientImages , sprites);
    }
    private void RefreshImageSlots
        (GameObject[] slots, Image[] images, IReadOnlyList<Sprite> sprites)
    {
        int slotCount = Mathf.Min(MaxSlotCount, Mathf.Min(slots?.Length ?? 0, images?.Length ?? 0));

        // 모든 슬롯 초기화
        for (int i = 0; i < slotCount; i++)
        {
            if (slots[i] != null) slots[i].SetActive(false);

            if (images[i] != null)
            {
                images[i].sprite = null;
                images[i].enabled = false;
            }
        }

        if (sprites == null || sprites.Count == 0) return;

        int count = Mathf.Min(sprites.Count, slotCount);

        // 5개 슬롯 기준 가운데 정렬
        int startIndex = (slotCount - count) / 2;

        for (int i = 0; i < count; i++)
        {
            Sprite sprite = sprites[i];

            if (sprite == null) continue;

            int slotIndex = startIndex + i;

            if (slots[slotIndex] != null) slots[slotIndex].SetActive(true);

            if (images[slotIndex] != null)
            {
                images[slotIndex].sprite = sprite;
                images[slotIndex].enabled = true;
            }
        }
    }
    private void RefreshFunction(IReadOnlyList<string> functions)
    {
        int slotCount = Mathf.Min(MaxSlotCount,
            Mathf.Min(functionSlots?.Length ?? 0, functionDescriptions?.Length ?? 0));
        
        //모든 슬롯 초기화
        for (int i = 0; i < slotCount; i++)
        {
            if (functionSlots[i] != null) functionSlots[i].SetActive(false);

            if (functionDescriptions[i] != null) functionDescriptions[i].text = string.Empty;
        }

        if (functions == null || functions.Count == 0) return;

        int count = Mathf.Min(functions.Count, slotCount);

        for (int i = 0; i < count; i++)
        {
            if (functionSlots[i] != null) functionSlots[i].SetActive(true);

            if (functionDescriptions[i] != null) functionDescriptions[i].text = functions[i];
        }
    }

}

