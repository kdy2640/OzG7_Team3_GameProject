using System.Collections;

public class HubScene : SceneBase
{
    public override SceneType SceneType => SceneType.Hub;
    public override string SceneName => "HubScene";

    public override IEnumerator Enter()
    {
        // 업그레이드 씬 진입 시 데이터 준비
        yield return null;
    }

    public override IEnumerator Exit()
    {
        // 업그레이드 결과 확정
        // RuntimeStat 계산 준비
        yield return null;
    }
}