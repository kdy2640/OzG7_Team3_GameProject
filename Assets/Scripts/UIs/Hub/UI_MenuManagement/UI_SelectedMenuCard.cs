using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_SelectedMenuCard : MonoBehaviour
{
    [SerializeField] private Image menuIcon;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private GameObject lockedCover;

    public void SetData(DishType dishType)
    {
        DishDataSO data = DishDataDB.GetData(dishType);
        if (data == null)
            return;

        int level = GameManager.Instance.Upgrade.RuntimeStat.Dish.GetLevel(dishType);

        menuIcon.sprite = data.Icon;
        levelText.text = $"Lv.{level}";
    }

    public void SetCover(bool setCover)
    {
        lockedCover.SetActive(setCover);
    }
}
