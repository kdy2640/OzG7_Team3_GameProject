using UnityEngine;

public class QueueSlot
{
    private KitchenSlotData data;
    private KitchenSlotViewer viewer;

    public KitchenSlotData Data => data;
    public KitchenSlotViewer Viewer => viewer;

    public void SetData(KitchenSlotData data) => this.data = data;
    public void SetViewer(KitchenSlotViewer viewer) => this.viewer = viewer;
}
