using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TempGetMoney : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null) button = gameObject.GetOrAddComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        GameManager.Instance.CurrencyManager.Add(100);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
