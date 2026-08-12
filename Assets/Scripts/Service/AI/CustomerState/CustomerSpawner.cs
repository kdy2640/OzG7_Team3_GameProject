using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private CustomerStateManager customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private RuntimeAnimatorController controller;

    [Header("랜덤 동물 범위")]
    [SerializeField] List<GameObject> animalPrefabs = new();

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCustomer();
            timer = 0f;
        }
    }

    private void SpawnCustomer()
    {
        CustomerStateManager customer =
            Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        GameObject animal =
            Instantiate(animalPrefabs[Random.Range(0, animalPrefabs.Count)], customer.transform);
        animal.transform.localScale = Vector3.one * 0.5f;
        animal.transform.localPosition += Vector3.down * 0.5f;
        
        Animator animator = animal.GetComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        customer.SetAnimator(animator);
    }
}