using System.Collections;
using UnityEngine;

public sealed class MainSceneBootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        SceneController sceneController = GameManager.Instance.Scene;

        while (sceneController.IsChangingScene)
            yield return null;

        sceneController.ChangeScene(SceneType.Hub);
    }
}
