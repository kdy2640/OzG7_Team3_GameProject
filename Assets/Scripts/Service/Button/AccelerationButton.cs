
using System;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationButton : MonoBehaviour
{
    [SerializeField] List<GameObject> accelCountImg = new();
    private ServerList serverList;
    private int level;
    private int accelCount;

    public event Action OnClicked;

    private void OnEnable()
    {
        accelCount = accelCountImg.Count;
    }

    private void Start()
    {
        level = GameManager.Instance.Upgrade.RuntimeLevel.Get(FacilityType.Decor_5);
        if (level <= 0)
            Destroy(gameObject);

        serverList = FindFirstObjectByType<ServerList>();
        // 조건
    }

    public void OnClick()
    {
        if(serverList.Acceled)
        {
            return;
        }

        if(accelCount <=0) return;

        accelCount--;

        accelCountImg[accelCount].SetActive(false);

        OnClicked?.Invoke();
    }
}
