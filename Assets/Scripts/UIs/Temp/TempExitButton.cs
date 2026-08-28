using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TempExitButton : MonoBehaviour
{  
    [SerializeField] private Button button;
    public void OnButtonClicked()
    { 
        Application.Quit();
    } 
    private void Awake()
    {
        if (button == null) button = gameObject.GetOrAddComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    } 
}
