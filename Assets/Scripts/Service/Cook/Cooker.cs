using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private CookingList cookingList;
    public KitchenSlotData Data => data;
    public KitchenSlotViewer Viewer => viewer;

    [SerializeField] Image autoCookingImg;

    private bool isAutoCooking = false;
    
    public bool IsBusy => isBusy;


    private void Start()
    {
        StartCoroutine(AutoCookUIUpdateCo());
    }

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

    public void Initialize(int level, DishRequestQueue requestQueue, DishEffectQueue effectQueue,KitchenSlotViewer prefab, CookingList cookingList)
    {
        this.level = level;
        this.requestQueue = requestQueue;
        this.effectQueue = effectQueue;
        this.viewerPrefab = prefab;
        this.cookingList = cookingList;
    }

    public void GetNextCook(KitchenSlotData data)
    {
        isBusy = true;
        cookingList.Add(data.DishType);
        data.RemainTime = CookSpeedApply(data);
        this.data = data;
        SetViewer(data);
    }
    public void FinishCooking()
    {
        cookingList.Remove(data.DishType);
        GameManager.Instance.CookingManager.AddCookedDish(data.DishType);
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_CookComplete);

        if(tipChanceApply)
            CustomerTipChanceUpApply(data.DishType);

        if(eatSpeedApply)
            CustomerEatSpeedUpApply(data.DishType);
        isAutoCooking = false;
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
                if (!GameManager.Instance.CookingManager.CanCook(requiredDish))
                    break;
                for (int i = 0; i < GameManager.Instance.StockManager.StockData.Dishes.Count; i++)
                {
                    if (requiredDish == GameManager.Instance.StockManager.StockData.Dishes[i].dish 
                        && GameManager.Instance.StockManager.StockData.Dishes[i].amount > 0)
                    {
                        hasDish = true; break;
                    }
                }
                if (!hasDish)
                {
                    KitchenSlotData data = new KitchenSlotData(requiredDish, 3.0f);
                    isAutoCooking = true;
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

    private IEnumerator AutoCookUIUpdateCo()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            if(isAutoCooking)
            {
                autoCookingImg.gameObject.SetActive(true);
            }
            else
            {
                autoCookingImg.gameObject.SetActive(false);
            }
        }
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
