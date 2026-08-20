using System.Collections;
using UnityEngine;

public class HarvestPreStart : MonoBehaviour
{
    [SerializeField] private StartUI startUI;

    private void Awake()
    {
        if (startUI != null)
            startUI.gameObject.SetActive(false);
    }

    public IEnumerator Run()
    {
        if (startUI == null)
        {
            Debug.LogError("[HarvestPreStart] StartUI가 연결되지 않았습니다.", this);
            yield break;
        }

        startUI.gameObject.SetActive(true);
        yield return startUI.PlayRoutine();
    }
}
