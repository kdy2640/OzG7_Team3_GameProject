using TMPro;
using UnityEngine;

public class KitchenSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text timerText;

    private DishType currentDish = DishType.Count;
    private float timer;

    public bool IsEmpty => currentDish == DishType.Count;

    public void Initialize()
    {
        Clear();
    }

    public void StartCooking(DishType dish)
    {
        currentDish = dish;
        timer = 3f;
        
        UpdateUI();
    }

    private void Update()
    {
        if (IsEmpty) return;

        timer -= Time.deltaTime;

        UpdateTimerUI();
         
        if(timer <= 0)
        {
            timer = 0;
            FinishCooking();
        }
    }


    private void FinishCooking()
    {
        DishType finishedDish = currentDish;

        currentDish = DishType.Count;

        Clear();

        GameManager.Instance.CookingManager.AddCookedDish(finishedDish);
    }



    private void UpdateUI()
    {
        DishDataSO data = DishDataDB.GetData(currentDish);

        if (data == null)
            return;

        dishName.text = data.DisplayName;
    }

    private void UpdateTimerUI()
    {
        timerText.text = timer.ToString("F0") + "s";
    }

    private void Clear()
    {
        dishName.text = "";
        timerText.text = "";
    }
}
