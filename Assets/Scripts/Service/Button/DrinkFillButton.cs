using System;
using UnityEngine;
using UnityEngine.UI;

public class DrinkFillButton : MonoBehaviour
{
    public event Action Filled;
    private Button button;
    private float fillAmount = 0;

    public float FillAmount => fillAmount;
    public Button Button => button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        ResetFillAmount();
        button.image.fillAmount = fillAmount;
    }
    
    public void OnClick()
    {
        fillAmount += 0.25f;
        button.image.fillAmount = fillAmount;
        if (fillAmount >= 1.0f)
        {
            Filled?.Invoke();
            ResetFillAmount();
            this.gameObject.SetActive(false);
        }
    }

    public void ResetFillAmount()
    {
        fillAmount = 0;
    }
}
