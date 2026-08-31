using System.Collections.Generic;
using UnityEngine;

public static class TutorialDataDB
{
    private const string LoadPath = "SOs/TutorialDataSO";

    private static Dictionary<TutorialManager.TutorialType, TutorialDataSO>
        tutorialDataMap;

    public static TutorialDataSO GetData(
        TutorialManager.TutorialType tutorialType)
    {
        if (!TryGetData(tutorialType, out TutorialDataSO data))
        {
            Debug.LogWarning(
                $"There is no TutorialDataSO. tutorialType : {tutorialType}");
        }

        return data;
    }

    public static bool TryGetData(
        TutorialManager.TutorialType tutorialType,
        out TutorialDataSO data)
    {
        Initialize();
        return tutorialDataMap.TryGetValue(tutorialType, out data);
    }

    private static void Initialize()
    {
        if (tutorialDataMap != null)
            return;

        tutorialDataMap =
            new Dictionary<TutorialManager.TutorialType, TutorialDataSO>();

        TutorialDataSO[] resources =
            Resources.LoadAll<TutorialDataSO>(LoadPath);

        foreach (TutorialDataSO data in resources)
        {
            if (data == null)
                continue;

            if (data.tutorialType == TutorialManager.TutorialType.Length)
            {
                Debug.LogWarning(
                    $"{data.name} TutorialDataSO tutorialType is Length.");
                continue;
            }

            if (tutorialDataMap.ContainsKey(data.tutorialType))
            {
                Debug.LogWarning(
                    $"TutorialDataSO tutorialType duplication. tutorialType : {data.tutorialType}, SO Name : {data.name}");
                continue;
            }

            tutorialDataMap.Add(data.tutorialType, data);
        }
    }
}
