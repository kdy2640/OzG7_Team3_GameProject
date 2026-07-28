using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialType
    {
        BeforeGameLoop, 
        Length
    }

    private Dictionary<TutorialType, bool> tutorialProgressionMap = new();

    private void Awake()
    {
        Initialize();
    }

    public bool GetTutorialProgressed(TutorialType type)
    {
        if (type < 0 || type >= TutorialType.Length)
            return false;

        return tutorialProgressionMap[type];
    }

    public void ResolveTutorial(TutorialType type)
    {
        if (type < 0 || type >= TutorialType.Length)
        {
            Debug.LogError($"Invalid tutorial type: {type}");
            return;
        }

        tutorialProgressionMap[type] = true;
    }

    public void Initialize()
    {
        tutorialProgressionMap = new Dictionary<TutorialType, bool>();

        for (int i = 0; i < (int)TutorialType.Length; i++)
            tutorialProgressionMap.Add((TutorialType)i, false);
    }

#if UNITY_EDITOR
    [ContextMenu("Debug/Log Tutorial Progress")]
    private void LogTutorialProgress()
    {
        foreach (KeyValuePair<TutorialType, bool> pair in tutorialProgressionMap)
            Debug.Log($"{pair.Key}: {pair.Value}");
    }
#endif
}
