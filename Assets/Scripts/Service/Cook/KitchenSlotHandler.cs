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
    
    private readonly List<Cooker> cookers = new();
    private readonly List<QueueSlot> queueSlots = new();

    private void Start()
    {
        for (int i = 0; i < CookerTypes.Length; i++)
        {
            if (GameManager.Instance.Upgrade.RuntimeLevel.Get(CookerTypes[i]) >= 1)
            {
                Cooker cooker = new Cooker();
                cookers.Add(cooker);
            }
        }
        if(cookers.Count <= 0)
        {
            cookers.Add(new Cooker());
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
        if (kitchenSlotViewerPrefab == null)
            return false;

        if (!GameManager.Instance.CookingManager.CanCook(dishType))
            return false;

        if (!GameManager.Instance.CookingManager.TryCook(dishType))
            return false;

        KitchenSlotData slotData = new(dishType, cookingTime);
        AddWaiting(slotData);
        return true;
    }

    private void AddWaiting(KitchenSlotData slotData)
    {
        QueueSlot queueSlot = new();

        KitchenSlotViewer slotViewer = Instantiate(kitchenSlotViewerPrefab, transform);
        slotViewer.SetData(slotData);

        queueSlot.SetData(slotData);
        queueSlot.SetViewer(slotViewer);

        queueSlots.Add(queueSlot);
    }

    private void ManageCookSequence()
    {
        Cook();
        foreach (Cooker cooker in cookers)
        {
            if (!cooker.IsBusy) continue;

            cooker.Data.RemainTime -= Time.deltaTime;
            cooker.Viewer.Refresh();

            if (cooker.Data.RemainTime <= 0)
            {
                cooker.FinishCooking();
                Destroy(cooker.Viewer.gameObject);
            }
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
            if (!cookers[i].IsBusy)
            {
                cookers[i].GetNextCook(data);
                KitchenSlotViewer viewer = Instantiate(kitchenSlotViewerPrefab, transform);
                cookers[i].SetViewer(viewer);
                break;
            }
        }
    }

    #endregion


    private void SkillApply(int index)
    {
        // 2~4 레벨 요리속도 10% 증가
        for(int i = 1; i < GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)index) -1 ;i++)
        {
            cookers[index].Data.RemainTime /= 1.1f;
        }
        switch (index)
        {
            case 0:
                if(GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)index) >= 3)
                {
                    //AutoCook();
                }
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)index) >= 5)
                {
                    //TipChanceUp();
                }
                return;
            case 1:
                //AutoCook();
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)index) >= 3)
                {
                    //CustomerEatSpeedUp();
                }
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)index) >= 5)
                {
                    //TipChanceUp();
                }
                return;
            case 2:
                //CustomerEatSpeedUp();
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)index) >= 3)
                {
                    //AutoCook();
                }
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)index) >= 5)
                {
                    //TipChanceUp();
                }
                return;
            default: return;
        }
    }

}
