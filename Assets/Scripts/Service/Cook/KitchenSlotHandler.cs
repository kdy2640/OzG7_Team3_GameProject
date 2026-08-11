using System.Collections.Generic;
using UnityEngine;

public class KitchenSlotHandler : MonoBehaviour
{
    private static readonly EmployeeType[] ServerTypes =
    {
        EmployeeType.Server_1,
        EmployeeType.Server_2,
        EmployeeType.Server_3,
        EmployeeType.Server_4
    };

    [SerializeField] private int concurrentCookingCount;
    [SerializeField, Min(0f)] private float cookingTime = 3f;
    [SerializeField] private KitchenSlotViewer kitchenSlotViewerPrefab;

    private readonly List<KitchenSlotData> slotDatas = new();
    private readonly List<KitchenSlotViewer> slotViewers = new();

    private void Start()
    {
        //for (int i = 0; i < ServerTypes.Length; i++)
        //{
        //    if (GameManager.Instance.Upgrade.GetLevel(ServerTypes[i]) >= 1)
        //        concurrentCookingCount++;
        //}
        concurrentCookingCount = 2;
    }

    public bool TryRequestCook(DishType dishType)
    {
        if (kitchenSlotViewerPrefab == null)
            return false;

        if (!GameManager.Instance.CookingManager.TryCook(dishType))
            return false;

        KitchenSlotData slotData = new(dishType, cookingTime);
        AddSlot(slotData);
        return true;
    }

    private void AddSlot(KitchenSlotData slotData)
    {
        KitchenSlotViewer slotViewer = Instantiate(kitchenSlotViewerPrefab, transform);
        slotViewer.SetData(slotData);

        slotDatas.Add(slotData);
        slotViewers.Add(slotViewer);
    }

    private void Update()
    {
        int cookingCount = Mathf.Min(concurrentCookingCount, slotDatas.Count);

        for (int i = 0; i < cookingCount; i++)
        {
            KitchenSlotData slotData = slotDatas[i];
            slotData.RemainTime = Mathf.Max(0f, slotData.RemainTime - Time.deltaTime);
            slotViewers[i].Refresh();
        }

        for (int i = cookingCount - 1; i >= 0; i--)
        {
            if (slotDatas[i].RemainTime > 0f)
                continue;

            FinishCooking(i);
        }
    }

    private void FinishCooking(int index)
    {
        DishType dishType = slotDatas[index].DishType;

        Destroy(slotViewers[index].gameObject);
        slotDatas.RemoveAt(index);
        slotViewers.RemoveAt(index);

        GameManager.Instance.CookingManager.AddCookedDish(dishType);
    }
}
