
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> accelCountImg = new();
    [SerializeField] private Image fillImg;

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
        fillImg.gameObject.SetActive(false);
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
        if (!isAcceled)
            return;

        timer -= Time.deltaTime;

        fillImg.fillAmount = timer / accelDuration;

        if(timer <= 0f)
            Decel();
    }

    public void OnClick()
    {
        Accel();
    }

    private void Accel()
    {
        if (isAcceled || accelCount <= 0)
            return;

        fillImg.gameObject.SetActive(true);
        accelCount--;
        timer = accelDuration;
        isAcceled = true;

        serverList.Acceleration(percentage);
        kitchenSlotHandler.Acceleration(percentage);

        accelCountImg[accelCount].SetActive(false);
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_Acceleration);
    }

    private void Decel()
    {
        isAcceled = false;
        fillImg.gameObject.SetActive(false);
        serverList.Deceleration();
        kitchenSlotHandler.Deceleration();
    }


}
