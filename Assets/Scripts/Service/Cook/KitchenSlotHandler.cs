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

    private CookSkillManager skillManager = new();

    private readonly List<Cooker> cookers = new();
    private readonly List<QueueSlot> queueSlots = new();

    private void OnEnable()
    {
        if(requestQueue ==  null)
        {
            requestQueue = FindFirstObjectByType<DishRequestQueue>();
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
                cooker.Initialize(GameManager.Instance.Upgrade.RuntimeLevel.Get(CookerTypes[i]), requestQueue, kitchenSlotViewerPrefab);
                
                cookers.Add(cooker);
                skillManager.SkillApply(i);
            }
            if (cookers.Count <= 0)
            {
                cookers.Add(Instantiate(cookerPrefab, CookerPanel.transform));
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

        KitchenSlotViewer slotViewer = Instantiate(kitchenSlotViewerPrefab, QueuePanel.transform);
        slotViewer.SetData(slotData);
        slotViewer.transform.SetAsFirstSibling();
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
            cooker.Cook();
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
                break;
            }
        }
    }

    #endregion
}
