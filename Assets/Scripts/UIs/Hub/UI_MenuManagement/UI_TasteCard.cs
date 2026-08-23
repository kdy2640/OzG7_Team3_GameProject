using TMPro;
using UnityEngine;

public sealed class UI_TasteCard : MonoBehaviour
{
    [SerializeField] private TMP_Text tasteText;

    public void SetData(TasteType tasteType)
    {
        tasteText.text = tasteType.ToString();
    }

    public void SetData(CategoryType categoryType)
    {
        tasteText.text = categoryType.ToString();
    }
}
