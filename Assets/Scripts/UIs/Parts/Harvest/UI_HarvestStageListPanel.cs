using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class UI_HarvestStageListPanel : MonoBehaviour
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor =
        new(1f, 0.8f, 0.3f, 1f);

    private readonly Button[] stageButtons =
        new Button[(int)StageType.Count];
    private readonly UnityAction[] clickActions =
        new UnityAction[(int)StageType.Count];

    private StageType selectedStage = StageType.Count;
    private bool isInitialized;

    public event Action<StageType> OnSelected;

    public void Initialize()
    {
        if (isInitialized)
            return;

        isInitialized = true;

        int stageCount = Mathf.Min(
            transform.childCount,
            (int)StageType.Count);

        for (int i = 0; i < stageCount; i++)
        {
            Button button = transform.GetChild(i).GetComponent<Button>();

            if (button == null)
                continue;

            int stageIndex = i;
            UnityAction clickAction = () => Select((StageType)stageIndex);

            stageButtons[i] = button;
            clickActions[i] = clickAction;
            button.onClick.AddListener(clickAction);

            StageDataSO stageData = StageDataDB.GetData((StageType)i);
            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);

            if (label != null)
            {
                label.text = stageData != null
                    ? stageData.DisplayName
                    : $"Stage {i + 1}";
            }
        }
    }

    public void Select(StageType stageType)
    {
        int stageIndex = (int)stageType;

        if (stageIndex < 0 || stageIndex >= (int)StageType.Count)
            return;

        selectedStage = stageType;
        RefreshSelection();
        OnSelected?.Invoke(stageType);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i] != null && clickActions[i] != null)
                stageButtons[i].onClick.RemoveListener(clickActions[i]);
        }
    }

    private void RefreshSelection()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i]?.image != null)
            {
                stageButtons[i].image.color = i == (int)selectedStage
                    ? selectedColor
                    : normalColor;
            }
        }
    }
}
