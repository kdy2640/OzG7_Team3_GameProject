using System.Collections.Generic;
using UnityEngine;

public enum HarvestType
{
    Rice,
    Carrot,
    Chicken,
    Wheat,
    Onion,
    Cow,
    Potato,
    Cabbage,
    Sheep,
    Corn,
    Tomato,
    Grape,
    Pig,
    Count
}

[System.Serializable]
public sealed class HarvestAnimalStat
{
    [SerializeField, Min(0f)] private float detectionRange = 3f;
    [SerializeField, Min(0f)] private float fleeDistance = 4f;
    [SerializeField, Min(0f)] private float patrolSpeed = 1.25f;
    [SerializeField, Min(0f)] private float fleeSpeed = 2.5f;

    public float DetectionRange => detectionRange;
    public float FleeDistance => fleeDistance;
    public float PatrolSpeed => patrolSpeed;
    public float FleeSpeed => fleeSpeed;
}

[CreateAssetMenu(menuName = "Game/HarvestDataSO")]
public sealed class HarvestDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private HarvestType harvestType = HarvestType.Count;
    [SerializeField] private List<GroceryAmount> rewards = new();
    [SerializeField] private bool isMove;
    [SerializeField] private HarvestAnimalStat animalStat = new();
    [SerializeField] private RuntimeAnimatorController animatorController;
    [SerializeField] private GameObject solidPrefab;
    [SerializeField] private GameObject itemPrefab;

    public string Id => id;
    public HarvestType HarvestType => harvestType;
    public List<GroceryAmount> Rewards => rewards;
    public bool IsMove => isMove;
    public HarvestAnimalStat AnimalStat => animalStat;
    public RuntimeAnimatorController AnimatorController => animatorController;
    public GameObject SolidPrefab => solidPrefab;
    public GameObject ItemPrefab => itemPrefab;
}
