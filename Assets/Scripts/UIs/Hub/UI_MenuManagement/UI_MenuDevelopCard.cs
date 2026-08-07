using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MenuDevelopStatus
{
    Opened,
    CanOpen,
    Locked
}

public class UI_MenuDevelopCard : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Image menuIcon;

    [SerializeField] private TMP_Text menuNameText;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private TMP_Text cookText;

    [SerializeField] private TMP_Text priceText;

    [SerializeField] private GameObject canOpen;

    [SerializeField] private GameObject lockOverlay;

    [SerializeField] private Color developedColor = Color.yellow;

    [SerializeField] private Color lockedColor = Color.gray;

    private DishType dishType;
    public DishType DishType => dishType;

    public void SetData(DishType dishType)
    {
        DishDataSO data = DishDataDB.GetData(dishType);
        if (data == null)
            return;

        this.dishType = dishType;

        int level = GameManager.Instance.Upgrade.RuntimeStat.Dish.GetLevel(dishType);

        menuIcon.sprite = data.Icon;

        menuNameText.text = data.DisplayName;

        levelText.text = $"Lv.{level}";

        int cookableAmount = GameManager.Instance.CookingManager
            .CalculateCookableAmount(dishType);

        cookText.text = $"C {cookableAmount:N0}";
        priceText.text = $"G {data.Cost:N0}";
    }

    public void SetStatus(MenuDevelopStatus status)
    {
        bool isOpened = status == MenuDevelopStatus.Opened;

        canOpen.SetActive(status == MenuDevelopStatus.CanOpen);
        lockOverlay.SetActive(!isOpened);

        backgroundImage.color = isOpened
            ? developedColor : lockedColor;
    }
}
