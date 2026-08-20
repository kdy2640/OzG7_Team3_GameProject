using System.Collections;
using UnityEngine;

public class ServiceScene : SceneBase
{
    public override SceneType SceneType => SceneType.Service;
    public override string SceneName => "ServiceScene";

    public override IEnumerator PrepareBeforeReveal()
    {
        ServicePreloader preloader =
            Object.FindFirstObjectByType<ServicePreloader>();

        if (preloader == null)
        {
            Debug.LogError("[ServiceScene] ServicePreloader를 찾을 수 없습니다.");
            yield break;
        }

        yield return preloader.Run();
        GameManager.Instance.Service.PrepareReveal();
    }

    public override IEnumerator Enter()
    {
        ServiceStartSequence startSequence =
            Object.FindFirstObjectByType<ServiceStartSequence>();

        if (startSequence == null)
        {
            Debug.LogError("[ServiceScene] ServiceStartSequence를 찾을 수 없습니다.");
            yield break;
        }

        yield return startSequence.Run();
        GameManager.Instance.Service.StartLoop();
    }

    public override IEnumerator Exit()
    {
        ServiceEndSequence endSequence =
            Object.FindFirstObjectByType<ServiceEndSequence>();

        if (endSequence == null)
        {
            Debug.LogError("[ServiceScene] ServiceEndSequence를 찾을 수 없습니다.");
            yield break;
        }

        yield return endSequence.Run(
            GameManager.Instance.Service.LastSalesResult);

        GameManager.Instance.Market.MoveToNextPhase();
    }
}
