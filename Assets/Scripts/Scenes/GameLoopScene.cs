using System.Collections;

public class GameLoopScene : SceneBase
{
    public override SceneType SceneType => SceneType.GameLoop;
    public override string SceneName => "GameLoopScene";

    public override IEnumerator PrepareBeforeReveal()
    {
        GameManager.Instance.GameLoop.PrepareReveal();
        yield return null;
    }

    public override IEnumerator Enter()
    {
        GameManager.Instance.GameLoop.StartLoop();
        yield return null;
    }

    public override IEnumerator Exit()
    {
        GameManager.Instance.GameLoop.EndLoop();
        GameManager.Instance.Upgrade.ClearTemporaryUpgrades();
        yield return null;
    }
}
