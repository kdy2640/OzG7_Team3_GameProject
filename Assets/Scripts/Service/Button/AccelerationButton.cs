
using System;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationButton : MonoBehaviour
{
    [SerializeField] List<GameObject> accelCountImg = new();
    
    private int accelCount;
    public event Action OnClicked;
    private void OnEnable()
    {
        accelCount = accelCountImg.Count;
    }

    public void OnClick()
    {
        if(accelCount <=0) return;

        accelCount--;

        accelCountImg[accelCount].SetActive(false);

        OnClicked?.Invoke();
    }
}
