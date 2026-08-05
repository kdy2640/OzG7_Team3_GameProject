using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_SelectedMenuCard : MonoBehaviour
{
    [SerializeField] private Image menuIcon;

    [SerializeField] private TMP_Text menuNameText;

    [SerializeField] private Button button;

    public Button Button => button;

    public void SetData(MenuCardData data)
    {
        if (data == null) return;

        menuIcon.sprite = data.MenuIcon;

        menuNameText.text = data.MenuName;
    }
    public void Bind(Action onClick)
    {
        button.onClick.RemoveAllListeners();

        if (onClick != null)
            button.onClick.AddListener(() => onClick());
    }
}