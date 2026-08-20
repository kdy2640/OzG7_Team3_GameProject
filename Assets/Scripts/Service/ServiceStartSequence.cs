using System.Collections;
using UnityEngine;

public class ServiceStartSequence : MonoBehaviour
{
    [SerializeField] private UI_SalesStartPanel salesStartPanel;
    [SerializeField, Min(0f)] private float visibleDuration = 1.5f;

    public IEnumerator Run()
    {
        if (salesStartPanel == null)
        {
            Debug.LogError("[ServiceStartSequence] SalesStartPanel이 연결되지 않았습니다.", this);
            yield break;
        }

        yield return salesStartPanel.Show();
        yield return new WaitForSeconds(visibleDuration);
        yield return salesStartPanel.Hide();
    }
}
