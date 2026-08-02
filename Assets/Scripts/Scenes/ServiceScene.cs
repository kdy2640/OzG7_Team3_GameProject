using System.Collections;

public class ServiceScene : SceneBase
{
    public override SceneType SceneType => SceneType.Service;
    public override string SceneName => "ServiceScene";

    public override IEnumerator PrepareBeforeReveal()
    {
        GameManager.Instance.Service.PrepareReveal();
        yield return null;
    }

    public override IEnumerator Enter()
    {
        GameManager.Instance.Service.StartLoop();
        yield return null;
    }

    public override IEnumerator Exit()
    {
        GameManager.Instance.Service.EndLoop();
        yield return null;
    }
}
