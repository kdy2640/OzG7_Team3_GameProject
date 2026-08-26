using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private CustomerStateManager customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private DishRequestQueue requestQueue;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private TipBox tipBox;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int spawnCount;
    [SerializeField] private RuntimeAnimatorController controller;
    [SerializeField] private Dirty dirtyPrefab;
    [SerializeField] private Combo combo;


    [Header("랜덤 동물 범위")]
    [SerializeField] List<GameObject> animalPrefabs = new();
    [SerializeField] float animalSize;

    private float timer;
    private bool serviceEnd;
    private Action endService;


    private void Awake()
    {
        if (exitPoint == null)
        {
            exitPoint = transform;
        }
        if (tableManager == null)
        {
            tableManager = FindFirstObjectByType<TableManager>();
        }
        if(requestQueue == null)
        {
            requestQueue = FindFirstObjectByType<DishRequestQueue>();
        }
    }

    private void OnEnable()
    {
        endService += ServiceEnd;
        GameManager.Instance.Service.Events.Subscribe(ServiceEventType.LoopEnded, endService);
    }

    private void Start()
    {
        spawnCount = (int)GameManager.Instance.Upgrade.RuntimeStat.Service.Get(ServiceStatType.CustomerCount);
        spawnInterval = 100 / spawnCount;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && spawnCount > 0 && !serviceEnd)
        {
            SpawnCustomer();
            spawnCount--;
            timer = 0f;
        }
    }

    private void SpawnCustomer()
    {
        CustomerStateManager customer =
            Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);

        GameObject animal =
            Instantiate(animalPrefabs[UnityEngine.Random.Range(0, animalPrefabs.Count)], customer.transform);

        customer.Initialize(exitPoint, tableManager, tipBox, requestQueue, dirtyPrefab, combo);
        animal.transform.localScale = Vector3.one * animalSize;
        animal.transform.localPosition += Vector3.down * (1 - animalSize) * 2;
        
        Animator animator = animal.GetComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        customer.SetAnimator(animator);
        GameManager.Instance.Service.ResultBuilder.RecordCustomer();
    }
    
    private void ServiceEnd()
    {
        serviceEnd = true;
    }

    private void OnDisable()
    {
        endService -= ServiceEnd;
        GameManager.Instance.Service.Events.Unsubscribe(ServiceEventType.LoopEnded, endService);
    }
}
