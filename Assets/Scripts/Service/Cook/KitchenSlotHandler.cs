using System;
using System.Collections.Generic;
using UnityEngine;

public class KitchenSlotHandler : MonoBehaviour
{
    private static readonly EmployeeType[] CookerTypes =
    {
        EmployeeType.Cooker_1,
        EmployeeType.Cooker_2,
        EmployeeType.Cooker_3
    };

    [SerializeField, Min(0f)] private float cookingTime = 3f;
    [SerializeField] private KitchenSlotViewer kitchenSlotViewerPrefab;
    [SerializeField] private Cooker cookerPrefab;
    [SerializeField] Transform QueuePanel;
    [SerializeField] Transform CookerPanel;
    [SerializeField] DishRequestQueue requestQueue;
    [SerializeField] DishEffectQueue effectQueue;
    [SerializeField] CookingList cookingList;
    [SerializeField] private int maxQueueSlots = 3;
    [SerializeField] private int maxDishAmount = 3;
    [SerializeField] private ServiceWarningMessage warningMessage;
    private CookSkillManager skillManager = new();

    private readonly List<Cooker> cookers = new();
    private readonly List<QueueSlot> queueSlots = new();

    private bool isAcceled;
    private float accelPercentage;


    private void OnEnable()
    {
        ClearDishes();

        if(requestQueue ==  null)
        {
            requestQueue = FindFirstObjectByType<DishRequestQueue>();
        }
        if(effectQueue == null)
        {
            effectQueue = FindFirstObjectByType<DishEffectQueue>();
        }
        if(cookingList == null)
        {
            cookingList = FindFirstObjectByType<CookingList>();
        }
        skillManager.Initialize(cookers);

    }

    private void Start()
    {
        for (int i = 0; i < CookerTypes.Length; i++)
        {
            if (GameManager.Instance.Upgrade.RuntimeLevel.Get(CookerTypes[i]) >= 1)
            {
                Cooker cooker = Instantiate(cookerPrefab, CookerPanel.transform);
                cooker.Initialize(GameManager.Instance.Upgrade.RuntimeLevel.Get(CookerTypes[i]), requestQueue, effectQueue, kitchenSlotViewerPrefab, cookingList);
                
                cookers.Add(cooker);
                skillManager.SkillApply(i);
            }

            else
            {
                cookers.Add(null);
            }
        }
    }

    private void Update()
    {
        ManageCookSequence();
    }
    #region Cook Sequence
    //Input
    public bool TryRequestCook(DishType dishType)
    {
        int cookingCount = 0;
        int stockAmount = 0;
        int queueCount = 0;

        foreach (DishType cookingDish in cookingList.List)
        {
            if (cookingDish == dishType)
                cookingCount++;
        }

        foreach (DishAmount stockDish in GameManager.Instance.StockManager.StockData.Dishes)
        {
            if (stockDish.dish == dishType)
            {
                stockAmount = stockDish.amount;
                break;
            }
        }

        foreach (QueueSlot queueSlot in queueSlots)
        {
            if(queueSlot.Data.DishType == dishType)
            {
                queueCount++;
            }
        }

        if (cookingCount + stockAmount + queueCount >= maxDishAmount)
        {
            warningMessage.ShowMessage(MessageType.wFullStock);
            return false;
        }

        if (queueSlots.Count >= maxQueueSlots)
        {
            warningMessage.ShowMessage(MessageType.wFullQueue);
            return false;
        }
            

        if (kitchenSlotViewerPrefab == null)
            return false;

        if (!GameManager.Instance.CookingManager.CanCook(dishType))
            return false;

        if (!GameManager.Instance.CookingManager.TryCook(dishType))
            return false;

        KitchenSlotData slotData = new(dishType, cookingTime);
        AddWaiting(slotData);

        GameManager.Instance.Utility.Audio.PlaySFX(
            SFXType.Service_CookQueued
        );

        return true;
    }

    private void AddWaiting(KitchenSlotData slotData)
    {
        QueueSlot queueSlot = new();

        KitchenSlotViewer slotViewer = Instantiate(kitchenSlotViewerPrefab, QueuePanel.transform);
        // 사이즈 줄이기
        RectTransform rect = slotViewer.GetComponent<RectTransform>();
        rect.sizeDelta *= 0.5f;

        slotViewer.SetData(slotData);
        slotViewer.transform.SetAsFirstSibling();
        queueSlot.SetData(slotData);
        queueSlot.SetViewer(slotViewer);

        queueSlots.Add(queueSlot);
    }

    private void ManageCookSequence()
    {
        Cook();
        float speedMultiplier = isAcceled ? 1f + accelPercentage : 1f;

        foreach (Cooker cooker in cookers)
        {
            if(cooker== null) continue;
            if (!cooker.IsBusy) continue;
            cooker.Cook(speedMultiplier);
        }
    }

    private void Cook()
    {
        if (queueSlots.Count <= 0)
            return;
        if (CanStartCook())
        {
            StartCook(FinishWaiting());
        }
    }

    private KitchenSlotData FinishWaiting()
    {
        KitchenSlotData data = queueSlots[0].Data;

        Destroy(queueSlots[0].Viewer.gameObject);
        queueSlots.RemoveAt(0);

        return data;
    }

    private bool CanStartCook()
    {
        for (int i = 0; i < cookers.Count; i++)
        {
            if (cookers[i] == null) continue;
            if (!cookers[i].IsBusy)
            {
                return true;
            }
        }
        return false;
    }

    private void StartCook(KitchenSlotData data)
    {
        for (int i = 0; i < cookers.Count; i++)
        {
            if (cookers[i] == null) continue;
            if (!cookers[i].IsBusy)
            {
                cookers[i].GetNextCook(data);
                break;
            }
        }
    }

    #endregion

    #region 가속
    public void Acceleration(float percentage)
    {
        accelPercentage = percentage;
        isAcceled = true;
    }

    public void Deceleration()
    {
        isAcceled = false;
    }
    #endregion

    private void ClearDishes()
    {
        GameManager.Instance.StockManager.ClearDishes();
    }
}
