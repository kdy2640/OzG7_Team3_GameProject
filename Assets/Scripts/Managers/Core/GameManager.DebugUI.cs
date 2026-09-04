using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public partial class GameManager
{
    private IEnumerator Start()
    {
        if (!Debug.isDebugBuild)
        {
            DebugManager debugManager = DebugManager.instance;
            BindingFlags methodFlags =
                BindingFlags.Instance | BindingFlags.NonPublic;

            typeof(DebugManager)
                .GetMethod("RegisterInputs", methodFlags)
                .Invoke(debugManager, null);
            typeof(DebugManager)
                .GetMethod("RegisterActions", methodFlags)
                .Invoke(debugManager, null);

            debugManager.enableRuntimeUI = false;
            debugManager.enableRuntimeUI = true;
        }

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
