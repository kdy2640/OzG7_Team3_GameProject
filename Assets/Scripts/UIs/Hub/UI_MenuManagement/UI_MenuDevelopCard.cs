using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuDevelopCard : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Image menuIcon;

    [SerializeField] private TMP_Text menuNameText;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private TMP_Text priceText;

    [SerializeField] private Button button;

    [SerializeField] private Color developedColor = Color.yellow;

    [SerializeField] private Color lockedColor = Color.gray;

    public void SetData(MenuCardData data)
    {
        if (data == null) return;

        menuIcon.sprite = data.MenuIcon;

        menuNameText.text = data.MenuName;

        levelText.text = $"Lv.{data.Level}";

        priceText.text = $"{data.Price:N0} G";

        backgroundImage.color = data.IsDeveloped
            ? developedColor : lockedColor;
    }
    public void Bind(Action onClick)
    {
        button.onClick.RemoveAllListeners();

        if (onClick != null)
            button.onClick.AddListener(() => onClick());
    }
}
