using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TempCookHamburgerButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        GameManager.Instance.CookingManager.TryCook(DishType.Hamburger);
    }
}
