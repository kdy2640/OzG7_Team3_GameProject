using System;
using Unity.VisualScripting;
using UnityEngine;

public class Cooker
{

    private KitchenSlotData data;
    private KitchenSlotViewer viewer;
    private bool isBusy;
    

    public KitchenSlotData Data => data;
    public KitchenSlotViewer Viewer => viewer;

    public bool IsBusy => isBusy;
    public void GetNextCook(KitchenSlotData data)
    {
        isBusy = true;
        this.data = data;
    }
    public void FinishCooking()
    {
        GameManager.Instance.CookingManager.AddCookedDish(data.DishType);
        isBusy = false;
    }

    public void SetViewer(KitchenSlotViewer viewer)
    {
        this.viewer = viewer;
    }
}
