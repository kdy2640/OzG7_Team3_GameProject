using System;
using TMPro;
using UnityEngine;

public class CookSlot : MonoBehaviour
{
    public event Action OnClicked;

    [SerializeField] private DishType dishType;
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text stateText;
    
    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        DishDataSO data = DishDataDB.GetData(dishType);

        if (data == null) return;

        dishName.text = data.DisplayName;
        stateText.text = "No\nGrocery";
        stateText.enabled = false;
        bool canCook = GameManager.Instance.CookingManager.CanCook(dishType);

        
        if ( !canCook )
        {
            dishName.enabled = false;
            stateText.enabled = true;
        }
    }

    public void OnClick()
    {
        if (GameManager.Instance.CookingManager.TryCook(dishType))
        {
            UpdateUI();
            OnClicked?.Invoke();
        }
    }
}