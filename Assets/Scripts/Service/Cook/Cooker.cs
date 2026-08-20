using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Cooker : MonoBehaviour
{
    
    private KitchenSlotData data;
    private KitchenSlotViewer viewer;
    private KitchenSlotViewer viewerPrefab;
    private bool isBusy = false;
    private int level;
    private DishRequestQueue requestQueue;
    public KitchenSlotData Data => data;
    public KitchenSlotViewer Viewer => viewer;

    public bool IsBusy => isBusy;


    private void Update()
    {
        if (isBusy) Cook();
    }

    public void Cook()
    {
        Data.RemainTime -= Time.deltaTime;
        viewer.Refresh();

        if (Data.RemainTime <= 0)
        {
            FinishCooking();
            Destroy(Viewer.gameObject);
        }
    }

    public void Initialize(int level, DishRequestQueue queue, KitchenSlotViewer prefab)
    {
        this.level = level;
        this.requestQueue = queue;
        this.viewerPrefab = prefab;
    }

    public void GetNextCook(KitchenSlotData data)
    {
        isBusy = true;
        data.RemainTime = CookSpeedApply(data);
        this.data = data;
        SetViewer(data);
    }
    public void FinishCooking()
    {
        GameManager.Instance.CookingManager.AddCookedDish(data.DishType);
        isBusy = false;
    }

    private void SetViewer(KitchenSlotData data)
    {
        viewer = Instantiate(viewerPrefab, transform);
        viewer.SetData(data);
    }

    private float CookSpeedApply(KitchenSlotData data)
    {
        for (int i = 0; i < level - 1; i++)
        {
            data.RemainTime /= 1.1f;
        }
        return data.RemainTime;
    }

    private IEnumerator AutoCookCo()
    {
        bool hasDish;
        
        while (true)
        {
            hasDish = false;

            if (!isBusy && requestQueue.Queue.Count > 0)
            {
                DishType requiredDish = requestQueue.Queue.Peek();
                for (int i = 0; i < GameManager.Instance.StockManager.StockData.Dishes.Count; i++)
                {
                    if (requiredDish == GameManager.Instance.StockManager.StockData.Dishes[i].dish && GameManager.Instance.StockManager.StockData.Dishes[i].amount > 0)
                    {
                        hasDish = true; break;
                    }
                }
                if (!hasDish)
                {
                    KitchenSlotData data = new KitchenSlotData(requiredDish, 3.0f);
                    GetNextCook(data);
                    requestQueue.Queue.Dequeue();
                }
            }
            
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void AutoCook()
    {
        StartCoroutine(AutoCookCo());
    }
}
