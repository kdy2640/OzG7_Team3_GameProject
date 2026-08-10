using Unity.VisualScripting;
using UnityEngine;

public class QueueSlotContainer : MonoBehaviour
{
    [SerializeField] QueueSlot queueSlotPrefab;
    [SerializeField] CookingQueue cookingQueue;

    private void Awake()
    {
        cookingQueue = GetComponent<CookingQueue>();
    }

    private void OnEnable()
    {
        cookingQueue.queueChanged += UpdateUI;
    }

    private void UpdateUI()
    {
        Clear();
        foreach (var dish in cookingQueue.DishesQueue)
        {
            if(dish == DishType.Count)
            {
                continue;
            }
            QueueSlot slot = Instantiate(queueSlotPrefab, transform);
            slot.SetDish(dish);
        }
    }

    private void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDisable()
    {
        cookingQueue.queueChanged -= UpdateUI;
    }
}
