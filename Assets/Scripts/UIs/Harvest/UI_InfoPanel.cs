using UnityEngine;

public sealed class UI_InfoPanel : MonoBehaviour
{
    [SerializeField] private TractorController player;
    [SerializeField] private GridChunkHandler gridChunkHandler;
    [SerializeField] private UI_GroceryViewPanel groceryViewPanel;

    private StageType currentStage = StageType.Count;

    private void Start()
    {
        RefreshGroceryViewPanel();
    }

    private void Update()
    {
        RefreshGroceryViewPanel();
    }

    public void RefreshGroceryViewPanel()
    {
        float playerLocalZ = gridChunkHandler.transform
            .InverseTransformPoint(player.transform.position)
            .z;
        StageDataSO currentStageData = StageDataDB.GetData(StageType.Stage_1);

        foreach (StageDataSO stageData in StageDataDB.GetAllData())
        {
            if (stageData.ZStart <= playerLocalZ
                && stageData.ZStart > currentStageData.ZStart)
            {
                currentStageData = stageData;
            }
        }

        if (currentStage == currentStageData.StageType)
        {
            return;
        }

        currentStage = currentStageData.StageType;
        groceryViewPanel.Initialize(currentStageData.RewardList);
    }
}
