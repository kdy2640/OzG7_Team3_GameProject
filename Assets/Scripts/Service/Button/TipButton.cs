using System;
using TMPro;
using UnityEngine;

public class TipButton : MonoBehaviour
{
    public event Action OnClicked;
    [SerializeField] private TMP_Text amountText;
    public void OnClick()
    {
        OnClicked?.Invoke();
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_TipCollected);
    }

    public void SetAmountText(int amount)
    {
        amountText.text = $"{amount}";
    }
}
