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
    private bool eatSpeedApply = false;
    private bool tipChanceApply = false;
    private int level;
    private DishRequestQueue requestQueue;
    private DishEffectQueue effectQueue;
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

    public void Initialize(int level, DishRequestQueue requestQueue, DishEffectQueue effectQueue,KitchenSlotViewer prefab)
    {
        this.level = level;
        this.requestQueue = requestQueue;
        this.effectQueue = effectQueue;
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

        if(tipChanceApply)
            CustomerTipChanceUpApply(data.DishType);

        if(eatSpeedApply)
            CustomerEatSpeedUpApply(data.DishType);

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

    private void CustomerTipChanceUpApply(DishType dish)
    {
        if(UnityEngine.Random.Range(0, 2) == 0) 
            effectQueue.TipChanceUpQueue.Enqueue(dish);
    }

    private void CustomerEatSpeedUpApply(DishType dish)
    {
        if (UnityEngine.Random.Range(0, 2) == 0)
            effectQueue.EatSpeedUpQueue.Enqueue(dish);
    }

    public void TipChanceUp()
    {
        tipChanceApply = true;
    }

    public void EatSpeedUp()
    {
        eatSpeedApply = true;
    }
}
