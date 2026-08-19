using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestEndSequence : MonoBehaviour
{
    [SerializeField] private EndUI endUI;
    [SerializeField] private ResultUI resultUI;

    private readonly int[] stockBeforeLoop = new int[(int)GroceryType.Count];
    private readonly List<GroceryAmount> sessionResult = new();
    private bool isEnding;

    private void Start()
    {
        HarvestManager harvestManager = GameManager.Instance?.Harvest;

        if (harvestManager == null)
        {
            Debug.LogError("[HarvestEndSequence] HarvestManager를 찾을 수 없습니다.", this);
            return;
        }

        harvestManager.Events.Subscribe(HarvestEventType.LoopStarted, HandleLoopStarted);
        harvestManager.Events.Subscribe(HarvestEventType.LoopEnded, HandleLoopEnded);

        if (endUI != null)
            endUI.gameObject.SetActive(false);

        if (resultUI != null)
            resultUI.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        HarvestManager harvestManager = GameManager.Instance?.Harvest;

        if (harvestManager == null)
            return;

        harvestManager.Events.Unsubscribe(HarvestEventType.LoopStarted, HandleLoopStarted);
        harvestManager.Events.Unsubscribe(HarvestEventType.LoopEnded, HandleLoopEnded);
    }

    private void HandleLoopStarted()
    {
        CaptureStock(stockBeforeLoop);
    }

    private void HandleLoopEnded()
    {
        if (isEnding)
            return;

        sessionResult.Clear();
        int[] stockAfterLoop = new int[(int)GroceryType.Count];
        CaptureStock(stockAfterLoop);

        for (int i = 0; i < stockAfterLoop.Length; i++)
        {
            int earnedAmount = Mathf.Max(0, stockAfterLoop[i] - stockBeforeLoop[i]);

            if (earnedAmount > 0)
                sessionResult.Add(new GroceryAmount((GroceryType)i, earnedAmount));
        }

        StartCoroutine(PlayEndSequence());
    }

    private IEnumerator PlayEndSequence()
    {
        isEnding = true;

        if (endUI != null)
        {
            endUI.gameObject.SetActive(true);
            yield return endUI.PlayRoutine();
        }

        if (resultUI != null)
        {
            resultUI.SetData(sessionResult);
            yield return resultUI.Show();
        }
        else
        {
            Debug.LogError("[HarvestEndSequence] ResultUI가 연결되지 않았습니다.", this);
        }
    }

    private static void CaptureStock(int[] destination)
    {
        System.Array.Clear(destination, 0, destination.Length);

        IReadOnlyList<GroceryAmount> groceries =
            GameManager.Instance?.StockManager?.StockData?.Groceries;

        if (groceries == null)
            return;

        for (int i = 0; i < groceries.Count; i++)
        {
            GroceryAmount groceryAmount = groceries[i];

            if (groceryAmount == null)
                continue;

            int index = (int)groceryAmount.grocery;

            if (index < 0 || index >= destination.Length)
                continue;

            destination[index] += Mathf.Max(0, groceryAmount.amount);
        }
    }
}
