using TMPro;
using UnityEngine;

public sealed class UI_FestivalCard : MonoBehaviour
{
    [SerializeField] private TMP_Text festivalText;

    public void SetData(string festivalName, int daysLeft)
    {
        festivalText.text = $"{festivalName}\n{Mathf.Max(0, daysLeft)} days left";
    }
}
