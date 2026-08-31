using System;
using UnityEngine;

public sealed class HarvestUpgradeListPanel : MonoBehaviour
{
    private HarvestUpgradeSelectionButton[] selectionButtons;

    public event Action<HarvestUpgradeType> OnSelected;

    private void Awake()
    {
        selectionButtons = GetComponentsInChildren<HarvestUpgradeSelectionButton>(true);

        for (int i = 0; i < selectionButtons.Length; i++)
            selectionButtons[i]?.Initialize(this);

        ClearSelection();
    }

    public void Select(HarvestUpgradeType upgradeType)
    {
        if (upgradeType == HarvestUpgradeType.Count)
            return;

        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Hub_Select);

        for (int i = 0; i < selectionButtons.Length; i++)
        {
            HarvestUpgradeSelectionButton selectionButton = selectionButtons[i];
            selectionButton?.SetSelected(selectionButton.UpgradeType == upgradeType);
        }

        OnSelected?.Invoke(upgradeType);
    }

    public void ClearSelection()
    {
        if (selectionButtons == null)
            return;

        for (int i = 0; i < selectionButtons.Length; i++)
            selectionButtons[i]?.SetSelected(false);
    }
}
