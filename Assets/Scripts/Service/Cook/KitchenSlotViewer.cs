using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenSlotViewer : MonoBehaviour
{
    private float cookingDuration;

    [SerializeField] private TMP_Text dishName;
    [SerializeField] private Image dishImage;
    [SerializeField] private Image timeMask;

    private KitchenSlotData slotData;

    public void SetData(KitchenSlotData slotData)
    {
        cookingDuration = slotData.RemainTime;
        this.slotData = slotData;
        if (slotData == null)
        {
            Clear();
            return;
        }

        DishDataSO data = DishDataDB.GetData(slotData.DishType);

        if (data == null)
        {
            Clear();
            return;
        }

        dishName.text = data.DisplayName;
        dishImage.sprite = data.Icon;
        Refresh();
    }

    public void Refresh()
    {
        if (slotData == null)
            return;

        timeMask.fillAmount = Mathf.Clamp01(slotData.RemainTime / cookingDuration);
    }

    private void Clear()
    {
        dishName.text = "";
        timeMask.fillAmount = 0f;
    }
}
