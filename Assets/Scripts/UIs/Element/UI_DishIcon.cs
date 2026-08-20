using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class UI_DishIcon : MonoBehaviour
{
    [SerializeField] private Image dishImage;

    private DishType dishType = DishType.None;

    public DishType DishType => dishType;

    private void Awake()
    {
        if (dishImage == null)
            dishImage = GetComponent<Image>();
    }

    public void SetData(DishType dishType)
    {
        this.dishType = dishType;

        if (dishImage == null)
            dishImage = GetComponent<Image>();

        if (dishType == DishType.None
            || dishType == DishType.Count
            || !DishDataDB.TryGetData(dishType, out DishDataSO dishData))
        {
            dishImage.sprite = null;
            return;
        }

        dishImage.sprite = dishData.Icon;
    }
}
