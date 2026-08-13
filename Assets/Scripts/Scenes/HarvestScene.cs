using System.Collections;

public class HarvestScene : SceneBase
{
    public override SceneType SceneType => SceneType.Harvest;
    public override string SceneName => "HarvestScene";

    public override IEnumerator PrepareBeforeReveal()
    {
        GameManager.Instance.Harvest.PrepareReveal();
        yield return null;
    }

    public override IEnumerator Enter()
    {
        GameManager.Instance.Harvest.StartLoop();
        yield return null;
    }

    public override IEnumerator Exit()
    { 
        GameManager.Instance.Market.MoveToNextPhase();
        yield return null;
    }
}
