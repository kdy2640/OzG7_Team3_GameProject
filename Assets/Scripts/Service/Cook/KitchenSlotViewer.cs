using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenSlotViewer : MonoBehaviour
{
    private const float CookingDuration = 3f;

    [SerializeField] private TMP_Text dishName;
    [SerializeField] private Image timeMask;

    private KitchenSlotData slotData;

    public void SetData(KitchenSlotData slotData)
    {
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
        Refresh();
    }

    public void Refresh()
    {
        if (slotData == null)
            return;

        timeMask.fillAmount = Mathf.Clamp01(slotData.RemainTime / CookingDuration);
    }

    private void Clear()
    {
        dishName.text = "";
        timeMask.fillAmount = 0f;
    }
}
