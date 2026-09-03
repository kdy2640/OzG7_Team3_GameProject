using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(GroceryPooler))]
public sealed class GroceryGainRoutine : MonoBehaviour
{
    private const int TargetCount = 3;

    [SerializeField] private Transform[] targets = new Transform[TargetCount];
    [SerializeField] private UI_GroceryViewPanel groceryViewPanel;
    [SerializeField, Min(0)] private int prewarmCount = 20;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float spawnDelay = 0.12f;
    [SerializeField, Min(0f)] private float moveDelay = 0.3f;
    [SerializeField, Min(1)] private int amountPerPresenter = 1;

    private readonly int[] nowAliveGrocery =
        new int[(int)GroceryType.Count];
    private GroceryPooler pooler;

    private void Awake()
    {
        pooler = GetComponent<GroceryPooler>();
        pooler.Prewarm(prewarmCount);
        groceryViewPanel.SetDelayRefresh(true);
    }

    public void Play(
        IReadOnlyList<GroceryAmount> rewards,
        Vector3 spawnPoint)
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            GroceryAmount reward = rewards[i];
            nowAliveGrocery[(int)reward.grocery] += reward.amount;
        }

        StartCoroutine(PlayRoutine(rewards, spawnPoint));
    }

    private IEnumerator PlayRoutine(
        IReadOnlyList<GroceryAmount> rewards,
        Vector3 spawnPoint)
    {
        int amountPerVisual = Mathf.Max(1, amountPerPresenter);

        for (int i = 0; i < rewards.Count; i++)
        {
            GroceryAmount reward = rewards[i];
            int remainAmount = reward.amount;

            while (remainAmount > 0)
            {
                int splitAmount = Mathf.Min(
                    amountPerVisual,
                    remainAmount);
                remainAmount -= splitAmount;

                GroceryAmount separatedReward = new(
                    reward.grocery,
                    splitAmount);

                StartCoroutine(ApplyRoutine(
                    spawnPoint,
                    separatedReward));

                yield return new WaitForSeconds(
                    spawnDelay * Random.value);
            }
        }
    }

    private IEnumerator ApplyRoutine(
        Vector3 spawnPoint,
        GroceryAmount reward)
    {
        GroceryArgs args = new(
            spawnPoint,
            reward.grocery);
        GroceryPresenter presenter = pooler.Get(args);

        yield return presenter.PopUpRoutine();
        yield return new WaitForSeconds(
            moveDelay * Random.value);

        int targetIndex = GetTargetIndex(reward.grocery);
        yield return presenter.MoveToTarget(targets[targetIndex]);

        int groceryIndex = (int)reward.grocery;
        nowAliveGrocery[groceryIndex] -= reward.amount;

        groceryViewPanel.RefreshOneUI(
            reward,
            nowAliveGrocery[groceryIndex] == 0);
    }

    private int GetTargetIndex(GroceryType groceryType)
    {
        return (int)groceryType % TargetCount;
    }
}
