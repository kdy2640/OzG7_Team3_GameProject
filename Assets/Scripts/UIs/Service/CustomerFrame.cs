using TMPro;
using UnityEngine;

public class CustomerFrame : MonoBehaviour
{
    [SerializeField] TMP_Text leftCustomer;
    private CustomerSpawner customerSpawner;

    private void OnEnable()
    {
        customerSpawner = FindFirstObjectByType<CustomerSpawner>();
        customerSpawner.CustomerSpawned += UpdateUI;
        GameManager.Instance.Service.Events.Subscribe(
            ServiceEventType.LoopStarted,
            UpdateUI); 
    }
    private void Start()
    { 
        leftCustomer.text = "남은 손님 <size=50>" + Mathf.RoundToInt(
           GameManager.Instance.Upgrade.RuntimeStat.Service
               .Get(ServiceStatType.CustomerCount)) + "</size>";
    }

    private void UpdateUI()
    {
        leftCustomer.text = "남은 손님 <size=50>"+ customerSpawner.SpawnCount + "</size>";
    }

    private void OnDisable()
    {
        customerSpawner.CustomerSpawned -= UpdateUI;
        GameManager.Instance.Service.Events.Unsubscribe(
            ServiceEventType.LoopStarted,
            UpdateUI);
    }
}
