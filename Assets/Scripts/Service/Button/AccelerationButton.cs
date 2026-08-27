
using System;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationButton : MonoBehaviour
{
    [SerializeField] List<GameObject> accelCountImg = new();
    private ServerList serverList;
    private KitchenSlotHandler kitchenSlotHandler;
    private int level;
    private int accelCount;
    private float accelDuration;
    private float percentage;
    private bool isAcceled;
    private float timer;

    private void OnEnable()
    {
        accelCount = accelCountImg.Count;
        serverList = FindFirstObjectByType<ServerList>();
        kitchenSlotHandler = FindFirstObjectByType<KitchenSlotHandler>();
    }

    private void Start()
    {
        level = GameManager.Instance.Upgrade.RuntimeLevel.Get(FacilityType.Decor_5);
        if (level <= 0)
            Destroy(gameObject);

        accelDuration = 10f + 5f * level;
        percentage = (100 + 20f * (level - 1) + 5f * (level - 1) * (level - 2)) / 100f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            Decel();
            isAcceled = false;
        }
    }

    public void OnClick()
    {
        Accel();
    }

    private void Accel()
    {
        accelCount--;

        timer = accelDuration;

        if (isAcceled)
            return;

        if (accelCount <= 0)
            return;
        isAcceled = true; 

        serverList.Acceleration(percentage);
        kitchenSlotHandler.Acceleration(percentage);

        accelCountImg[accelCount].SetActive(false);
    }

    private void Decel()
    {
        isAcceled = false;
        serverList.Deceleration();
        kitchenSlotHandler.Deceleration();
    }


}
