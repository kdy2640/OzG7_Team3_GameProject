using System.Collections;
using UnityEngine;

public class ServiceEndSequence : MonoBehaviour
{
    [SerializeField] private UI_DailySalesPanel salesResultPanel;

    public IEnumerator Run(SalesResultData result)
    {
        if (salesResultPanel == null || result == null)
        {
            Debug.LogError("[ServiceEndSequence] 결과 패널 또는 결과 데이터가 없습니다.", this);
            yield break;
        }

        salesResultPanel.SetData(result);

        yield return salesResultPanel.Show();
        yield return new WaitUntil(() => salesResultPanel.IsExitRequested);
        yield return salesResultPanel.Hide();
    }
}
