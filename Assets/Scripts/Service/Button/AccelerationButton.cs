
using System;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationButton : MonoBehaviour
{
    [SerializeField] List<GameObject> accelCountImg = new();
    private ServerList serverList;

    private void Start()
    {
        serverList = FindFirstObjectByType<ServerList>();
        // 조건
    }

    private int accelCount;
    public event Action OnClicked;
    private void OnEnable()
    {
        accelCount = accelCountImg.Count;
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
