using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Main,
    Hub,
    Harvest,
    Service
}

public class SceneController : MonoBehaviour
{
    [SerializeField] private UI_Loading loadingPrefab;

    private Dictionary<SceneType, SceneBase> scenes;
    private SceneBase currentScene;
    private bool isChangingScene;
    private UI_Loading loading;

    public SceneType CurrentSceneType => currentScene.SceneType;

    private void Awake()
    {
        scenes = new Dictionary<SceneType, SceneBase>
        {
            { SceneType.Main, new MainScene() },
            { SceneType.Hub, new HubScene() },
            { SceneType.Harvest, new HarvestScene() },
            { SceneType.Service, new ServiceScene() }
        };

        currentScene = scenes[SceneType.Main];
    }

    private void Start()
    {
        loading = Instantiate(loadingPrefab);
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
        yield return loading.OpenLoading();

        switch (nextSceneType)
        {
            case SceneType.Hub:
                GameManager.Instance.Utility.Audio.PlayBGM(BGMType.HubBGM);
                break;
            case SceneType.Harvest:
                GameManager.Instance.Utility.Audio.PlayBGM(BGMType.HarvestBGM);
                break;
            case SceneType.Service:
                GameManager.Instance.Utility.Audio.PlayBGM(BGMType.ServiceBGM);
                break;
        }

        SceneBase nextScene = scenes[nextSceneType];
        ThreadPriority previousLoadingPriority =
            Application.backgroundLoadingPriority;
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene.SceneName);
        currentScene = nextScene;

        while (!operation.isDone)
            yield return null;

        Application.backgroundLoadingPriority = previousLoadingPriority;

        yield return currentScene.PrepareBeforeReveal();

        yield return loading.CloseLoading();

        yield return currentScene.Enter();

        isChangingScene = false;
    }
}
