using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TempUpgrade : MonoBehaviour
{
    [SerializeField] private UpgradeDataSO upgradeData;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null) button = gameObject.GetOrAddComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        GameManager.Instance.Upgrade.TryUpgrade(upgradeData);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
