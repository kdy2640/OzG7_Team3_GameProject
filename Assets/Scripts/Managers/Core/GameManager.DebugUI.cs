#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using UnityEngine;

public partial class GameManager
{
    private IEnumerator Start()
    {
        yield return null;

        StockManager.RegisterDebugUI();
        Upgrade.RegisterDebugUI();
        Market.RegisterDebugUI();
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        StockManager.UnregisterDebugUI();
        Upgrade.UnregisterDebugUI();
        Market.UnregisterDebugUI();
    }
}
#endif
