using System;
using UnityEngine;

public class CookButton : MonoBehaviour
{
    public event Action OnClicked;

    [SerializeField] private DishType dishType;


    public void OnClick()
    {
        if (GameManager.Instance.CookingManager.TryCook(dishType))
        {
            OnClicked?.Invoke();
        }
    }
}