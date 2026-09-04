using System.Collections;
using UnityEngine;

public sealed class MainSceneBootstrap : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    private IEnumerator Start()
    {
        yield return null;

        SceneController sceneController = GameManager.Instance.Scene;

        while (sceneController.IsChangingScene)
            yield return null;

        sceneController.ChangeScene(SceneType.Hub);
    }
}
