using System.Collections;
using UnityEngine;

public sealed class MainSceneBootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        GameManager.Instance.Scene.ChangeScene(SceneType.Hub);
    }
}
