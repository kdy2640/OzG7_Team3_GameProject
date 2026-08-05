using UnityEngine;
using TMPro;

// 허브 화면에 표시되는 플레이어 정보와 재화 정보를 갱신하는 UI.
// UI_HubView와 별도로 데이터 표시만 담당한다.

public sealed class UI_HubDisplay : MonoBehaviour
{
    [Header("Top Resource")]
    [SerializeField] private TMP_Text goldText;

    [Header("Player Information")]
    [SerializeField] private TMP_Text playerLevelText;

    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private TMP_Text totalGoldText;

    [SerializeField] private TMP_Text promotionQuestTitleText;

    [SerializeField] private TMP_Text promotionQuestDescriptionText;

    public void SetData(HubDisplayData data)
    {
        if (data == null) return;

        goldText.text = data.Gold.ToString("N0");

        playerLevelText.text = $"Lv. {data.PlayerLevel}";
        playerNameText.text = data.PlayerName;

        totalGoldText.text = data.TotalGold.ToString("N0");

        promotionQuestTitleText.text = data.PromotionQuestTitle;
        promotionQuestDescriptionText.text = data.PromotionQuestDescription;
    }
}
//예시
//HubDisplayData data = new HubDisplayData
//{
//    Gold = 15000,
//    PlayerLevel = 1,
//    PlayerName = "Restaurant Master",
//    TotalGold = 125000,
//    PromotionQuestTitle = "Serve 10 Customers",
//    PromotionQuestDescription = "Complete today's service with 10 satisfied customers."
//};

//hubDisplay.SetData(data);


