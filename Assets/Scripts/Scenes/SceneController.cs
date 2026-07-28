using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Main,
    Upgrade,
    GameLoop
}

public class SceneController : MonoBehaviour
{
    private Dictionary<SceneType, SceneBase> scenes;
    private SceneBase currentScene;
    private bool isChangingScene;

    public SceneType CurrentSceneType => currentScene.SceneType;

    private void Awake()
    {
        scenes = new Dictionary<SceneType, SceneBase>
        {
            { SceneType.Main, new MainScene() },
            { SceneType.Upgrade, new UpgradeScene() },
            { SceneType.GameLoop, new GameLoopScene() }
        };

        currentScene = scenes[SceneType.Main];
    }

    public void ChangeScene(SceneType nextSceneType, bool isForced = false)
    {
        if (isChangingScene)
            return;

        if (currentScene.SceneType == nextSceneType && !isForced)
            return;

        StartCoroutine(ChangeSceneRoutine(nextSceneType));
    }

    public void RestartScene(SceneType nextSceneType)
    {
        if (isChangingScene)
            return;

        if (currentScene.SceneType == nextSceneType)
            StartCoroutine(ChangeSceneRoutine(nextSceneType));
    }

    private IEnumerator ChangeSceneRoutine(SceneType nextSceneType)
    {
        isChangingScene = true;

        yield return currentScene.Exit();

        SceneBase nextScene = scenes[nextSceneType];
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene.SceneName);
        currentScene = nextScene;

        while (!operation.isDone)
            yield return null;

        yield return currentScene.PrepareBeforeReveal();
        yield return currentScene.Enter();

        isChangingScene = false;
    }
}
