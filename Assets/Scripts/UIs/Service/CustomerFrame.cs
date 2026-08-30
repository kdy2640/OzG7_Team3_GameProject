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
        
    }

    private void Start()
    {
        UpdateUI();
    }
    private void UpdateUI()
    {
        leftCustomer.text = "남은 손님 <size=50>"+ customerSpawner.SpawnCount + "</size>";
    }

    private void OnDisable()
    {
        customerSpawner.CustomerSpawned -= UpdateUI;
    }
}
